using System.ComponentModel.DataAnnotations;

namespace Alumni.Models.Admin;

public class AdminUserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Institutional Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role selection is required.")]
    [Display(Name = "System Role")]
    public string Role { get; set; } = "Alumni";

    [MinLength(6, ErrorMessage = "New password must be at least 6 characters if provided.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password (Leave blank to keep existing)")]
    public string? NewPassword { get; set; }

    [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    public string? ConfirmNewPassword { get; set; }

    public bool IsCurrentAdmin { get; set; }
}
