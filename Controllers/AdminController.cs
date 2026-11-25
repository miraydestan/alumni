using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;
using Alumni.Models.Admin;

namespace Alumni.Controllers;

[Authorize(Roles = "Administrator")]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly AlumniDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    private static readonly HashSet<string> ValidRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Administrator",
        "Alumni",
        "Student"
    };

    public AdminController(AlumniDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    // ── DASHBOARD ────────────────────────────────────────────────────────────
    // GET: /Admin or /Admin/Index
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        ViewData["ActiveAdminTab"] = "Dashboard";
        ViewData["ActivePage"] = "AdminDashboard";

        var totalUsers = await _db.Users.CountAsync();
        var totalAlumni = await _db.Users.CountAsync(u => u.Role == "Alumni");
        var totalStudents = await _db.Users.CountAsync(u => u.Role == "Student");
        var totalAdmins = await _db.Users.CountAsync(u => u.Role == "Administrator");
        var totalAnnouncements = await _db.Announcements.CountAsync();
        var totalJobPostings = await _db.JobPostings.CountAsync();
        var totalApplications = await _db.Applications.CountAsync();

        var recentUsers = await _db.Users
            .OrderByDescending(u => u.Id)
            .Take(5)
            .ToListAsync();

        var vm = new AdminDashboardViewModel
        {
            TotalUsers = totalUsers,
            TotalAlumni = totalAlumni,
            TotalStudents = totalStudents,
            TotalAdministrators = totalAdmins,
            TotalAnnouncements = totalAnnouncements,
            TotalJobPostings = totalJobPostings,
            TotalApplications = totalApplications,
            RecentUsers = recentUsers
        };

        return View(vm);
    }

    // ── USER MANAGEMENT (READ ALL) ───────────────────────────────────────────
    // GET: /Admin/Users
    [HttpGet("Users")]
    public async Task<IActionResult> Users(string? q, string? role)
    {
        ViewData["ActiveAdminTab"] = "Users";
        ViewData["ActivePage"] = "AdminUsers";
        ViewData["SearchQuery"] = q;
        ViewData["SelectedRole"] = role;

        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.Trim().ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(search) || u.Email.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(role) && ValidRoles.Contains(role))
        {
            query = query.Where(u => u.Role == role);
        }

        var users = await query
            .OrderByDescending(u => u.Id)
            .ToListAsync();

        return View(users);
    }

    // ── CREATE USER ──────────────────────────────────────────────────────────
    // GET: /Admin/Users/Create
    [HttpGet("Users/Create")]
    public IActionResult CreateUser()
    {
        ViewData["ActiveAdminTab"] = "CreateUser";
        return View(new AdminUserCreateViewModel());
    }

    // POST: /Admin/Users/Create
    [HttpPost("Users/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(AdminUserCreateViewModel model)
    {
        ViewData["ActiveAdminTab"] = "CreateUser";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!ValidRoles.Contains(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), "Please select a valid role: Administrator, Alumni, or Student.");
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "A user with this email address is already registered.");
            return View(model);
        }

        var canonicalRole = model.Role.Equals("Administrator", StringComparison.OrdinalIgnoreCase) ? "Administrator"
            : model.Role.Equals("Student", StringComparison.OrdinalIgnoreCase) ? "Student"
            : "Alumni";

        var newUser = new User
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Role = canonicalRole
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, model.Password);

        _db.Users.Add(newUser);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.Email), "A user with this email address is already registered.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"User '{newUser.Name}' ({newUser.Email}) was successfully created as {newUser.Role}.";
        return RedirectToAction(nameof(Users));
    }

    // ── USER DETAILS ─────────────────────────────────────────────────────────
    // GET: /Admin/Users/Details/{id}
    [HttpGet("Users/Details/{id:int}")]
    public async Task<IActionResult> UserDetails(int id)
    {
        ViewData["ActiveAdminTab"] = "Users";

        var user = await _db.Users
            .Include(u => u.JobPostings)
            .Include(u => u.Applications)
                .ThenInclude(a => a.JobPosting)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            TempData["ErrorMessage"] = $"User with ID {id} was not found.";
            return RedirectToAction(nameof(Users));
        }

        var vm = new AdminUserDetailsViewModel
        {
            User = user,
            JobPostings = user.JobPostings.ToList(),
            Applications = user.Applications.ToList(),
            IsCurrentAdmin = IsCurrentUser(user)
        };

        return View(vm);
    }

    // ── EDIT USER ────────────────────────────────────────────────────────────
    // GET: /Admin/Users/Edit/{id}
    [HttpGet("Users/Edit/{id:int}")]
    public async Task<IActionResult> EditUser(int id)
    {
        ViewData["ActiveAdminTab"] = "Users";

        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = $"User with ID {id} was not found.";
            return RedirectToAction(nameof(Users));
        }

        var vm = new AdminUserEditViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsCurrentAdmin = IsCurrentUser(user)
        };

        return View(vm);
    }

    // POST: /Admin/Users/Edit/{id}
    [HttpPost("Users/Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(int id, AdminUserEditViewModel model)
    {
        ViewData["ActiveAdminTab"] = "Users";

        if (id != model.Id)
        {
            return BadRequest();
        }

        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = $"User with ID {id} was not found.";
            return RedirectToAction(nameof(Users));
        }

        model.IsCurrentAdmin = IsCurrentUser(user);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!ValidRoles.Contains(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), "Please select a valid role: Administrator, Alumni, or Student.");
            return View(model);
        }

        // Security Guard: Prevent an administrator from revoking their own administrator privileges
        if (model.IsCurrentAdmin && !string.Equals(model.Role, "Administrator", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Role), "Security Guard: You cannot demote your own account or revoke your own Administrator privileges.");
            return View(model);
        }

        // Duplicate email check across other users
        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var emailExists = await _db.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Another user with this email address already exists.");
            return View(model);
        }

        var canonicalRole = model.Role.Equals("Administrator", StringComparison.OrdinalIgnoreCase) ? "Administrator"
            : model.Role.Equals("Student", StringComparison.OrdinalIgnoreCase) ? "Student"
            : "Alumni";

        user.Name = model.Name.Trim();
        user.Email = model.Email.Trim();
        user.Role = canonicalRole;

        // Update password if a new one was provided
        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.Email), "Another user with this email address already exists.");
            return View(model);
        }

        // Refresh authentication cookie claims if the administrator updated their own profile
        if (model.IsCurrentAdmin)
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

        TempData["SuccessMessage"] = $"User '{user.Name}' was successfully updated.";
        return RedirectToAction(nameof(Users));
    }

    // ── DELETE USER ──────────────────────────────────────────────────────────
    // GET: /Admin/Users/Delete/{id}
    [HttpGet("Users/Delete/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        ViewData["ActiveAdminTab"] = "Users";

        var user = await _db.Users
            .Include(u => u.JobPostings)
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            TempData["ErrorMessage"] = $"User with ID {id} was not found.";
            return RedirectToAction(nameof(Users));
        }

        var vm = new AdminUserDetailsViewModel
        {
            User = user,
            JobPostings = user.JobPostings.ToList(),
            Applications = user.Applications.ToList(),
            IsCurrentAdmin = IsCurrentUser(user)
        };

        return View(vm);
    }

    // POST: /Admin/Users/Delete/{id}
    [HttpPost("Users/Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserConfirmed(int id)
    {
        ViewData["ActiveAdminTab"] = "Users";

        var user = await _db.Users
            .Include(u => u.JobPostings)
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            TempData["ErrorMessage"] = $"User with ID {id} was not found.";
            return RedirectToAction(nameof(Users));
        }

        // Security Guard 1: Prevent an administrator from deleting their own account
        if (IsCurrentUser(user))
        {
            TempData["ErrorMessage"] = "Security Guard: You cannot delete your own Administrator account.";
            return RedirectToAction(nameof(Users));
        }

        // Security Guard 2: Prevent deleting users with dependent records (ON DELETE RESTRICT in database)
        if (user.JobPostings.Count > 0 || user.Applications.Count > 0)
        {
            TempData["ErrorMessage"] = $"Cannot delete user '{user.Name}': account has active dependencies ({user.JobPostings.Count} job postings, {user.Applications.Count} applications). Reassign or remove these records first.";
            return RedirectToAction(nameof(Users));
        }

        _db.Users.Remove(user);
        try
        {
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User '{user.Name}' ({user.Email}) was successfully removed.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = $"Cannot delete user '{user.Name}' because related records still reference this account.";
        }

        return RedirectToAction(nameof(Users));
    }

    // ── SECURITY HELPER ──────────────────────────────────────────────────────
    private bool IsCurrentUser(User user)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(currentUserIdStr, out var currentUserId) && currentUserId == user.Id)
        {
            return true;
        }

        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        return !string.IsNullOrEmpty(currentUserEmail) &&
               string.Equals(currentUserEmail, user.Email, StringComparison.OrdinalIgnoreCase);
    }
}
