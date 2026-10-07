using Microsoft.AspNetCore.Mvc;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AnnouncementAdminController : Controller
{
    // GET: /AnnouncementAdmin
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        var announcements = AnnouncementStore.Announcements.OrderByDescending(a => a.CreatedDate).ToList();
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
    public IActionResult Create(Announcement announcement)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";

        if (string.IsNullOrWhiteSpace(announcement.Title) || string.IsNullOrWhiteSpace(announcement.Content))
        {
            ModelState.AddModelError(string.Empty, "Title and Content are required.");
            return View(announcement);
        }

        if (announcement.Id == 0)
        {
            announcement.Id = AnnouncementStore.Announcements.Count > 0
                ? AnnouncementStore.Announcements.Max(a => a.Id) + 1
                : 1;
        }

        announcement.CreatedDate = DateTime.Now;
        AnnouncementStore.Announcements.Add(announcement);

        return RedirectToAction(nameof(Index));
    }

    // GET: /AnnouncementAdmin/Edit/{id}
    [HttpGet]
    public IActionResult Edit(int id)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        var announcement = AnnouncementStore.Announcements.FirstOrDefault(a => a.Id == id);
        if (announcement == null)
        {
            return NotFound();
        }

        return View(announcement);
    }

    // POST: /AnnouncementAdmin/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Announcement updatedAnnouncement)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        var existing = AnnouncementStore.Announcements.FirstOrDefault(a => a.Id == id);
        if (existing == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updatedAnnouncement.Title) || string.IsNullOrWhiteSpace(updatedAnnouncement.Content))
        {
            ModelState.AddModelError(string.Empty, "Title and Content are required.");
            return View(updatedAnnouncement);
        }

        existing.Title = updatedAnnouncement.Title;
        existing.Content = updatedAnnouncement.Content;

        return RedirectToAction(nameof(Index));
    }

    // GET: /AnnouncementAdmin/Delete/{id}
    [HttpGet]
    public IActionResult Delete(int id)
    {
        ViewData["ActivePage"] = "AnnouncementAdmin";
        var announcement = AnnouncementStore.Announcements.FirstOrDefault(a => a.Id == id);
        if (announcement == null)
        {
            return NotFound();
        }

        return View(announcement);
    }

    // POST: /AnnouncementAdmin/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var announcement = AnnouncementStore.Announcements.FirstOrDefault(a => a.Id == id);
        if (announcement != null)
        {
            AnnouncementStore.Announcements.Remove(announcement);
        }

        return RedirectToAction(nameof(Index));
    }
}
