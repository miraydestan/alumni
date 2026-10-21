using Alumni.Models;

namespace Alumni.Data.Factories;

public static class JobPostingFactory
{
    public static IReadOnlyList<JobPosting> GetSeedJobPostings(IReadOnlyList<User> users)
    {
        if (users == null || users.Count == 0)
            return [];

        var u1 = users[0].Id;
        var u2 = users.Count > 1 ? users[1].Id : u1;
        var u3 = users.Count > 2 ? users[2].Id : u1;

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
