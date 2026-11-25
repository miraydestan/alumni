using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
[Tags("Users")]
public class ApiUserController : ControllerBase
{
    private readonly AlumniDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    private static readonly HashSet<string> ValidRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Administrator",
        "Alumni",
        "Student"
    };

    public ApiUserController(
        AlumniDbContext db,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    // GET: /api/users (Administrator only)
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users.OrderBy(u => u.Id).ToListAsync();
        return Ok(users);
    }

    // GET: /api/users/{id} (Administrator or own account)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");

        if (!isAdmin && (currentUserId <= 0 || currentUserId != id))
            return Forbid();

        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // POST: /api/users (Administrator only)
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (request == null)
            return BadRequest();

        var roleInput = string.IsNullOrWhiteSpace(request.Role) ? "Alumni" : request.Role.Trim();
        if (!ValidRoles.Contains(roleInput))
            return BadRequest(new { message = "Invalid role. Permitted roles are Administrator, Alumni, or Student." });

        var trimmedName = request.Name.Trim();
        var trimmedEmail = request.Email.Trim();
        var normalizedEmail = trimmedEmail.ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(trimmedName) || string.IsNullOrWhiteSpace(trimmedEmail))
            return BadRequest(new { message = "Name and Email are required." });

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return BadRequest(new { message = "Password is required and must be at least 6 non-whitespace characters." });

        var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
            return Conflict(new { message = "A user with this email address already exists." });

        var newUser = new User
        {
            Name = trimmedName,
            Email = trimmedEmail,
            Role = ToCanonicalRole(roleInput)
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

        _db.Users.Add(newUser);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "A user with this email address already exists." });
        }

        return Created($"/api/users/{newUser.Id}", newUser);
    }

    // PUT: /api/users/{id} (Administrator or own account; role change restricted to Administrator)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        if (request == null)
            return BadRequest();

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");

        if (!isAdmin && (currentUserId <= 0 || currentUserId != id))
            return Forbid();

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Name and Email are required." });

        var existing = await _db.Users.FindAsync(id);
        if (existing == null)
            return NotFound();

        if (!isAdmin && !string.IsNullOrWhiteSpace(request.Role) &&
            !string.Equals(request.Role.Trim(), existing.Role, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        var trimmedName = request.Name.Trim();
        var trimmedEmail = request.Email.Trim();
        var normalizedEmail = trimmedEmail.ToLowerInvariant();

        if (!isAdmin && IsConfiguredAdminEmail(trimmedEmail))
            return Forbid();

        var emailInUse = await _db.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == normalizedEmail);
        if (emailInUse)
            return Conflict(new { message = "Another user with this email address already exists." });

        if (isAdmin && !string.IsNullOrWhiteSpace(request.Role))
        {
            var roleInput = request.Role.Trim();
            if (!ValidRoles.Contains(roleInput))
                return BadRequest(new { message = "Invalid role. Permitted roles are Administrator, Alumni, or Student." });

            var canonicalRole = ToCanonicalRole(roleInput);
            if (currentUserId == id && canonicalRole != "Administrator")
                return BadRequest(new { message = "You cannot revoke your own Administrator privileges." });

            existing.Role = canonicalRole;
        }

        if (request.Password != null)
        {
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 non-whitespace characters." });

            if (!isAdmin)
            {
                if (string.IsNullOrEmpty(request.CurrentPassword))
                    return BadRequest(new { message = "CurrentPassword is required to change your password." });

                if (string.IsNullOrEmpty(existing.PasswordHash) ||
                    _passwordHasher.VerifyHashedPassword(existing, existing.PasswordHash, request.CurrentPassword) == PasswordVerificationResult.Failed)
                {
                    return BadRequest(new { message = "Current password is incorrect." });
                }
            }

            existing.PasswordHash = _passwordHasher.HashPassword(existing, request.Password);
        }

        existing.Name = trimmedName;
        existing.Email = trimmedEmail;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "Another user with this email address already exists." });
        }

        if (currentUserId == id)
        {
            await RefreshCurrentUserCookieAsync(existing);
        }

        return Ok(existing);
    }

    // PATCH: /api/users/{id} (Administrator or own account; role change restricted to Administrator)
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PatchUser(int id, [FromBody] UpdateUserRequest request)
    {
        if (request == null)
            return BadRequest();

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");

        if (!isAdmin && (currentUserId <= 0 || currentUserId != id))
            return Forbid();

        var existing = await _db.Users.FindAsync(id);
        if (existing == null)
            return NotFound();

        if (!isAdmin && !string.IsNullOrWhiteSpace(request.Role) &&
            !string.Equals(request.Role.Trim(), existing.Role, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            existing.Name = request.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var trimmedEmail = request.Email.Trim();
            var normalizedEmail = trimmedEmail.ToLowerInvariant();

            if (!isAdmin && IsConfiguredAdminEmail(trimmedEmail))
                return Forbid();

            var emailInUse = await _db.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == normalizedEmail);
            if (emailInUse)
                return Conflict(new { message = "Another user with this email address already exists." });

            existing.Email = trimmedEmail;
        }

        if (isAdmin && !string.IsNullOrWhiteSpace(request.Role))
        {
            var roleInput = request.Role.Trim();
            if (!ValidRoles.Contains(roleInput))
                return BadRequest(new { message = "Invalid role. Permitted roles are Administrator, Alumni, or Student." });

            var canonicalRole = ToCanonicalRole(roleInput);
            if (currentUserId == id && canonicalRole != "Administrator")
                return BadRequest(new { message = "You cannot revoke your own Administrator privileges." });

            existing.Role = canonicalRole;
        }

        if (request.Password != null)
        {
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 non-whitespace characters." });

            if (!isAdmin)
            {
                if (string.IsNullOrEmpty(request.CurrentPassword))
                    return BadRequest(new { message = "CurrentPassword is required to change your password." });

                if (string.IsNullOrEmpty(existing.PasswordHash) ||
                    _passwordHasher.VerifyHashedPassword(existing, existing.PasswordHash, request.CurrentPassword) == PasswordVerificationResult.Failed)
                {
                    return BadRequest(new { message = "Current password is incorrect." });
                }
            }

            existing.PasswordHash = _passwordHasher.HashPassword(existing, request.Password);
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "Another user with this email address already exists." });
        }

        if (currentUserId == id)
        {
            await RefreshCurrentUserCookieAsync(existing);
        }

        return Ok(existing);
    }

    // DELETE: /api/users/{id} (Administrator only)
    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == id)
            return BadRequest(new { message = "You cannot delete your own Administrator account." });

        var user = await _db.Users
            .Include(u => u.JobPostings)
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        if (user.JobPostings.Count > 0 || user.Applications.Count > 0)
        {
            return Conflict(new
            {
                message = "Cannot delete user with existing job postings or applications."
            });
        }

        _db.Users.Remove(user);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Cannot delete user because related records still reference this account."
            });
        }
        return NoContent();
    }

    // GET: /api/users/{id}/jobs (Administrator or Alumni owner)
    [Authorize(Roles = "Alumni,Administrator")]
    [HttpGet("{id:int}/jobs")]
    public async Task<IActionResult> GetJobsByUserId(int id)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");

        if (!isAdmin && (currentUserId <= 0 || currentUserId != id))
            return Forbid();

        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        var jobs = await _db.JobPostings
            .Where(j => j.UserId == id)
            .ToListAsync();

        return Ok(jobs);
    }

    // POST: /api/users/{id}/jobs (Administrator or Alumni creating under own ID only)
    [Authorize(Roles = "Alumni,Administrator")]
    [HttpPost("{id:int}/jobs")]
    public async Task<IActionResult> CreateJobForUser(int id, [FromBody] CreateJobPostingRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");

        if (!isAdmin && (currentUserId <= 0 || currentUserId != id))
            return Forbid();

        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        if (!string.Equals(user.Role, "Alumni", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Job postings can only be created for users with the Alumni role." });

        if (request == null || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Company))
            return BadRequest(new { message = "Title and Company are required." });

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

    // GET: /api/users/{id}/applications (Administrator or Student owner only)
    [Authorize(Roles = "Student,Administrator")]
    [HttpGet("{id:int}/applications")]
    public async Task<IActionResult> GetApplicationsByUserId(int id)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrator");

        if (!isAdmin && (currentUserId <= 0 || currentUserId != id))
            return Forbid();

        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        var applications = await _db.Applications
            .Where(a => a.UserId == id)
            .ToListAsync();

        return Ok(applications);
    }

    private int GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : 0;
    }

    private static string ToCanonicalRole(string role)
    {
        if (role.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
            return "Administrator";
        if (role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            return "Student";
        return "Alumni";
    }

    private bool IsConfiguredAdminEmail(string email)
    {
        var envAdminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        var configAdminEmail = _configuration["Admin:Email"];
        var adminEmail = !string.IsNullOrWhiteSpace(envAdminEmail)
            ? envAdminEmail.Trim()
            : !string.IsNullOrWhiteSpace(configAdminEmail)
                ? configAdminEmail.Trim()
                : "admin@alumni.edu.tr";

        return string.Equals(email.Trim(), adminEmail, StringComparison.OrdinalIgnoreCase);
    }

    private async Task RefreshCurrentUserCookieAsync(User user)
    {
        var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var isPersistent = authResult.Properties?.IsPersistent ?? false;
        var expiresUtc = authResult.Properties?.ExpiresUtc;

        var updatedClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var updatedIdentity = new ClaimsIdentity(updatedClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        var updatedPrincipal = new ClaimsPrincipal(updatedIdentity);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = isPersistent ? (expiresUtc ?? DateTimeOffset.UtcNow.AddDays(7)) : null
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            updatedPrincipal,
            authProperties);
    }
}
