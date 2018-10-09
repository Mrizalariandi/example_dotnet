using System;
using Microsoft.AspNetCore.Identity;

namespace data_access.entities
{
    public class UserProfile : IdentityUser
    {
        public string FullName { set; get; }
        public string ProfilePicture { set; get; }
    }
}
