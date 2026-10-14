using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alumni.Data;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class UserController : Controller
{
    private readonly AlumniDbContext _db;

    public UserController(AlumniDbContext db)
    {
        _db = db;
    }

    // GET: /users
    [HttpGet("/users")]
    public async Task<IActionResult> UsersList()
    {
        ViewData["ActivePage"] = "Users";
        var users = await _db.Users.ToListAsync();
        return View("Index", users);
    }

    // POST: /users  (quick-create form on the Index page — no AntiForgeryToken in that form)
    [HttpPost("/users")]
    public async Task<IActionResult> CreateFromUsersRoute(User user)
    {
        ViewData["ActivePage"] = "Users";

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            var users = await _db.Users.ToListAsync();
            return View("Index", users);
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
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
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["ActivePage"] = "Users";
        return View();
    }

    // POST: /User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        ViewData["ActivePage"] = "Users";

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            return View(user);
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /User/Edit/{id}
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User updatedUser)
    {
        ViewData["ActivePage"] = "Users";

        var existing = await _db.Users.FindAsync(id);
        if (existing == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(updatedUser.Name) || string.IsNullOrWhiteSpace(updatedUser.Email))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            return View(updatedUser);
        }

        existing.Name  = updatedUser.Name;
        existing.Email = updatedUser.Email;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /User/Delete/{id}
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        ViewData["ActivePage"] = "Users";
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // POST: /User/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
