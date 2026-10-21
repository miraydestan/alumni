namespace Alumni.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
