using System.Text.Json.Serialization;

namespace Alumni.Models;

public class Application
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int JobPostingId { get; set; }
    public string Status { get; set; } = "applied";

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public JobPosting? JobPosting { get; set; }
}
