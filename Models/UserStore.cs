namespace Alumni.Models;

/// <summary>
/// Simple shared in-memory data store for User entities.
/// Shared across UserController (MVC) and ApiUserController (Web API).
/// </summary>
public static class UserStore
{
    public static List<User> Users { get; } = new();
}
