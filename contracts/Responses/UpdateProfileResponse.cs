using System;
namespace contracts.Responses
{
    public class UpdateProfileResponse
    {   
        public string UserName { set; get; }
        public string Namalengkap { set; get; }
        public string Email { set; get; }
        public string NoTelp { set; get; }
        public string ProfilePicture { set; get; }
        public string Role { set; get; }
    }
}
