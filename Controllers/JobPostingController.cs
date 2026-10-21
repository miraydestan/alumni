using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class JobPostingController : Controller
{
    private readonly AlumniDbContext _db;

    public JobPostingController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /JobPosting or /jobs
    [HttpGet]
    [Route("JobPosting")]
    [Route("JobPosting/Index")]
    [Route("jobs")]
    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "JobPostings";
        var jobs = await _db.JobPostings
            .Include(j => j.User)
            .Include(j => j.Applications)
            .OrderByDescending(j => j.Id)
            .ToListAsync();

        return View(jobs);
    }

    // GET: /JobPosting/Details/{id}
    [HttpGet]
    [Route("JobPosting/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        ViewData["ActivePage"] = "JobPostings";
        var job = await _db.JobPostings
            .Include(j => j.User)
            .Include(j => j.Applications)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            return NotFound();

        return View(job);
    }

    // GET: /JobPosting/Applications/{id}
    [HttpGet]
    [Route("JobPosting/Applications/{id}")]
    public async Task<IActionResult> Applications(int id)
    {
        ViewData["ActivePage"] = "JobPostings";
        var job = await _db.JobPostings
            .Include(j => j.User)
            .Include(j => j.Applications)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
            return NotFound();

        return View(job);
    }

    // POST: /JobPosting/Apply/{id}
    [HttpPost]
    [Route("JobPosting/Apply/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(int id)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job == null)
            return NotFound();

        // Oturum açılmamışsa başka kullanıcı adına başvuruya izin verilmez
        if (User?.Identity?.IsAuthenticated != true)
        {
            TempData["ErrorMessage"] = "Başvuru yapabilmek için sisteme giriş yapmış olmanız gerekmektedir. Başka bir kullanıcı adına başvuru yapılamaz.";
            return RedirectToAction(nameof(Applications), new { id });
        }

        // Kullanıcı kimliği form parametresinden değil, güvenilir oturum claim'inden alınır
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
        {
            TempData["ErrorMessage"] = "Geçersiz kullanıcı oturumu. Lütfen tekrar giriş yapınız.";
            return RedirectToAction(nameof(Applications), new { id });
        }

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Oturum açmış kullanıcı sistemde bulunamadı.";
            return RedirectToAction(nameof(Applications), new { id });
        }

        var alreadyApplied = await _db.Applications
            .AnyAsync(a => a.UserId == userId && a.JobPostingId == id);

        if (alreadyApplied)
        {
            TempData["ErrorMessage"] = "Bu ilana daha önce başvurdunuz. Aynı ilana mükerrer başvuru yapılamaz.";
            return RedirectToAction(nameof(Applications), new { id });
        }

        var application = new Application
        {
            JobPostingId = id,
            UserId = userId,
            Status = "applied"
        };

        _db.Applications.Add(application);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Sayın {user.Name}, başvurunuz başarıyla alındı!";
        return RedirectToAction(nameof(Applications), new { id });
    }
}
