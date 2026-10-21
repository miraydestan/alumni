using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Route("api/applications")]
[Tags("Applications")]
public class ApiApplicationController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public ApiApplicationController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /api/applications
    [HttpGet]
    public async Task<IActionResult> GetApplications()
    {
        var applications = await _db.Applications.ToListAsync();
        return Ok(applications);
    }

    // GET: /api/applications/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationById(int id)
    {
        var application = await _db.Applications.FindAsync(id);
        if (application == null)
            return NotFound();

        return Ok(application);
    }

    // POST: /api/applications
    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        if (request == null)
            return BadRequest();

        var jobPostingId = request.EffectiveJobPostingId;
        if (jobPostingId <= 0)
            return BadRequest(new { message = "A valid job_posting_id is required." });

        // Gerçek kullanıcı kimliği doğrulanamıyorsa başvuru oluşturulmasına izin verilmez (Fail-Closed)
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Unauthorized(new
            {
                message = "Authentication is required to create an application. Client-supplied user_id cannot be accepted as verified identity."
            });
        }

        // İleride kimlik doğrulama eklendiğinde kullanıcı kimliği ClaimTypes.NameIdentifier üzerinden alınır
        var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claimValue, out var authenticatedUserId) || authenticatedUserId <= 0)
        {
            return Unauthorized(new { message = "Invalid user identity claim in session." });
        }

        // İstemcinin gönderdiği user_id ile doğrulanmış kullanıcı kimliği eşleşmelidir
        var requestedUserId = request.EffectiveUserId;
        if (requestedUserId > 0 && requestedUserId != authenticatedUserId)
        {
            return Forbid();
        }

        var effectiveUserId = authenticatedUserId;

        var user = await _db.Users.FindAsync(effectiveUserId);
        if (user == null)
            return NotFound(new { message = "Authenticated user does not exist in the database." });

        var job = await _db.JobPostings.FindAsync(jobPostingId);
        if (job == null)
            return NotFound(new { message = "Job posting does not exist." });

        var alreadyApplied = await _db.Applications
            .AnyAsync(a => a.UserId == effectiveUserId && a.JobPostingId == jobPostingId);

        if (alreadyApplied)
            return Conflict(new { message = "User has already applied for this job posting." });

        var application = new Application
        {
            UserId = effectiveUserId,
            JobPostingId = jobPostingId,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "applied" : request.Status.Trim()
        };

        _db.Applications.Add(application);
        await _db.SaveChangesAsync();

        return Created($"/api/applications/{application.Id}", application);
    }
}
