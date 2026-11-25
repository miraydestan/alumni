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
public class AccountController : Controller
{
    private readonly AlumniDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    // Allowed self-registration roles. "Administrator" is strictly prohibited.
    private static readonly HashSet<string> AllowedUserTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Alumni",
        "Student"
    };

    public AccountController(AlumniDbContext db, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    // GET: /Account/Register or /register
    [HttpGet]
    [Route("Account/Register")]
    [Route("register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["Title"] = "Member Registration";
        ViewData["ActivePage"] = "Register";
        return View(new RegisterViewModel());
    }

    // POST: /Account/Register or /register
    [HttpPost]
    [Route("Account/Register")]
    [Route("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewData["Title"] = "Member Registration";
        ViewData["ActivePage"] = "Register";

        // 1. Model State Validation (HTML5 / DataAnnotations)
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // 2. Strict Role & Security Check
        // Explicitly reject any attempt to register as Administrator
        if (string.Equals(model.UserType, "Administrator", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.UserType), "Registration as Administrator is strictly prohibited.");
            return View(model);
        }

        if (!AllowedUserTypes.Contains(model.UserType))
        {
            ModelState.AddModelError(nameof(model.UserType), "Invalid user type. Permitted roles are Alumni or Student.");
            return View(model);
        }

        var canonicalRole = string.Equals(model.UserType, "Student", StringComparison.OrdinalIgnoreCase)
            ? "Student"
            : "Alumni";

        // 3. Password Confirmation Check
        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(model.ConfirmPassword), "The password and confirmation password do not match.");
            return View(model);
        }

        // 4. Block Self-Registration with Reserved Configured Admin Email
        var trimmedEmail = model.Email.Trim();
        var configuredAdminEmail = GetConfiguredAdminEmail();
        if (!string.IsNullOrWhiteSpace(configuredAdminEmail) &&
            string.Equals(trimmedEmail, configuredAdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Email), "This email address is reserved for system administration.");
            return View(model);
        }

        // 5. Duplicate Email Prevention (Case-insensitive)
        var normalizedEmail = trimmedEmail.ToLowerInvariant();
        var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email address is already registered.");
            return View(model);
        }

        // 6. Construct User Entity with Secure Password Hash
        var newUser = new User
        {
            Name = model.Name.Trim(),
            Email = trimmedEmail,
            Role = canonicalRole
        };

        // Secure password hashing with PBKDF2-SHA256 and cryptographic salt
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, model.Password);

        // 7. Persist to PostgreSQL Database
        _db.Users.Add(newUser);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email address is already registered.");
            return View(model);
        }

        // 7. Clear Feedback and Redirect to Login
        TempData["SuccessMessage"] = $"Registration successful! Welcome to the Alumni Network, {newUser.Name}. You may now sign in.";
        return RedirectToAction(nameof(Login));
    }

    // GET: /Account/Login or /login
    [HttpGet]
    [Route("Account/Login")]
    [Route("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        // If already logged in, redirect to returnUrl or homepage
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["Title"] = "Member Login";
        ViewData["ActivePage"] = "Login";
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Account/Login or /login
    [HttpPost]
    [Route("Account/Login")]
    [Route("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["Title"] = "Member Login";
        ViewData["ActivePage"] = "Login";

        // 1. Validate Form Inputs
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();

        // 2. Retrieve User from PostgreSQL using EF Core (Case-Insensitive)
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        // Security: Generic error message for both nonexistent email and wrong password
        // Prevents user enumeration attacks
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid email address or password.");
            return View(model);
        }

        // 3. Cryptographic Password Verification
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid email address or password.");
            return View(model);
        }

        // 4. Construct Claims Principal strictly from the persisted database Role
        // (No automatic role promotion by email during login; roles are managed only via DatabaseSeeder or Admin Panel)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(7)
                : null
        };

        // 5. Issue Fresh Cookie Authentication Session (Mitigates session fixation)
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);

        TempData["SuccessMessage"] = $"Welcome back, {user.Name}!";

        // 6. Safe Local Redirection (Prevents Open Redirect Vulnerabilities)
        return RedirectToLocal(model.ReturnUrl);
    }

    // POST: /Account/Logout or /logout
    [HttpPost]
    [Route("Account/Logout")]
    [Route("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "You have been successfully signed out.";
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Logout or /logout (Safety redirect for direct URL hits)
    [HttpGet]
    [Route("Account/Logout")]
    [Route("logout")]
    public IActionResult LogoutGet()
    {
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Profile or /profile
    [Authorize]
    [HttpGet]
    [Route("Account/Profile")]
    [Route("profile")]
    public async Task<IActionResult> Profile()
    {
        ViewData["Title"] = "My Profile";
        ViewData["ActivePage"] = "Profile";

        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _db.Users
            .Include(u => u.JobPostings)
                .ThenInclude(j => j.Applications)
                    .ThenInclude(a => a.User)
            .Include(u => u.Applications)
                .ThenInclude(a => a.JobPosting)
                    .ThenInclude(j => j!.User)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        return View(user);
    }

    // GET: /Account/EditProfile or /profile/edit
    [Authorize]
    [HttpGet]
    [Route("Account/EditProfile")]
    [Route("profile/edit")]
    public async Task<IActionResult> EditProfile()
    {
        ViewData["Title"] = "Edit Profile";
        ViewData["ActivePage"] = "Profile";

        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        var vm = new EditProfileViewModel
        {
            UserId = user.Id,
            Role = user.Role,
            Name = user.Name,
            Email = user.Email
        };

        return View(vm);
    }

    // POST: /Account/EditProfile or /profile/edit
    [Authorize]
    [HttpPost]
    [Route("Account/EditProfile")]
    [Route("profile/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        ViewData["Title"] = "Edit Profile";
        ViewData["ActivePage"] = "Profile";

        // Security: Always identify target account exclusively from authenticated claims
        var userId = GetCurrentUserId();
        if (userId <= 0)
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // Re-populate read-only display fields from authoritative database record
        model.UserId = user.Id;
        model.Role = user.Role;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var trimmedName = model.Name.Trim();
        var trimmedEmail = model.Email.Trim();
        var normalizedEmail = trimmedEmail.ToLowerInvariant();

        // Security: Prevent non-administrators from changing their email to the reserved configured Admin email
        var configuredAdminEmail = GetConfiguredAdminEmail();
        if (!string.Equals(user.Role, "Administrator", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(configuredAdminEmail) &&
            string.Equals(trimmedEmail, configuredAdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Email), "This email address is reserved for system administration.");
            return View(model);
        }

        // Check duplicate email against all OTHER users (case-insensitive)
        var emailInUse = await _db.Users.AnyAsync(u => u.Id != userId && u.Email.ToLower() == normalizedEmail);
        if (emailInUse)
        {
            ModelState.AddModelError(nameof(model.Email), "This email address is already registered to another account.");
            return View(model);
        }

        // Update only permitted profile fields; Role and PasswordHash are strictly untouched
        user.Name = trimmedName;
        user.Email = trimmedEmail;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.Email), "This email address is already registered to another account.");
            return View(model);
        }

        // Refresh authentication cookie claims so navbar and session reflect updated Name & Email immediately
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

        TempData["SuccessMessage"] = "Your profile information has been updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    private string GetConfiguredAdminEmail()
    {
        var envAdminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        var configAdminEmail = _configuration["Admin:Email"];
        return !string.IsNullOrWhiteSpace(envAdminEmail)
            ? envAdminEmail.Trim()
            : !string.IsNullOrWhiteSpace(configAdminEmail)
                ? configAdminEmail.Trim()
                : "admin@alumni.edu.tr";
    }

    // GET: /Account/AccessDenied or /access-denied
    [HttpGet]
    [Route("Account/AccessDenied")]
    [Route("access-denied")]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        ViewData["Title"] = "Access Denied";
        ViewData["ActivePage"] = "AccessDenied";
        return View();
    }

    // Helper: Validates that a return URL belongs to the current application
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
