using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Route("api/jobs")]
[Tags("Jobs")]
public class ApiJobController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public ApiJobController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /api/jobs/{id}/applications
    [HttpGet("{id}/applications")]
    public async Task<IActionResult> GetApplicationsByJobId(int id)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job == null)
            return NotFound();

        var applications = await _db.Applications
            .Where(a => a.JobPostingId == id)
            .ToListAsync();

        return Ok(applications);
    }

    // GET: /api/jobs/{id}/applications/{applicationId}
    [HttpGet("{id}/applications/{applicationId}")]
    public async Task<IActionResult> GetApplicationById(int id, int applicationId)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job == null)
            return NotFound();

        var application = await _db.Applications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostingId == id);

        if (application == null)
            return NotFound();

        return Ok(application);
    }

    // POST: /api/jobs/{id}/applications
    [HttpPost("{id}/applications")]
    public async Task<IActionResult> CreateApplicationForJob(int id, [FromBody] CreateApplicationRequest request)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job == null)
            return NotFound();

        if (request == null)
            return BadRequest();

        // Gerçek kullanıcı kimliği doğrulanamıyorsa başvuru oluşturulmasına izin verilmez (Fail-Closed)
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Unauthorized(new
            {
                message = "Authentication is required to apply for a job posting. Client-supplied user_id cannot be accepted as verified identity."
            });
        }

        // İleride kimlik doğrulama eklendiğinde kullanıcı kimliği ClaimTypes.NameIdentifier üzerinden alınır
        var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claimValue, out var authenticatedUserId) || authenticatedUserId <= 0)
        {
            return Unauthorized(new { message = "Invalid user identity claim in session." });
        }

        // İstemcinin gönderdiği user_id ile doğrulanmış kullanıcı kimliği eşleşmelidir
        var applicantId = request.EffectiveUserId;
        if (applicantId > 0 && applicantId != authenticatedUserId)
        {
            return Forbid();
        }

        var effectiveUserId = authenticatedUserId;

        var user = await _db.Users.FindAsync(effectiveUserId);
        if (user == null)
            return NotFound(new { message = "Authenticated user does not exist in the database." });

        var alreadyApplied = await _db.Applications
            .AnyAsync(a => a.UserId == effectiveUserId && a.JobPostingId == id);

        if (alreadyApplied)
            return Conflict(new { message = "User has already applied for this job posting." });

        var application = new Application
        {
            JobPostingId = id,
            UserId = effectiveUserId,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "applied" : request.Status.Trim()
        };

        _db.Applications.Add(application);
        await _db.SaveChangesAsync();

        return Created($"/api/jobs/{id}/applications/{application.Id}", application);
    }
}
