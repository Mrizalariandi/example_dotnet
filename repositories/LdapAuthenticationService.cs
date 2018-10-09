using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using contracts;
using data_access;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using services;
using data_access.entities;
using contracts.Requests;
using contracts.Responses;

namespace repositories
{
    public class LdapAuthenticationService :CoreRepository, IAuthenticationService
    {
        private const string MemberOfAttribute = "memberOf";
        private const string DisplayNameAttribute = "displayName";
        private const string SAMAccountNameAttribute = "sAMAccountName";
        private const int loginDuration = 30; // Minutes

        private readonly LdapConfig _config;
        private readonly LdapConnection _connection;

        // Aspnet Identity Role
        public RoleManager<IdentityRole> _roleManager;
        public UserManager<UserProfile> _userManager;


        public LdapAuthenticationService(DBBakumContext db,IOptions<LdapConfig> config,RoleManager<IdentityRole> roleManager,UserManager<UserProfile> userManager):base(db)
        {
            _config = config.Value;
            _connection = new LdapConnection
            {
                SecureSocketLayer = false
            };

            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        public AppUser checkToken(string token)
        {
            // Get Token From Database
            var data = from x in this.Db.Sessions
                                    from u in this.Db.Users
                                     from role in this.Db.UserRoles
                       where x.UserName == u.UserName
                                     && x.Token==token
                                     && role.UserId==u.Id
                       select new AppUser {
                 Token = x.Token,
                 DisplayName = u.UserName,
                 Username = x.UserName,
                Role = role.RoleId
            };

            if(data.Count()>0){
                return data.First();
            }

            return new AppUser { Username="", DisplayName="" };
        }

        public string insertToken(string username,string DN,string ip,string platform,int token_minutes)
        {
            string token =  Guid.NewGuid().ToString();

            // ADD to Tables
            var data = new data_access.entities.SessionLog();
            data.UserName = username;
            data.IP = ip;
            data.Platform = platform;
            data.Token = token;
            data.DateCreate = DateTime.Now;
            data.DateEnd = DateTime.Now.AddMinutes(token_minutes);
            data.DN = DN;

            this.Db.Sessions.Add(data);
            this.Db.SaveChanges();
            return token;
        }

        public async Task<AppUser> Login(string username, string password)
        {
           
            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_PORT);
                _connection.Bind(_config.BindDn, _config.BindCredentials);

               /* var result = _connection.Search(
                    _config.SearchBase,
                   LdapConnection.SCOPE_SUB,
                    string.Format(_config.SearchFilter, username),
                    new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute },
                   false
               );*/
			   
			    var result = _connection.Search(
                    "OU=divkum,DC=divkum,DC=polri,DC=go,DC=id",
                   LdapConnection.SCOPE_SUB,
                    string.Format("(|(uid=*{0}*)(displayName=*{0}*)(cn=*{0}*)(sn=*{0}*))", username),
                    new[] { "cn", "displayName", "title" },
                   false
               );

                var user = result.next();
                if (user != null)
                {
                    _connection.Bind(user.DN, password);
                    if (_connection.Bound)
                    {

                        // Get Token and Insert Token '
                        string token = this.insertToken(username,user.DN,"127.0.0.1","",loginDuration);
                        string displayName = user.getAttribute(DisplayNameAttribute)!=null? user.getAttribute(DisplayNameAttribute).StringValue:username;
                        // bool IsAdmin = user.getAttribute(MemberOfAttribute) != null ? user.getAttribute(MemberOfAttribute).StringValueArray.Contains(_config.AdminCn) : false;

                        // Copy Record Login UserName only to Table "AspNetUsers"
                        await this.createUser(username, password,"","","","");

                        // Get Role 
                        var role = await this._roleManager.FindByNameAsync(username);

                        return new AppUser
                        {
                            DisplayName = displayName,
                            Username = username,
                            Token = token,
                            Role = role != null ? role.Name : ""
                        };



                    }
                }
            }
            catch(Exception exc)
            {
		Console.WriteLine(exc.Message);
              //  throw new Exception("Windows Account Login failed."+exc.Message);


                // Trying Login from database
                var identity = await this._userManager.FindByNameAsync(username);
                var passwordMatch = await _userManager.CheckPasswordAsync(identity,password);

                if(passwordMatch){
                    // Get Token and Insert Token '
                    string token = this.insertToken(username, username, "127.0.0.1", "", loginDuration);
                    // Get Role 
                    var role = await this._roleManager.FindByNameAsync(username);

                    return new AppUser
                    {
                        DisplayName = identity.UserName,
                        Username = identity.UserName,
                        Token = token,
                        Role = role != null ? role.Name : ""
                    };
                }

            }
            _connection.Disconnect();
            return null;
        }

