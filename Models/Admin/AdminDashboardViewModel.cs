using Alumni.Models;

namespace Alumni.Models.Admin;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalAlumni { get; set; }
    public int TotalStudents { get; set; }
    public int TotalAdministrators { get; set; }
    public int TotalAnnouncements { get; set; }
    public int TotalJobPostings { get; set; }
    public int TotalApplications { get; set; }

    public IReadOnlyList<User> RecentUsers { get; set; } = [];
}
