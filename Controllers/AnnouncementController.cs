using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;

namespace Alumni.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AnnouncementController : Controller
{
    private readonly AlumniDbContext _db;

    public AnnouncementController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /Announcement
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "Announcements";
        var announcements = await _db.Announcements
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
        return View(announcements);
    }

    // GET: /Announcement/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        ViewData["ActivePage"] = "Announcements";
        var announcement = await _db.Announcements.FindAsync(id);
        if (announcement == null)
            return NotFound();

        return View(announcement);
    }
}
