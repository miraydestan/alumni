using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Authorize]
[Route("api/jobs")]
[Tags("Jobs")]
public class ApiJobController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public ApiJobController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /api/jobs/{id}/applications (Administrator or Alumni owner of the job posting)
    [Authorize(Roles = "Alumni,Administrator")]
    [HttpGet("{id:int}/applications")]
    public async Task<IActionResult> GetApplicationsByJobId(int id)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job == null)
            return NotFound();

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");
        if (!isAdmin && (currentUserId <= 0 || job.UserId != currentUserId))
            return Forbid();

        var applications = await _db.Applications
            .Where(a => a.JobPostingId == id)
            .ToListAsync();

        return Ok(applications);
    }

    // GET: /api/jobs/{id}/applications/{applicationId} (Administrator, Alumni owner of the job, or Student applicant)
    [HttpGet("{id:int}/applications/{applicationId:int}")]
    public async Task<IActionResult> GetApplicationById(int id, int applicationId)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job == null)
            return NotFound();

        var application = await _db.Applications
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostingId == id);

        if (application == null)
            return NotFound();

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");
        var isJobOwner = User.IsInRole("Alumni") && currentUserId > 0 && job.UserId == currentUserId;
        var isApplicant = User.IsInRole("Student") && currentUserId > 0 && application.UserId == currentUserId;

        if (!isAdmin && !isJobOwner && !isApplicant)
            return Forbid();

        return Ok(application);
    }

    // POST: /api/jobs/{id}/applications (Student only; own identity from claims; cannot apply to own job)
    [Authorize(Roles = "Student")]
    [HttpPost("{id:int}/applications")]
    public async Task<IActionResult> CreateApplicationForJob(int id, [FromBody] CreateApplicationRequest? request)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job == null)
            return NotFound();

        var authenticatedUserId = GetCurrentUserId();
        if (authenticatedUserId <= 0)
        {
            return Unauthorized(new { message = "Invalid user identity claim in session." });
        }

        // Reject spoofing attempts if client supplies a mismatched user_id
        var applicantId = request?.EffectiveUserId ?? 0;
        if (applicantId > 0 && applicantId != authenticatedUserId)
        {
            return Forbid();
        }

        // Prevent self-application
        if (job.UserId == authenticatedUserId)
        {
            return BadRequest(new { message = "You cannot apply to your own job posting." });
        }

        var user = await _db.Users.FindAsync(authenticatedUserId);
        if (user == null)
            return NotFound(new { message = "Authenticated user does not exist in the database." });

        var alreadyApplied = await _db.Applications
            .AnyAsync(a => a.UserId == authenticatedUserId && a.JobPostingId == id);

        if (alreadyApplied)
            return Conflict(new { message = "User has already applied for this job posting." });

        var application = new Application
        {
            JobPostingId = id,
            UserId = authenticatedUserId,
            Status = "applied"
        };

        _db.Applications.Add(application);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "User has already applied for this job posting." });
        }

        return Created($"/api/jobs/{id}/applications/{application.Id}", application);
    }

    private int GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : 0;
    }
}
