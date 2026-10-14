using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AnnouncementAdminController : Controller
{
    private readonly AlumniDbContext _db;

    public AnnouncementAdminController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /AnnouncementAdmin
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        var announcements = await _db.Announcements
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
        return View(announcements);
    }

    // GET: /AnnouncementAdmin/Create
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        return View(new Announcement());
    }

    // POST: /AnnouncementAdmin/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Announcement announcement)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";

        if (string.IsNullOrWhiteSpace(announcement.Title) || string.IsNullOrWhiteSpace(announcement.Content))
        {
            ModelState.AddModelError(string.Empty, "Title and Content are required.");
            return View(announcement);
        }

        announcement.CreatedDate = DateTime.UtcNow;
        _db.Announcements.Add(announcement);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /AnnouncementAdmin/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        var announcement = await _db.Announcements.FindAsync(id);
        if (announcement == null)
            return NotFound();

        return View(announcement);
    }

    // POST: /AnnouncementAdmin/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Announcement updatedAnnouncement)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";

        var existing = await _db.Announcements.FindAsync(id);
        if (existing == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(updatedAnnouncement.Title) || string.IsNullOrWhiteSpace(updatedAnnouncement.Content))
        {
            ModelState.AddModelError(string.Empty, "Title and Content are required.");
            return View(updatedAnnouncement);
        }

        existing.Title   = updatedAnnouncement.Title;
        existing.Content = updatedAnnouncement.Content;
        // CreatedDate is intentionally not updated on edit
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /AnnouncementAdmin/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        var announcement = await _db.Announcements.FindAsync(id);
        if (announcement == null)
            return NotFound();

        return View(announcement);
    }

    // POST: /AnnouncementAdmin/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var announcement = await _db.Announcements.FindAsync(id);
        if (announcement != null)
        {
            _db.Announcements.Remove(announcement);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
