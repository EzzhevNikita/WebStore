using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Login is required", AllowEmptyStrings = false)]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required", AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Minimal length of password 5 symbols")]
        public string password { get; set; }

       
    }

    public class RestoreViewModel : LoginViewModel
    {
        [Required(ErrorMessage = "Repeat password", AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Minimal length of password 5 symbols")]
        public string repetedpassword { get; set; }
    }
}