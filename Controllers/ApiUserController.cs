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
}
