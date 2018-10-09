using System;
namespace contracts
{
    public class AppUser
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Token { set; get; }
        public string Role { set; get; }
        public string DN { set; get; }
    }

    public class UserInfoDirectory
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        //public string Title { set; get; }
        //public string DN { set; get; }
    }
}
