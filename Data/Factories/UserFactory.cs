using Alumni.Models;

namespace Alumni.Data.Factories;

public static class UserFactory
{
    public static IReadOnlyList<User> GetSeedUsers() =>
    [
        new User { Name = "Ayşe Kaya",    Email = "ayse.kaya@alumni.edu.tr" },
        new User { Name = "Mehmet Demir", Email = "mehmet.demir@alumni.edu.tr" },
        new User { Name = "Zeynep Çelik", Email = "zeynep.celik@alumni.edu.tr" },
        new User { Name = "Can Yıldız",   Email = "can.yildiz@alumni.edu.tr" },
        new User { Name = "Elif Şahin",   Email = "elif.sahin@alumni.edu.tr" },
    ];
}
