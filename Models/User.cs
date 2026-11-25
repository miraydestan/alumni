using System.Text.Json.Serialization;

namespace Alumni.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Alumni";

    [JsonIgnore]
    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();

    [JsonIgnore]
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
