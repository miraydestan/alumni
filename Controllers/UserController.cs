using Microsoft.AspNetCore.Mvc;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class UserController : Controller
{
    // GET: /users
    [HttpGet("/users")]
    public IActionResult UsersList()
    {
        ViewData["ActivePage"] = "Users";
        return View("Index", UserStore.Users);
    }

    // POST: /users
    [HttpPost("/users")]
    public IActionResult CreateFromUsersRoute(User user)
    {
        ViewData["ActivePage"] = "Users";

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            return View("Index", UserStore.Users);
        }

        if (user.Id == 0)
        {
            user.Id = UserStore.Users.Count > 0 ? UserStore.Users.Max(u => u.Id) + 1 : 1;
        }

        UserStore.Users.Add(user);
        return Redirect("/users");
    }

    // GET: /User or /User/Index
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Users";
        return View(UserStore.Users);
    }

    // GET: /User/Details/{id}
    [HttpGet]
    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Users";
        var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

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
    public IActionResult Create(User user)
    {
        ViewData["ActivePage"] = "Users";

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            return View(user);
        }

        if (user.Id == 0)
        {
            user.Id = UserStore.Users.Count > 0 ? UserStore.Users.Max(u => u.Id) + 1 : 1;
        }

        UserStore.Users.Add(user);
        return RedirectToAction(nameof(Index));
    }

    // GET: /User/Edit/{id}
    [HttpGet]
    public IActionResult Edit(int id)
    {
        ViewData["ActivePage"] = "Users";
        var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: /User/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, User updatedUser)
    {
        ViewData["ActivePage"] = "Users";
        var existingUser = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updatedUser.Name) || string.IsNullOrWhiteSpace(updatedUser.Email))
        {
            ModelState.AddModelError(string.Empty, "Name and Email are required.");
            return View(updatedUser);
        }

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;

        return RedirectToAction(nameof(Index));
    }

    // GET: /User/Delete/{id}
    [HttpGet]
    public IActionResult Delete(int id)
    {
        ViewData["ActivePage"] = "Users";
        var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: /User/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            UserStore.Users.Remove(user);
        }

        return RedirectToAction(nameof(Index));
    }
}