        public async Task removeToken(string token)
        {
            var data = this.Db.Sessions.Where(a => a.Token == token);
            if(data.Count()>0){
                this.Db.Sessions.Remove(data.First());
                await this.Db.SaveChangesAsync();
            }
        }

        private async Task<string> addRole(string role){
            
            if (!await this._roleManager.RoleExistsAsync(role) && !string.IsNullOrWhiteSpace(role))
            {
                await this._roleManager.CreateAsync(new IdentityRole { Name = role});
            }else{
                role = "";
            }

            return role;
        }

        private async Task<string> createUser(string username,string password,string namalengkap,string profilepicture,string phonenumber,string email){

            string response = string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var user = await this._userManager.FindByNameAsync(username);
            IdentityResult result;
            if (user == null)
            {

                result = await this._userManager.CreateAsync(new UserProfile
                {
                    UserName = username,
                    ProfilePicture = profilepicture,
                    PhoneNumber = phonenumber,
                    Email = email,
                    FullName = namalengkap

                },
                    password);

                foreach (var item in result.Errors)
                {
                    sb.Append(item.Description);
                }

                if (result.Errors.Count() > 0)
                {
                    return await Task.Run(() => response = sb.ToString());
                }

            }

            return response;
        }

        private async Task<string> addUserRoleByUserName(string username,string role){

            string errMsg=string.Empty;
            try
            {
                // Find User By username;
                var identity = await this._userManager.FindByNameAsync(username);

                // remove first
                await this._userManager.RemoveFromRoleAsync(identity, role);


                // add user role
                await this._userManager.AddToRoleAsync(identity, role);

            }
            catch(Exception ex)
            {
                errMsg = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
            }


            return errMsg;

        }

        /// <summary>
        ///  Fungsi ini hanya untuk inisialisasi awal deployment ke production.
        /// </summary>
        /// <returns>The user.</returns>
        public async Task simulateUser()
        {
            // Add Roles:

            // Add Role Administrator
            await this.addRole("Administrator");
            // Add Role Operator
            await this.addRole("Operator");

            // Add Users:

            // Add user : adamb password:adamb
            //await this.createUser("adamb", "P@ss1W0Rd!");

            // Add user :admin password: P@ss1W0Rd!
            await this.createUser("admin_dev", "P@ss1W0Rd!","","","","");



            for (int i = 1; i < 10; i++){
                await this.createUser("user"+i, "P@ss1W0Rd!","","","","");
            }


            // Add User Role
            await this.addUserRoleByUserName("adamb", "Operator");
            await this.addUserRoleByUserName("admin_dev", "Administrator");


        }

        public  Response<UserInfoDirectory[]> getUsersActiveDirectory()
        {
            Response<UserInfoDirectory[]> response = new Response<UserInfoDirectory[]> { };

            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_PORT);
                _connection.Bind(_config.BindDn, _config.BindCredentials);

                var result = _connection.Search(
                    "OU=divkum,DC=divkum,DC=polri,DC=go,DC=id",
                   LdapConnection.SCOPE_SUB,
                    string.Format("objectclass=person"),
                    new[] { "cn", "displayName", "title" },
                   false,null,null
               );

