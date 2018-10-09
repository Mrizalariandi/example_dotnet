using System;
namespace contracts.Requests
{
    public class UpdateProfileRequest
    {
        public string UserName { set; get; }
        public string Namalengkap { set; get; }
        public string Email { set; get; }
        public string NoTelp { set; get; }
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
        public string Role { set; get; }

    }
}
