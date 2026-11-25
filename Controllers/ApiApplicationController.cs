using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Authorize]
[Route("api/applications")]
[Tags("Applications")]
public class ApiApplicationController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public ApiApplicationController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /api/applications (Role-scoped: Admin=all, Student=own applications, Alumni=applications on own jobs)
    [HttpGet]
    public async Task<IActionResult> GetApplications()
    {
        var currentUserId = GetCurrentUserId();

        if (User.IsInRole("Administrator"))
        {
            var allApplications = await _db.Applications.ToListAsync();
            return Ok(allApplications);
        }

        if (currentUserId <= 0)
            return Unauthorized(new { message = "Invalid user identity claim in session." });

        if (User.IsInRole("Student"))
        {
            var myApplications = await _db.Applications
                .Where(a => a.UserId == currentUserId)
                .ToListAsync();
            return Ok(myApplications);
        }

        if (User.IsInRole("Alumni"))
        {
            var receivedApplications = await _db.Applications
                .Where(a => a.JobPosting != null && a.JobPosting.UserId == currentUserId)
                .ToListAsync();
            return Ok(receivedApplications);
        }

        return Forbid();
    }

    // GET: /api/applications/{id} (Administrator, Student applicant, or Alumni owner of the target job)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetApplicationById(int id)
    {
        var application = await _db.Applications
            .Include(a => a.JobPosting)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
            return NotFound();

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");
        var isApplicant = User.IsInRole("Student") && currentUserId > 0 && application.UserId == currentUserId;
        var isJobOwner = User.IsInRole("Alumni") && currentUserId > 0 && application.JobPosting?.UserId == currentUserId;

        if (!isAdmin && !isApplicant && !isJobOwner)
            return Forbid();

        return Ok(application);
    }

    // POST: /api/applications (Student only; own identity from claims; cannot apply to own job)
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        if (request == null)
            return BadRequest();

        var jobPostingId = request.EffectiveJobPostingId;
        if (jobPostingId <= 0)
            return BadRequest(new { message = "A valid job_posting_id is required." });

        var authenticatedUserId = GetCurrentUserId();
        if (authenticatedUserId <= 0)
        {
            return Unauthorized(new { message = "Invalid user identity claim in session." });
        }

        // Reject spoofing attempts if client supplies a mismatched user_id
        var requestedUserId = request.EffectiveUserId;
        if (requestedUserId > 0 && requestedUserId != authenticatedUserId)
        {
            return Forbid();
        }

        var user = await _db.Users.FindAsync(authenticatedUserId);
        if (user == null)
            return NotFound(new { message = "Authenticated user does not exist in the database." });

        var job = await _db.JobPostings.FindAsync(jobPostingId);
        if (job == null)
            return NotFound(new { message = "Job posting does not exist." });

        if (job.UserId == authenticatedUserId)
            return BadRequest(new { message = "You cannot apply to your own job posting." });

        var alreadyApplied = await _db.Applications
            .AnyAsync(a => a.UserId == authenticatedUserId && a.JobPostingId == jobPostingId);

        if (alreadyApplied)
            return Conflict(new { message = "User has already applied for this job posting." });

        var application = new Application
        {
            UserId = authenticatedUserId,
            JobPostingId = jobPostingId,
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

        return Created($"/api/applications/{application.Id}", application);
    }

    private int GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : 0;
    }
}
