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
            CreatedDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
        },
        new Announcement
        {
            Title       = "Kariyer Günleri",
            Content     = "Yıllık kariyer günlerimiz 20-21 Mart tarihlerinde düzenlenecektir. Tüm mezunlar davetlidir.",
            CreatedDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
        },
        new Announcement
        {
            Title       = "Alumni Bülten — Nisan 2026",
            Content     = "Nisan ayı bültenimiz yayımlandı. Mezun haberlerini ve etkinlik duyurularını okuyun.",
            CreatedDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
        },
    ];
}