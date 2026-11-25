using Microsoft.AspNetCore.Identity;
using Alumni.Models;

namespace Alumni.Data.Factories;

public static class UserFactory
{
    private static readonly PasswordHasher<User> _hasher = new();

    public static IReadOnlyList<User> GetSeedUsers() =>
    [
        new User { Name = "System Administrator", Email = "admin@alumni.edu.tr", Role = "Administrator", PasswordHash = _hasher.HashPassword(null!, "Admin2026!") },
        new User { Name = "Ayşe Kaya",    Email = "ayse.kaya@alumni.edu.tr",    Role = "Alumni",  PasswordHash = _hasher.HashPassword(null!, "Alumni2026!") },
        new User { Name = "Mehmet Demir", Email = "mehmet.demir@alumni.edu.tr", Role = "Alumni",  PasswordHash = _hasher.HashPassword(null!, "Alumni2026!") },
        new User { Name = "Zeynep Çelik", Email = "zeynep.celik@alumni.edu.tr", Role = "Student", PasswordHash = _hasher.HashPassword(null!, "Student2026!") },
        new User { Name = "Can Yıldız",   Email = "can.yildiz@alumni.edu.tr",   Role = "Alumni",  PasswordHash = _hasher.HashPassword(null!, "Alumni2026!") },
        new User { Name = "Elif Şahin",   Email = "elif.sahin@alumni.edu.tr",   Role = "Student", PasswordHash = _hasher.HashPassword(null!, "Student2026!") },
    ];
}
