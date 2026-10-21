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

        var userId = request.EffectiveUserId;
        var jobPostingId = request.EffectiveJobPostingId;

        if (userId <= 0 || jobPostingId <= 0)
            return BadRequest();

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return NotFound();

        var job = await _db.JobPostings.FindAsync(jobPostingId);
        if (job == null)
            return NotFound();

        var alreadyApplied = await _db.Applications
            .AnyAsync(a => a.UserId == userId && a.JobPostingId == jobPostingId);

        if (alreadyApplied)
            return Conflict(new { message = "User has already applied for this job posting." });

        var application = new Application
        {
            UserId = userId,
            JobPostingId = jobPostingId,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "applied" : request.Status.Trim()
        };

        _db.Applications.Add(application);
        await _db.SaveChangesAsync();

        return Created($"/api/applications/{application.Id}", application);
    }
}
