using Alumni.Models;

namespace Alumni.Data.Factories;

public static class AnnouncementFactory
{
    public static IReadOnlyList<Announcement> GetSeedAnnouncements() =>
    [
        new Announcement
        {
            Title       = "2026 Mezuniyet Töreni",
            Content     = "2026 yılı mezuniyet törenimiz 15 Haziran’da kampüs amfisinde gerçekleşecektir.",
            TitleEn     = "2026 Graduation Ceremony",
            ContentEn   = "Our 2026 graduation ceremony will take place on June 15 at the campus amphitheater.",
            CreatedDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
        },
        new Announcement
        {
            Title       = "Kariyer Günleri",
            Content     = "Yıllık kariyer günlerimiz 20-21 Mart tarihlerinde düzenlenecektir. Tüm mezunlar davetlidir.",
            TitleEn     = "Career Days",
            ContentEn   = "Our annual career days will be held on March 20–21. All alumni are cordially invited.",
            CreatedDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
        },
        new Announcement
        {
            Title       = "Alumni Bülten — Nisan 2026",
            Content     = "Nisan ayı bültenimiz yayımlandı. Mezun haberlerini ve etkinlik duyurularını okuyun.",
            TitleEn     = "Alumni Newsletter — April 2026",
            ContentEn   = "Our April newsletter has been published. Read alumni stories, news, and upcoming events.",
            CreatedDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
        },
    ];
}
