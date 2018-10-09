using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using contracts;
using contracts.Requests;
using contracts.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using services;

namespace api_bakum.Controllers
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }

    public class UploadStatus{
        public string FileName { set; get; }
        public bool Status { set; get; }
    }

    public class Apis : Controller
    {
        IBantuanHukumService bantuanHukumService;
        private readonly services.IAuthenticationService _authService;
        private readonly AppConfig _config;

        public Apis(IBantuanHukumService bantuanHukumService,services.IAuthenticationService authService,IOptions<AppConfig> config)
        {
            this.bantuanHukumService = bantuanHukumService;
            _authService = authService;
            _config = config.Value;

        }



        private async Task<UploadStatus> uploadFile(Microsoft.AspNetCore.Http.IFormFile media,string newFileName)
        {
            string message = string.Empty;
            bool status = false;
            if (media != null)
            {
                // Create full path and save the original media file
                newFileName = string.IsNullOrWhiteSpace(newFileName) ? media.FileName:newFileName;
                var saveDir = Path.Combine("aspnetcore_files", newFileName);


               
                if(System.IO.File.Exists("aspnetcore_files/"+newFileName)){
                    newFileName = Guid.NewGuid().ToString() + ".png";
                    saveDir = Path.Combine("aspnetcore_files",newFileName);
                }

                using (var stream = new FileStream(saveDir, FileMode.Create))
                {
                    var ext = Path.GetExtension(media.FileName);
                    var fAllows = _config.AllowFileType.Contains(ext.ToLower());
                    if(fAllows==false){
                        message = "File hanya diperbolehkan "+_config.AllowFileType;
                        return new UploadStatus { FileName = message, Status = status };

                    }

                    if (media.Length < _config.MaxSizeUploadFile && fAllows)
                    {
                           await media.CopyToAsync(stream);
                        message =newFileName;
                        status = true;

                    }else{
                        message = "File maksimal berukuran "+(_config.MaxSizeUploadFile/1048576)+"Mb";
                    }
                }

            }
            return new UploadStatus{ FileName = message,Status=status};
        }


        [Route("api/bantuanhukum/request")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public async Task<Response<FormBantuanHukumResponse>> request(FormBantuanHukumRequest request)
        {
            Response<FormBantuanHukumResponse> response = new Response<FormBantuanHukumResponse> { };
            var status = await this.uploadFile(request.File,string.Empty);

            if(status.Status){
                return this.bantuanHukumService.request(request, status.FileName);
            }else{
                // Error file upload
                response.message_type = 2;
                response.message = string.IsNullOrWhiteSpace(status.FileName) ? "Attachment file mohon diisi":status.FileName;
            }
            return response;
        }

        [Route("api/bantuanhukum/reply")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public async Task<Response<FormBantuanHukumResponse>> reply(ReplyConversationRequest request)
        {
            var response = new Response<FormBantuanHukumResponse> { };
            string userlogin=string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
          
            var lastStatus = this.bantuanHukumService.getLastStatusPermohonanByKodeLaporan(request.KodeLaporan);
          
            if(!string.IsNullOrWhiteSpace(info.Token)){
                userlogin = info.Username;
            }else{
                // Non Login
                userlogin = "user_"+request.KodeLaporan;
                // Validate, if User Non Login. Can't Change Status.
                request.Status = lastStatus.Status;

                var createdBy = this.bantuanHukumService.getLastConversationByHeaderID(lastStatus.HeaderID);
                if (createdBy==userlogin)
                {
                    response.message = "Posting komentar tidak bisa dilakukan sampai Operator kami merespon terlebih dahulu";
                    response.message_type = 2;
                    return response;
                }
            }


            if (request.File!=null && request.File.Length > 0)
            {
                var status = await this.uploadFile(request.File, string.Empty);

                if (status.Status)
                {
                    //return this.bantuanHukumService.request(request, status.FileName);

                    // Make sure Can't submit comment if not yet respond from Operator.
                    response = this.bantuanHukumService.reply(request, status.FileName, userlogin);
                }
            }else{
                response = this.bantuanHukumService.reply(request, string.Empty, userlogin);

            }

            return response;
        }

        [Route("api/bantuanhukum/list")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public Response<ListPermohonanResponse[]> listpermohonan([FromBody]FilterPermohonanRequest request){
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<ListPermohonanResponse[]> response = new Response<ListPermohonanResponse[]>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this.bantuanHukumService.listpermohonan(request);
            }else{
                response.message = "Unauthorize";
            }
            return response;
        }

        [Route("api/bantuanhukum/listNotification")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public Response<ConversationResponse[]> listNotification([FromBody]ListMessageRequest request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<ConversationResponse[]> response = new Response<ConversationResponse[]>();

            //if (!string.IsNullOrEmpty(info.Username))
            //{
            return this.bantuanHukumService.listNotification(request);
            //}
            //else
            //{
            //  response.message = "Unauthorize";
            // response.message_type = 2;
            //}
            // return response;
        }

        [Route("api/bantuanhukum/listMessage")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public Response<ConversationResponse[]> listMessage([FromBody]ListMessageRequest request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<ConversationResponse[]> response = new Response<ConversationResponse[]>();

            //if (!string.IsNullOrEmpty(info.Username))
            //{
                return this.bantuanHukumService.listMessages(request);
            //}
            //else
            //{
              //  response.message = "Unauthorize";
               // response.message_type = 2;
            //}
           // return response;
        }

        [Route("api/bantuanhukum/getbykodelaporan/{kodelaporan}")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public Response<ListPermohonanResponse> getbykodelaporan(string kodelaporan)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<ListPermohonanResponse> response = new Response<ListPermohonanResponse>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                ListPermohonanResponse[] datas = this.bantuanHukumService.listpermohonan(new FilterPermohonanRequest{ KodeLaporan=kodelaporan, pageIndex=1, pageSize=1}).data;
                if(datas.Length>0){
                    response.data = datas[0];   
                }
            }
            else
            {
                response.message = "Unauthorize";
            }
            return response;
        }

        [Route("api/countries")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public DropDown[] countries()
        {

            return this.bantuanHukumService.getCountries();
        }

        [Route("api/login")]
        [HttpPost]
        public async Task<Response<AppUser>> Login([FromBody]LoginViewModel model)
        {
            Response<AppUser> response = new Response<AppUser>();
            try
            {
                AppUser user = await _authService.Login(model.UserName, model.Password);
                if (null != user)
                {   
                    response.data = user;
                    response.message_type = 1;
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
            }


            return response;

        }

        [Route("api/logout/{token}")]
        [HttpGet]
        public async Task<string> LogoutApi(string token)
        {
            // Remove Token;
            await this._authService.removeToken(token);
            return "Logout";
        }

        [Route("api/init")]
        [HttpGet]
        public async Task<string> init()
        {
            await this._authService.simulateUser();
            return "simulateUser";
        }

        [Route("api/getUserActiveDirectory")]
        [HttpGet]
        public  Response<UserInfoDirectory[]> get()
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];
            Response<UserInfoDirectory[]> response = new Response<UserInfoDirectory[]> { };

            // Get user login from token
            var info = this._authService.checkToken(token);
            UploadStatus statusUpload = new UploadStatus();
            if (!string.IsNullOrEmpty(info.Username))
            {

                if (this._config.isActiveDirectoryUsers == 1)
                {
                    return this._authService.getUsersActiveDirectory();
                }
                UserInfoDirectory[] dataDumy = new UserInfoDirectory[] {
                new UserInfoDirectory{ Username="Boby27538", DisplayName="Mr.Jhoby Cuk"},
                new UserInfoDirectory{ Username="Cy4538", DisplayName="Mr.Cromi ya"},
                new UserInfoDirectory{ Username="U2b4lf", DisplayName="Mr.Cromi ya"}

            };


                response.data = dataDumy;
            }else{
                response.message_type = 2;
                response.message = "Unauthorize";
            }
                return response;
        }

        [Route("api/getAllRoles")]
        [HttpGet]
        public Response<string[]> getAllRoles()
        {
            return this._authService.getAllRoles();
        }

        [Route("api/mapUserToRole")]
        [HttpPost]
        public async Task<Response<string>> mapUserToRole([FromBody]UserToRoleRequest request)
        {
            return await this._authService.mapUserToRole(request.username,request.rolename);
        }
        [Route("api/updateprofile")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public async Task<Response<UpdateProfileResponse>> updateprofile(UpdateProfileRequest request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<UpdateProfileResponse> response = new Response<UpdateProfileResponse>();
            UploadStatus statusUpload = new UploadStatus();
            if (!string.IsNullOrEmpty(info.Username))
            {
                if (request.File!=null && request.File.Length > 0)
                {
                    statusUpload = await this.uploadFile(request.File, request.UserName+".png");
                    if(statusUpload.Status){
                        return await this._authService.updateProfile(request, statusUpload.FileName);
                    }
                    response.message_type = 2;
                    response.message = statusUpload.FileName;// Error upload file profile
                }
                return await this._authService.updateProfile(request,statusUpload.FileName);
            }
            else
            {
                response.message_type = 2;
                response.message = "Unauthorize";
            }
            return response;
        }

        [Route("api/bantuanhukum/Progress/{KodeLaporan}")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public Response<ListStatusProgressMain> listStatusProress(string KodeLaporan)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
           // var info = this._authService.checkToken(token);
            Response<ListStatusProgressMain> response = new Response<ListStatusProgressMain>();

            //if (!string.IsNullOrEmpty(info.Username))
            //{
                return this.bantuanHukumService.listStatusProgress(KodeLaporan);
            //}
            //else
            //{
              //  response.message = "Unauthorize";
                //response.message_type = 2;
            //}
           // return response;
        }

        [Route("api/bantuanhukum/listEmailBlocked")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public Response<ListBlockEmail[]> listEmailBlocked()
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
             var info = this._authService.checkToken(token);
            Response<ListBlockEmail[]> response = new Response<ListBlockEmail[]>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this.bantuanHukumService.listEmailBlocked();
            }
            else
            {
              response.message = "Unauthorize";
            response.message_type = 2;
            }
            return response;
        }

        [Route("api/bantuanhukum/blockingEmail")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public Response<string> blockingEmail([FromBody]ListBlockEmail request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<string> response = new Response<string>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this.bantuanHukumService.blockingEmail(request);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/bantuanhukum/listRegisteredUser")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public Response<ListRegisteredUsers[]> listRegisteredUser([FromBody]FilterRegisteredUser request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<ListRegisteredUsers[]> response = new Response<ListRegisteredUsers[]>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this._authService.listRegisteredUser(request);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/bantuanhukum/removeUser/{username}")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public Response<string> removeUser(string username)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<string> response = new Response<string>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                 this._authService.removeUser(username);
                response.data = username;
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/bantuanhukum/getTemplateByKodeLaporan/{KodeLaporan}/{name}")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public Response<string> getTemplateByKodeLaporan(string KodeLaporan,string name)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<string> response = new Response<string>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                var data = this.bantuanHukumService.getTemplateByKodeLaporan(name, KodeLaporan);
                response.data = data.data;
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/bantuanhukum/read/{ID}")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public Response<string> read(long ID)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<string> response = new Response<string>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this.bantuanHukumService.read(ID);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }


        [Route("api/updateuser")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public async Task<Response<UpdateProfileResponse>> updateuser(UpdateProfileRequest request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<UpdateProfileResponse> response = new Response<UpdateProfileResponse>();
            UploadStatus statusUpload = new UploadStatus();
            if (!string.IsNullOrEmpty(info.Username))
            {
                if (request.File != null && request.File.Length > 0)
                {
                    statusUpload = await this.uploadFile(request.File, request.UserName + ".png");
                    if (statusUpload.Status)
                    {
                        return await this._authService.updateUser(request, statusUpload.FileName);
                    }
                    response.message_type = 2;
                    response.message = statusUpload.FileName;// Error upload file profile
                }
                return await this._authService.updateUser(request, statusUpload.FileName);
            }
            else
            {
                response.message_type = 2;
                response.message = "Unauthorize";
            }
            return response;
        }

        [Route("api/bantuanhukum/getuser/{userid}")]
        [HttpGet]
        [EnableCors("default")]
        [ValidateModel]
        public Response<UpdateProfileResponse> getuser(string userid)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<UpdateProfileResponse> response = new Response<UpdateProfileResponse>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this._authService.getUser(new UpdateProfileRequest{ UserName = userid});
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }


        [Route("api/bantuanhukum/editemailblock")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public Response<string> editemailblock([FromBody] ListBlockEmail request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<string> response = new Response<string>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this.bantuanHukumService.editBlockingEmail(new ListBlockEmail {
                    Email = request.Email,
                     Id = request.Id
                });
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }


        [Route("api/bantuanhukum/removeemailblock")]
        [HttpPost]
        [EnableCors("default")]
        [ValidateModel]
        public Response<string> removeemailblock([FromBody] ListBlockEmail request)
        {
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<string> response = new Response<string>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this.bantuanHukumService.removeBlockingEmail(request.Id);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/createMenu")]
        [HttpPost]
        [EnableCors("default")]
        public Response<int> createMenu([FromBody]MenuRequest request){
         
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<int> response = new Response<int>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                request.UserLogin = info.Username;
                return this._authService.createMenu(request);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/getMenuById/{id}")]
        [HttpGet]
        [EnableCors("default")]
        public Response<MenuRequest> getMenuById(int id){
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<MenuRequest> response = new Response<MenuRequest>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this._authService.getMenuById(id);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/updateMenu")]
        [HttpPut]
        [EnableCors("default")]
        public Response<int> updateMenu([FromBody]MenuRequest request){
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<int> response = new Response<int>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                request.UserLogin = info.Username;
                return this._authService.updateMenu(request);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/deleteMenu/{id}")]
        [HttpDelete]
        [EnableCors("default")]
        public Response<int> deleteMenu(int id){
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<int> response = new Response<int>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this._authService.deleteMenu(new MenuRequest{ Id=id});
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/addRoleToMenu")]
        [HttpPost]
        [EnableCors("default")]
        public Response<int> addRoleToMenu([FromBody]MenuRoleRequest request){
            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<int> response = new Response<int>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this._authService.addRoleToMenu(request.RoleId, request.Menu);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }

        [Route("api/getMenus")]
        [HttpGet]
        [EnableCors("default")]
        public Response<MenuResponse[]> getMenus(){

            string userlogin = string.Empty;
            string token = Request.Headers["Authorization"];

            // Get user login from token
            var info = this._authService.checkToken(token);
            Response<MenuResponse[]> response = new Response<MenuResponse[]>();

            if (!string.IsNullOrEmpty(info.Username))
            {
                return this._authService.getMenus(info.Role);
            }
            else
            {
                response.message = "Unauthorize";
                response.message_type = 2;
            }
            return response;
        }


    }


}
