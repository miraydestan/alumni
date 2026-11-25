using Alumni.Data.Factories;
using Alumni.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Alumni.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AlumniDbContext>();
        var config = scope.ServiceProvider.GetService<IConfiguration>();

        await SeedUsersAsync(db, config);
        await SeedAnnouncementsAsync(db);
        await SeedJobPostingsAsync(db);
    }

    // ── Users ────────────────────────────────────────────────────────────────
    private static async Task SeedUsersAsync(AlumniDbContext db, IConfiguration? config)
    {
        if (!await db.Users.AnyAsync())
        {
            var users = UserFactory.GetSeedUsers();
            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync();
        }

        // Configuration-based Admin account promotion / verification
        var adminEmail = config?["Admin:Email"]
                         ?? Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                         ?? "admin@alumni.edu.tr";

        if (!string.IsNullOrWhiteSpace(adminEmail))
        {
            var normalized = adminEmail.Trim().ToLowerInvariant();
            var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);

            if (adminUser != null)
            {
                // Promote existing user to Administrator without creating duplicates
                if (adminUser.Role != "Administrator")
                {
                    adminUser.Role = "Administrator";
                    await db.SaveChangesAsync();
                }
            }
            else if (string.Equals(normalized, "admin@alumni.edu.tr", StringComparison.OrdinalIgnoreCase))
            {
                // If default admin does not exist yet, add it from seed
                var defaultAdmin = UserFactory.GetSeedUsers().FirstOrDefault(u => u.Role == "Administrator");
                if (defaultAdmin != null)
                {
                    await db.Users.AddAsync(defaultAdmin);
                    await db.SaveChangesAsync();
                }
            }
        }
    }

    // ── Announcements ────────────────────────────────────────────────────────
    private static async Task SeedAnnouncementsAsync(AlumniDbContext db)
    {
        var seedAnnouncements = AnnouncementFactory.GetSeedAnnouncements();

        if (!await db.Announcements.AnyAsync())
        {
            await db.Announcements.AddRangeAsync(seedAnnouncements);
            await db.SaveChangesAsync();
            return;
        }

        // If announcements already exist, ensure bilingual fields are populated
        var existingAnnouncements = await db.Announcements.ToListAsync();
        var modified = false;

        foreach (var seed in seedAnnouncements)
        {
            var match = existingAnnouncements.FirstOrDefault(a => a.Title == seed.Title || a.Id == seed.Id);
            if (match != null && (string.IsNullOrEmpty(match.TitleEn) || string.IsNullOrEmpty(match.ContentEn)))
            {
                match.TitleEn = seed.TitleEn;
                match.ContentEn = seed.ContentEn;
                modified = true;
            }
        }

        if (modified)
        {
            await db.SaveChangesAsync();
        }
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
