using System;
using System.Threading.Tasks;
using contracts;
using contracts.Requests;
using contracts.Responses;

namespace services
{
    public interface IAuthenticationService
    {
        Task<AppUser> Login(string username, string password);
        string insertToken(string username,string DN, string ip, string platform, int token_minutes);
        AppUser checkToken(string token);
        Task removeToken(string token);
        Task simulateUser();

        Response<UserInfoDirectory[]> getUsersActiveDirectory();
        Response<string[]> getAllRoles();
        Task<Response<string>> mapUserToRole(string username,string roleId);

        // Update Profile
        Task<Response<UpdateProfileResponse>> updateProfile(UpdateProfileRequest request,string fileProfile);
        Task<Response<UpdateProfileResponse>> updateUser(UpdateProfileRequest request, string fileProfile);
        Response<UpdateProfileResponse> getUser(UpdateProfileRequest request);


        Response<ListRegisteredUsers[]> listRegisteredUser(FilterRegisteredUser request);
        Task<Response<string>> removeUser(string username);

        Response<int> createMenu(MenuRequest request);
        Response<MenuRequest> getMenuById(int id);
        Response<int> updateMenu(MenuRequest request);
        Response<int> deleteMenu(MenuRequest request);
        Response<int> addRoleToMenu(string role, int id);
        Response<MenuResponse[]> getMenus(string role);

    }

}
