using System;
namespace contracts.Responses
{
    public class ListRegisteredUsers
    {
        public string UserName { set; get; }
        public string Email { set; get; }
        public string ProfilePicture { set; get; }
        public string Role { set; get; }
        public string FullName { set; get; }
        public string Phone { set; get; }

    }
}
