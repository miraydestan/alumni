using System.ComponentModel.DataAnnotations;

namespace Alumni.Models;

public class CreateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Alumni";
}

public class UpdateUserRequest
{
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    public string? CurrentPassword { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }

    public string? Role { get; set; }
}
