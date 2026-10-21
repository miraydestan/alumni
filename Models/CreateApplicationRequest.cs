using System.Text.Json.Serialization;

namespace Alumni.Models;

public class CreateApplicationRequest
{
    [JsonPropertyName("user_id")]
    public int? UserIdSnakeCase { get; set; }

    public int? UserId { get; set; }

    [JsonIgnore]
    public int EffectiveUserId => UserId ?? UserIdSnakeCase ?? 0;

    [JsonPropertyName("job_posting_id")]
    public int? JobPostingIdSnakeCase { get; set; }

    public int? JobPostingId { get; set; }

    [JsonIgnore]
    public int EffectiveJobPostingId => JobPostingId ?? JobPostingIdSnakeCase ?? 0;

    public string? Status { get; set; }
}
