using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Alumni.Models;

public class EditProfileViewModel
{
    [BindNever]
    public int UserId { get; set; }

    [BindNever]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(150, ErrorMessage = "Email address cannot exceed 150 characters.")]
    [Display(Name = "Institutional Email")]
    public string Email { get; set; } = string.Empty;
}
