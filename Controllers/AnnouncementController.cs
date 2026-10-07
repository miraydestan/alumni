using Microsoft.AspNetCore.Mvc;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class AnnouncementController : Controller
{
    // GET: /Announcement
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Announcements";
        var announcements = AnnouncementStore.Announcements.OrderByDescending(a => a.CreatedDate).ToList();
        return View(announcements);
    }

    // GET: /Announcement/Details/{id}
    [HttpGet]
    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Announcements";
        var announcement = AnnouncementStore.Announcements.FirstOrDefault(a => a.Id == id);
        if (announcement == null)
        {
            return NotFound();
        }

        return View(announcement);
    }
}