                LdapMessage message;
                List<UserInfoDirectory> dns = new List<UserInfoDirectory> { };
				int xx=0;
                while (( message = result.getResponse()) != null)
                {
					xx++;
					
					 if (!(message is LdapResponse xxxx))
                    {
                       LdapSearchResult entry = (LdapSearchResult)message;
                    
					 
					 var read = _connection.Read(entry.Entry.DN);
					 var ds =read.getAttribute("displayName");
					 string displayName = ds!=null ? read.getAttribute("displayName").StringValue : "empty" ;
					 var uids =  read.getAttribute("cn");
					 string uid = uids!=null ? read.getAttribute("cn").StringValue : "empty";
					 var ts = read.getAttribute("title");
					 string title = ts!=null ? read.getAttribute("title").StringValue: "empty";
					 
					dns.Add(new UserInfoDirectory{
						
							DisplayName=displayName,
							Username = uid,
							//Title = title
						});
                    }
					// 
					/*
                    if (!(message is LdapResponse xxxx))
                    {
                        LdapSearchResult entry = (LdapSearchResult)message;
                        var read = _connection.Read(entry.Entry.DN);


                        //{LdapEntry: cn=Dan Jump,ou=users,dc=contoso,dc=com; LdapAttributeSet: LdapAttribute: {type='title', value='CEO'} LdapAttribute: {type='userPassword', value='{SHA}RKkNn7+KoG94IN3x/B2jnm/4DS0='} LdapAttribute: {type='gidNumber', value='70001'} LdapAttribute: {type='displayName', value='Dan Jump'} LdapAttribute: {type='uid', value='danj'} LdapAttribute: {type='homeDirectory', value='/home/danj'} LdapAttribute: {type='mail', value='danj@contoso.com'} LdapAttribute: {type='objectClass', values='inetOrgPerson','person','organizationalPerson','posixAccount','top'} LdapAttribute: {type='givenName', value='Dan'} LdapAttribute: {type='cn', value='Dan Jump'} LdapAttribute: {type='sn', value='Jump'} LdapAttribute: {type='telephoneNumber', value='(425) 555-0179'} LdapAttribute: {type='uidNumber', value='70000'}}
                       // read.getAttribute("").StringValue;


                        dns.Add(new UserInfoDirectory { 
                            DisplayName=read.getAttribute("displayName").StringValue,
                            Username = read.getAttribute("uid").StringValue,
                            //DN = entry.Entry.DN,
                            Title = read.getAttribute("title").StringValue
                             
                        });
                    }*/
					

                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }
                }

                response.message_type = 1;
                response.data = dns.ToArray();
            }
            catch(Exception exc)
            {
                throw new Exception("Search Error"+exc.Message);

            }
            _connection.Disconnect();
            return response;
        }

        public async Task<Response<string>> mapUserToRole(string username, string rolename)
        {

            //  Add to User Table
            await this.createUser(username, "default","","","","");

            // Add To Role Table by RoleId
            await this.addUserRoleByUserName(username, rolename);

            var res = new Response<string>();
            res.data = rolename;
            return res;
        }

        public Response<string[]> getAllRoles()
        {
            Response<string[]> response = new Response<string[]>();

            response.data = this._roleManager.Roles.Select(a => a.Name).ToArray();

            return response;
        }

        public async Task<Response<UpdateProfileResponse>> updateProfile(UpdateProfileRequest request,string fileProfile)
        {
            Response<UpdateProfileResponse> response = new Response<UpdateProfileResponse>();

            try
            {
                if (!string.IsNullOrWhiteSpace(request.UserName))
                {
                    UserProfile user = await this._userManager.FindByNameAsync(request.UserName);

                    //if (string.IsNullOrWhiteSpace(user.ProfilePicture))
                    //{
                        if (!string.IsNullOrWhiteSpace(fileProfile))
                        {
                            user.ProfilePicture = fileProfile;
                        }
                    //}

                    user.FullName = request.Namalengkap;
                    user.Email = request.Email;
                    user.PhoneNumber = request.NoTelp;
                    this.Db.Users.Update(user);
                    this.Db.SaveChanges();
                }
            }
            catch(Exception exc){

                response.message = exc.Message;
                response.message_type = 2;
            }

            return response;
        }

        public async Task<Response<UpdateProfileResponse>> updateUser(UpdateProfileRequest request, string fileProfile)
        {
            Response<UpdateProfileResponse> response = new Response<UpdateProfileResponse>();

            try
            {
                if (!string.IsNullOrWhiteSpace(request.UserName))
                {
                    UserProfile user = await this._userManager.FindByNameAsync(request.UserName);

                    //if (string.IsNullOrWhiteSpace(user.ProfilePicture))
                    //{
                    if (!string.IsNullOrWhiteSpace(fileProfile))
                    {
                        user.ProfilePicture = fileProfile;
                    }
                    //}

                    user.FullName = request.Namalengkap;
                    user.Email = request.Email;
                    user.PhoneNumber = request.NoTelp;
                    this.Db.Users.Update(user);
                    this.Db.SaveChanges();

                    // update role
                    this.addUserRoleByUserName(request.UserName, request.Role).GetAwaiter().GetResult();
                }
            }
            catch (Exception exc)
            {

                response.message = exc.Message;
                response.message_type = 2;
            }

            return response;
        }

        public Response<ListRegisteredUsers[]> listRegisteredUser(FilterRegisteredUser request)
        {
            Response<ListRegisteredUsers[]> response = new Response<ListRegisteredUsers[]> { };


            var query = from x in this.Db.Users
                                       from y in this.Db.UserRoles
                                      from w in this.Db.Roles
                        where x.Id==y.UserId && w.Id==y.RoleId
                        select new ListRegisteredUsers {
                 Email = x.Email,
                 FullName = x.FullName,
                 Phone = x.PhoneNumber,
                 UserName = x.UserName,
                ProfilePicture = "staticFiles/"+x.ProfilePicture,
                             Role = w.Name
                        };

            if(!string.IsNullOrEmpty(request.Role)){
                query = query.Where(a => a.Role == request.Role);
            }




            // Paging
            var count = query.Count();
            var items = query.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToArray();

            response.data = items;

            // Property Information for javascript client
            response.count = count;
            response.totalpages = (int)Math.Ceiling(count / (double)request.pageSize);
            response.totalpages = response.totalpages < 0 ? 1 : response.totalpages;

            return response;
        }

        public async Task<Response<string>> removeUser(string username)
        {
            Response<string> response = new Response<string>();

            if(!string.IsNullOrWhiteSpace(username)){

                var user = await this._userManager.FindByNameAsync(username);
                if (user != null)
                {
                    var query = await this._userManager.DeleteAsync(user);
                    await this.Db.SaveChangesAsync();
                }

                // Remove All Role for this user

                var roles = this.Db.UserRoles.Where(a => a.UserId == user.Id);

                if (roles.Count() > 0)
                {
                    foreach (var item in roles)
                    {
                        this.Db.UserRoles.Remove(item);
                    }
                    await this.Db.SaveChangesAsync();
                }
            }

            return response;
        }

        public Response<UpdateProfileResponse> getUser(UpdateProfileRequest request)
        {
            Response<UpdateProfileResponse> response = new Response<UpdateProfileResponse>();

            var query = this.Db.Users.SingleOrDefault(a => a.UserName == request.UserName);

            response.data = new UpdateProfileResponse{ Email = query.Email,
             Namalengkap = query.FullName, NoTelp = query.PhoneNumber, UserName = query.UserName, 
                ProfilePicture = query.ProfilePicture};

            // get role 
            var roleid = this._userManager.GetRolesAsync(query).GetAwaiter().GetResult();

            if(roleid.Count()>0){
                string roleId = roleid.First();
                var rolemaster = this._roleManager.FindByNameAsync(roleId);
                response.data.Role = rolemaster.GetAwaiter().GetResult().Name;
            }

            return response;
        }

        public Response<int> createMenu(MenuRequest request)
        {
            Response<int> response = new Response<int>();

            var getParent = this.Db.Menus.SingleOrDefault(a => a.Id == request.ParentMenu);
            Menu menu = new Menu{

                Title = request.Title,
                Description = request.Description,
                IsFolder = request.IsFolder,
                 Css = request.Css,
                 Icon = request.Icon,
                 IsMobile = request.IsMobile,
                ParentMenu = getParent,
                 UISRef = request.UISRef,
                 CreatedBy = request.UserLogin,
                CreatedDate  = DateTime.Now,
                 ModifiedDate = DateTime.Now,
                 ModifiedBy = request.UserLogin
            };
            this.Db.Menus.Add(menu);
            this.Db.SaveChanges();

            response.data = menu.Id;

            return response;

        }

        public Response<int> updateMenu(MenuRequest request)
        {
            Response<int> response = new Response<int>();


            if(request.Id>0){
                var data = this.Db.Menus.SingleOrDefault(a => a.Id == request.Id);
                var getParent = this.Db.Menus.SingleOrDefault(a => a.Id == request.ParentMenu);

                data.Title = request.Title;

                data.Description = request.Description;
                data.IsFolder = request.IsFolder;
                data.Css = request.Css;
                data.Icon = request.Icon;
                data.IsMobile = request.IsMobile;
                data.ParentMenu = getParent;
                data.UISRef = request.UISRef;
                data.ModifiedBy = request.UserLogin;
                data.ModifiedDate = DateTime.Now;

                this.Db.Menus.Update(data);
                this.Db.SaveChanges();
                response.data = data.Id;
            }

            return response;
        }

        public Response<int> deleteMenu(MenuRequest request)
        {
            Response<int> response = new Response<int>();


            if (request.Id > 0)
            {
                var data = this.Db.Menus.SingleOrDefault(a => a.Id == request.Id);
                this.Db.Menus.Remove(data);
                this.Db.SaveChanges();
                response.data = data.Id;
            }

            return response;        
        }

        public Response<MenuRequest> getMenuById(int id)
        {
            Response<MenuRequest> response = new Response<MenuRequest>();

            var data = this.Db.Menus.SingleOrDefault(a => a.Id == id);

            if (data != null)
            {
                response.data = new MenuRequest
                {

                    Title = data.Title,
                     Css = data.Css,
                     Description = data.Description,
                     Icon = data.Icon,
                    IsFolder = data.IsFolder,
                     IsMobile = data.IsMobile,
                     ParentMenu = data.ParentMenu.Id,
                     UISRef = data.UISRef,
                     Id = data.Id

                };
            }

            return response;
        }

        public Response<int> addRoleToMenu(string role, int id)
        {
            Response<int> response = new Response<int>();


           

            if(!string.IsNullOrWhiteSpace(role) && id>0){
                var roledata = this._roleManager.FindByNameAsync(role).GetAwaiter().GetResult();
                var menu = this.Db.Menus.SingleOrDefault(a => a.Id == id);
                if(roledata!=null && menu!=null){

                    // Delete
                    var datas = this.Db.MenuRoles.Where(a => a.Role == roledata && a.Menu==menu);
                    if(datas.Count()>0){
                        this.Db.MenuRoles.RemoveRange(datas);
                    }

                    // Add
                    this.Db.MenuRoles.Add(new MenuRoles { Role = roledata, Menu = menu });

                    this.Db.SaveChanges();
                }


            }

            return response;
        }

        public Response<MenuResponse[]> getMenus(string RoleId)
        {
            Response<MenuResponse[]> response = new Response<MenuResponse[]>();
            var getRoot = from mn in this.Db.Menus.Where(a => a.ParentMenu == null)
                          from rl in this.Db.MenuRoles
                          where rl.Role.Id == RoleId
                          select mn;
            
            List<MenuResponse> menus = new List<MenuResponse>();

            if (getRoot.Count() > 0)
            {
                foreach (var item in getRoot)
                {
                    var child = this.getChilds(item.Id, RoleId);
                    menus.Add(new MenuResponse
                    {
                        Title = item.Title,
                        Children = child,
                        Description = item.Description,
                        UISRef = item.UISRef,
                        IsMobile = item.IsMobile,
                        IsFolder = item.IsFolder,
                        Css = item.Css,
                        Icon = item.Icon,
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = item.ModifiedBy
                    });
                }

                response.data = menus.ToArray();
            }

            return response;
        }

        private MenuResponse[] getChilds(int id,string RoleId){
            var getRoot = from mn in this.Db.Menus.Where(a => a.ParentMenu.Id == id)
                              from rl in this.Db.MenuRoles
                              where rl.Role.Id == RoleId
                              select mn;
            List<MenuResponse> menus = new List<MenuResponse>();

            if (getRoot.Count() > 0)
            {
                foreach (var item in getRoot)
                {
                    menus.Add(new MenuResponse
                    {
                        Title = item.Title,
                        Children = this.getChilds(item.Id, RoleId),
                        Description = item.Description,
                        UISRef = item.UISRef,
                        IsMobile = item.IsMobile,
                        IsFolder = item.IsFolder,
                        Css = item.Css,
                        Icon = item.Icon,
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = item.ModifiedBy
                    });
                }
            }
            return menus.ToArray();
        }

       
    }
}
