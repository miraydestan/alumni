using Microsoft.AspNetCore.Mvc;
using Alumni.Models;

namespace Alumni.Controllers;

[ApiController]
[Route("api/users")]
public class ApiUserController : ControllerBase
{
    // GET: /api/users
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(UserStore.Users);
    }

    // GET: /api/users/{id}
    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: /api/users
    [HttpPost]
    public IActionResult CreateUser([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest();
        }

        if (user.Id == 0)
        {
            user.Id = UserStore.Users.Count > 0 ? UserStore.Users.Max(u => u.Id) + 1 : 1;
        }

        UserStore.Users.Add(user);
        return Created($"/api/users/{user.Id}", user);
    }

    // PUT: /api/users/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        var existingUser = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;

        return Ok(existingUser);
    }

    // PATCH: /api/users/{id}
    [HttpPatch("{id}")]
    public IActionResult PatchUser(int id, [FromBody] User updatedUser)
    {
        var existingUser = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(updatedUser.Name))
        {
            existingUser.Name = updatedUser.Name;
        }

        if (!string.IsNullOrEmpty(updatedUser.Email))
        {
            existingUser.Email = updatedUser.Email;
        }

        return Ok(existingUser);
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        UserStore.Users.Remove(user);
        return NoContent();
    }
}
