using System;
using System.ComponentModel.DataAnnotations;

namespace contracts
{
    public class LoginViewModel
    {
        [Required]
        //[EmailAddress]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
