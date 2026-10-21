using System.Text.Json.Serialization;

namespace Alumni.Models;

public class JobPosting
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;

    [JsonIgnore]
    public User? User { get; set; }
}
