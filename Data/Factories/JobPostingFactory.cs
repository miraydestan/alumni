using Alumni.Models;

namespace Alumni.Data.Factories;

public static class JobPostingFactory
{
    public static IReadOnlyList<JobPosting> GetSeedJobPostings(IReadOnlyList<User> users)
    {
        if (users == null || users.Count == 0)
            return [];

        // Assign seeded job postings to Alumni accounts rather than Administrator/Student accounts
        var alumniUsers = users
            .Where(u => string.Equals(u.Role, "Alumni", StringComparison.OrdinalIgnoreCase))
            .ToList();
        var owners = alumniUsers.Count > 0 ? alumniUsers : users;

        var u1 = owners[0].Id;
        var u2 = owners.Count > 1 ? owners[1].Id : u1;
        var u3 = owners.Count > 2 ? owners[2].Id : u1;

        return
        [
            new JobPosting
            {
                UserId  = u1,
                Title   = "Software Developer",
                Company = "TechNova Solutions"
            },
            new JobPosting
            {
                UserId  = u2,
                Title   = "Data Analyst",
                Company = "InnoAnalytics"
            },
            new JobPosting
            {
                UserId  = u3,
                Title   = "Business Analyst",
                Company = "Global Ventures Consulting"
            },
            new JobPosting
            {
                UserId  = u1,
                Title   = "Junior Frontend Developer",
                Company = "PixelCraft Studios"
            }
        ];
    }
}
