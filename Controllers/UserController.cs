using System.ComponentModel.DataAnnotations;
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

[ApiExplorerSettings(IgnoreApi = true)]
public class UserController : Controller
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    private readonly AlumniDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserController(AlumniDbContext db, IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    // GET: /users
    [HttpGet("/users")]
    public async Task<IActionResult> UsersList()
    {
        ViewData["ActivePage"] = "Users";
        var users = await _db.Users.ToListAsync();
        return View("Index", users);
    }

    // POST: /users  (quick-create form on the Index page — Administrator only)
    [Authorize(Roles = "Administrator")]
    [HttpPost("/users")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromUsersRoute(string? name, string? email, string? password)
    {
        ViewData["ActivePage"] = "Users";

        var trimmedName = name?.Trim() ?? string.Empty;
        var trimmedEmail = email?.Trim() ?? string.Empty;
        var rawPassword = password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(trimmedName) || string.IsNullOrWhiteSpace(trimmedEmail))
        {
            TempData["ErrorMessage"] = "Full name and email address are required.";
            return Redirect("/users");
        }

        if (!IsValidEmail(trimmedEmail))
        {
            TempData["ErrorMessage"] = "Please enter a valid email address.";
            return Redirect("/users");
        }

        if (!IsValidPassword(rawPassword))
        {
            TempData["ErrorMessage"] = "Password is required and must be at least 6 non-whitespace characters long.";
            return Redirect("/users");
        }

        var normalizedEmail = trimmedEmail.ToLowerInvariant();
        var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            TempData["ErrorMessage"] = "A user with this email address already exists.";
            return Redirect("/users");
        }

        var newUser = new User
        {
            Name = trimmedName,
            Email = trimmedEmail,
            Role = "Alumni"
        };
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, rawPassword);

        _db.Users.Add(newUser);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "A user with this email address already exists.";
            return Redirect("/users");
        }

        TempData["SuccessMessage"] = $"Member '{newUser.Name}' was successfully added.";
        return Redirect("/users");
    }

    // GET: /User or /User/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "Users";
        var users = await _db.Users.ToListAsync();
        return View(users);
    }

    // GET: /User/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        ViewData["ActivePage"] = "Users";
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // GET: /User/Create
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["ActivePage"] = "Users";
        return View(new User());
    }

    // POST: /User/Create
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string? name, string? email, string? password)
    {
        ViewData["ActivePage"] = "Users";

        var trimmedName = name?.Trim() ?? string.Empty;
        var trimmedEmail = email?.Trim() ?? string.Empty;
        var rawPassword = password ?? string.Empty;

        var formModel = new User { Name = trimmedName, Email = trimmedEmail };

        if (string.IsNullOrWhiteSpace(trimmedName) || string.IsNullOrWhiteSpace(trimmedEmail))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            return View(formModel);
        }

        if (!IsValidEmail(trimmedEmail))
        {
            ModelState.AddModelError(nameof(Alumni.Models.User.Email), "Please enter a valid email address.");
            return View(formModel);
        }

        if (!IsValidPassword(rawPassword))
        {
            ModelState.AddModelError(string.Empty, "Password is required and must be at least 6 non-whitespace characters long.");
            return View(formModel);
        }

        var normalizedEmail = trimmedEmail.ToLowerInvariant();
        var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(Alumni.Models.User.Email), "A user with this email address already exists.");
            return View(formModel);
        }

        var newUser = new User
        {
            Name = trimmedName,
            Email = trimmedEmail,
            Role = "Alumni"
        };
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, rawPassword);

        _db.Users.Add(newUser);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(Alumni.Models.User.Email), "A user with this email address already exists.");
            return View(formModel);
        }

        TempData["SuccessMessage"] = $"Member '{newUser.Name}' was successfully registered.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /User/Edit/{id}
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["ActivePage"] = "Users";
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // POST: /User/Edit/{id}
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string? name, string? email, string? newPassword)
    {
        ViewData["ActivePage"] = "Users";

        var existing = await _db.Users.FindAsync(id);
        if (existing == null)
            return NotFound();

        var trimmedName = name?.Trim() ?? string.Empty;
        var trimmedEmail = email?.Trim() ?? string.Empty;

        var formModel = new User
        {
            Id = existing.Id,
            Name = trimmedName,
            Email = trimmedEmail,
            Role = existing.Role
        };

        if (string.IsNullOrWhiteSpace(trimmedName) || string.IsNullOrWhiteSpace(trimmedEmail))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            return View(formModel);
        }

        if (!IsValidEmail(trimmedEmail))
        {
            ModelState.AddModelError(nameof(Alumni.Models.User.Email), "Please enter a valid email address.");
            return View(formModel);
        }

        if (!string.IsNullOrEmpty(newPassword) && !IsValidPassword(newPassword))
        {
            ModelState.AddModelError(string.Empty, "New password must be at least 6 non-whitespace characters long if provided.");
            return View(formModel);
        }

        var normalizedEmail = trimmedEmail.ToLowerInvariant();
        var emailInUse = await _db.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == normalizedEmail);
        if (emailInUse)
        {
            ModelState.AddModelError(nameof(Alumni.Models.User.Email), "Another user with this email address already exists.");
            return View(formModel);
        }

        var isSelf = IsCurrentUser(existing);

        existing.Name = trimmedName;
        existing.Email = trimmedEmail;

        if (!string.IsNullOrEmpty(newPassword))
        {
            existing.PasswordHash = _passwordHasher.HashPassword(existing, newPassword);
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(Alumni.Models.User.Email), "Another user with this email address already exists.");
            return View(formModel);
        }

        if (isSelf)
        {
            await RefreshCurrentUserCookieAsync(existing);
        }

        TempData["SuccessMessage"] = $"Member '{existing.Name}' was successfully updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /User/Delete/{id}
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        ViewData["ActivePage"] = "Users";
        var user = await _db.Users
            .Include(u => u.JobPostings)
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

        ViewBag.IsCurrentAdmin = IsCurrentUser(user);
        ViewBag.JobPostingCount = user.JobPostings.Count;
        ViewBag.ApplicationCount = user.Applications.Count;

        return View(user);
    }

    // POST: /User/Delete/{id}
    [Authorize(Roles = "Administrator")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _db.Users
            .Include(u => u.JobPostings)
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return RedirectToAction(nameof(Index));

        // Security Guard 1: Prevent administrator from deleting their own account
        if (IsCurrentUser(user))
        {
            TempData["ErrorMessage"] = "Security Guard: You cannot delete your own Administrator account.";
            return RedirectToAction(nameof(Index));
        }

        // Security Guard 2: Prevent deleting users with dependent records (ON DELETE RESTRICT)
        if (user.JobPostings.Count > 0 || user.Applications.Count > 0)
        {
            TempData["ErrorMessage"] = $"Cannot delete user '{user.Name}': account has active dependencies ({user.JobPostings.Count} job postings, {user.Applications.Count} applications).";
            return RedirectToAction(nameof(Index));
        }

        _db.Users.Remove(user);
        try
        {
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Member '{user.Name}' was successfully removed.";
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = $"Cannot delete user '{user.Name}' because related records still reference this account.";
        }

        return RedirectToAction(nameof(Index));
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email)
            && email.Length <= 150
            && EmailValidator.IsValid(email);
    }

    private static bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
    }

    private bool IsCurrentUser(User user)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(currentUserIdStr, out var currentUserId) && currentUserId == user.Id)
            return true;

        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        return !string.IsNullOrEmpty(currentUserEmail) &&
               string.Equals(currentUserEmail, user.Email, StringComparison.OrdinalIgnoreCase);
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
