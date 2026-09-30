using Microsoft.AspNetCore.Mvc;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> _users = new();

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(_users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] User user)
    {
        _users.Add(user);
        return Created($"/api/users/{user.Id}", user);
    }

    [HttpPut("{id}")]
    [HttpPatch("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;

        return Ok(existingUser);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        _users.Remove(user);
        return NoContent();
    }
}
