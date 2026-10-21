using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Route("api/users")]
[Tags("Users")]
public class ApiUserController : ControllerBase
{
    private readonly AlumniDbContext _db;

    public ApiUserController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /api/users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users.ToListAsync();
        return Ok(users);
    }

    // GET: /api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // POST: /api/users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (user == null)
            return BadRequest();

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Created($"/api/users/{user.Id}", user);
    }

    // PUT: /api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        var existing = await _db.Users.FindAsync(id);
        if (existing == null)
            return NotFound();

        existing.Name  = updatedUser.Name;
        existing.Email = updatedUser.Email;
        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    // PATCH: /api/users/{id}  — only updates fields explicitly provided (non-null / non-empty)
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchUser(int id, [FromBody] User updatedUser)
    {
        var existing = await _db.Users.FindAsync(id);
        if (existing == null)
            return NotFound();

        if (!string.IsNullOrEmpty(updatedUser.Name))
            existing.Name = updatedUser.Name;

        if (!string.IsNullOrEmpty(updatedUser.Email))
            existing.Email = updatedUser.Email;

        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET: /api/users/{id}/jobs
    [HttpGet("{id}/jobs")]
    public async Task<IActionResult> GetJobsByUserId(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        var jobs = await _db.JobPostings
            .Where(j => j.UserId == id)
            .ToListAsync();

        return Ok(jobs);
    }

    // POST: /api/users/{id}/jobs
    [HttpPost("{id}/jobs")]
    public async Task<IActionResult> CreateJobForUser(int id, [FromBody] CreateJobPostingRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        if (request == null || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Company))
            return BadRequest();

        var job = new JobPosting
        {
            UserId = id,
            Title = request.Title.Trim(),
            Company = request.Company.Trim()
        };

        _db.JobPostings.Add(job);
        await _db.SaveChangesAsync();

        return Created($"/api/users/{id}/jobs/{job.Id}", job);
    }

    // GET: /api/users/{id}/applications
    [HttpGet("{id}/applications")]
    public async Task<IActionResult> GetApplicationsByUserId(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        var applications = await _db.Applications
            .Where(a => a.UserId == id)
            .ToListAsync();

        return Ok(applications);
    }
}
