using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

        var currentUserId = GetCurrentUserId();
        var isOwnerOrAdmin = User.IsInRole("Administrator") || (currentUserId > 0 && job.UserId == currentUserId);

        ViewBag.TotalApplicantCount = job.Applications.Count;
        ViewBag.CanViewAllApplicants = isOwnerOrAdmin;

        if (!isOwnerOrAdmin)
        {
            // Privacy guard: Prevent users/other alumni from inspecting other candidates' private data
            job.Applications = currentUserId > 0
                ? job.Applications.Where(a => a.UserId == currentUserId).ToList()
                : new List<Application>();
        }

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

    // GET: /JobPosting/MyApplications (Student Only)
    [Authorize(Roles = "Student")]
    [HttpGet]
    [Route("JobPosting/MyApplications")]
    public async Task<IActionResult> MyApplications()
    {
        ViewData["Title"] = "My Applications";
        ViewData["ActivePage"] = "MyApplications";

        var userId = GetCurrentUserId();
        if (userId <= 0)
            return RedirectToAction("Login", "Account");

        var applications = await _db.Applications
            .Include(a => a.JobPosting)
                .ThenInclude(j => j!.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Id)
            .ToListAsync();

        return View(applications);
    }

    // GET: /JobPosting/MyJobPostings (Alumni Only)
    [Authorize(Roles = "Alumni")]
    [HttpGet]
    [Route("JobPosting/MyJobPostings")]
    public async Task<IActionResult> MyJobPostings()
    {
        ViewData["Title"] = "My Job Postings";
        ViewData["ActivePage"] = "MyJobPostings";

        var userId = GetCurrentUserId();
        if (userId <= 0)
            return RedirectToAction("Login", "Account");

        var myJobs = await _db.JobPostings
            .Include(j => j.Applications)
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.Id)
            .ToListAsync();

        return View(myJobs);
    }

    // POST: /JobPosting/Create (Alumni Only)
    [Authorize(Roles = "Alumni")]
    [HttpPost]
    [Route("JobPosting/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string company)
    {
        var userId = GetCurrentUserId();
        if (userId <= 0)
            return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(company))
        {
            TempData["ErrorMessage"] = "Job title and company name are both required.";
            return RedirectToAction(nameof(MyJobPostings));
        }

        var job = new JobPosting
        {
            UserId = userId,
            Title = title.Trim(),
            Company = company.Trim()
        };

        _db.JobPostings.Add(job);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Job posting '{job.Title}' at '{job.Company}' has been published.";
        return RedirectToAction(nameof(MyJobPostings));
    }

    // GET: /JobPosting/ApplicationsReceived (Alumni Only)
    [Authorize(Roles = "Alumni")]
    [HttpGet]
    [Route("JobPosting/ApplicationsReceived")]
    public async Task<IActionResult> ApplicationsReceived()
    {
        ViewData["Title"] = "Applications Received";
        ViewData["ActivePage"] = "ApplicationsReceived";

        var userId = GetCurrentUserId();
        if (userId <= 0)
            return RedirectToAction("Login", "Account");

        var received = await _db.Applications
            .Include(a => a.JobPosting)
            .Include(a => a.User)
            .Where(a => a.JobPosting != null && a.JobPosting.UserId == userId)
            .OrderByDescending(a => a.Id)
            .ToListAsync();

        return View(received);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
