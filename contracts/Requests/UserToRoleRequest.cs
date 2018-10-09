using System;
namespace contracts.Requests
{
    public class UserToRoleRequest
    {
        public string username { set; get; }
        public string rolename { set; get; }
    }
}
