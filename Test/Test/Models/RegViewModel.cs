using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class RegViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter First name")]
        public string First_Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Last name")]
        public string Last_Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter email")]
        public string Email { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter password")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Minimum password length 5")]
        public string Password { get; set; }
    }
}