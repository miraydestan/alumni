namespace Alumni.Models;

/// <summary>
/// Simple in-memory data store for Announcement entities.
/// Shared across AnnouncementController and AnnouncementAdminController.
/// </summary>
public static class AnnouncementStore
{
    public static List<Announcement> Announcements { get; } = new()
    {
        new Announcement
        {
            Id = 1,
            Title = "Annual Alumni Homecoming 2026",
            Content = "Join us on campus for our annual alumni reunion! Reconnect with classmates, meet faculty members, and celebrate the accomplishments of our community.",
            CreatedDate = DateTime.Now.AddDays(-7)
        },
        new Announcement
        {
            Id = 2,
            Title = "Alumni Mentorship Program Launched",
            Content = "Applications are now open for our winter mentorship program connecting experienced alumni with senior undergraduate students for guidance and networking.",
            CreatedDate = DateTime.Now.AddDays(-3)
        },
        new Announcement
        {
            Id = 3,
            Title = "Campus Tech & Innovation Hub Grand Opening",
            Content = "Thanks to the generous support of our alumni community, our new Technology and Innovation Hub is opening this Friday. All alumni are cordially invited.",
            CreatedDate = DateTime.Now.AddDays(-1)
        }
    };
}
