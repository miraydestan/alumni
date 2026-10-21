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
        await SeedJobPostingsAsync(db);
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

    // ── Job Postings ─────────────────────────────────────────────────────────
    private static async Task SeedJobPostingsAsync(AlumniDbContext db)
    {
        var users = await db.Users.OrderBy(u => u.Id).ToListAsync();
        if (users.Count == 0)
            return;

        var seedJobs = JobPostingFactory.GetSeedJobPostings(users);
        var anyAdded = false;

        foreach (var job in seedJobs)
        {
            var exists = await db.JobPostings.AnyAsync(j =>
                j.Title == job.Title &&
                j.Company == job.Company &&
                j.UserId == job.UserId);

            if (!exists)
            {
                await db.JobPostings.AddAsync(job);
                anyAdded = true;
            }
        }

        if (anyAdded)
        {
            await db.SaveChangesAsync();
        }
    }
}
