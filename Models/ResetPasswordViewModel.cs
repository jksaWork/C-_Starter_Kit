using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentiyFreamwork.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } 

        [Required]
        [EmailAddress]
        public string Email { get; set; } 


        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } // User's password

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } // Confirm the password
    }
}