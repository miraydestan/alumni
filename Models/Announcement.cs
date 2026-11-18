namespace Alumni.Models;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string? ContentEn { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string GetTitle(string? lang = null) =>
        (lang == "tr" || string.IsNullOrWhiteSpace(TitleEn)) ? Title : TitleEn;

    public string GetContent(string? lang = null) =>
        (lang == "tr" || string.IsNullOrWhiteSpace(ContentEn)) ? Content : ContentEn;
}
