using Alumni.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace Alumni.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AlumniDbContext>();

        await SeedUsersAsync(db);
        await SeedAnnouncementsAsync(db);
    }

    // ── Users ────────────────────────────────────────────────────────────────
    private static async Task SeedUsersAsync(AlumniDbContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        var users = UserFactory.GetSeedUsers();
        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();
    }

    // ── Announcements ────────────────────────────────────────────────────────
    private static async Task SeedAnnouncementsAsync(AlumniDbContext db)
    {
        if (await db.Announcements.AnyAsync())
            return;

        var announcements = AnnouncementFactory.GetSeedAnnouncements();
        await db.Announcements.AddRangeAsync(announcements);
        await db.SaveChangesAsync();
    }
}
