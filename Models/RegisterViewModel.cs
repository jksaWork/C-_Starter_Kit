using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } // User's email address

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } // User's password

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } // Confirm the password

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; } // Option to remember the user

    public List<SelectListItem> SelectRole;
    public string Role {get; set;}
}
