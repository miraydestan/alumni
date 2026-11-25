using Alumni.Models;

namespace Alumni.Models.Admin;

public class AdminUserDetailsViewModel
{
    public User User { get; set; } = null!;
    public IReadOnlyList<JobPosting> JobPostings { get; set; } = [];
    public IReadOnlyList<Application> Applications { get; set; } = [];
    public bool IsCurrentAdmin { get; set; }
}
