using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Alumni.Middleware;

/// <summary>
/// Logs completed HTTP requests (both MVC and Web API) to the configured logger in the format:
/// time method route restype status duration
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var route = context.Request.Path.HasValue && !string.IsNullOrEmpty(context.Request.Path.Value)
            ? context.Request.Path.Value!
            : "/";
        var unhandledException = false;

        try
        {
            await _next(context);
        }
        catch
        {
            unhandledException = true;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            var statusCode = unhandledException && context.Response.StatusCode < StatusCodes.Status400BadRequest
                ? StatusCodes.Status500InternalServerError
                : context.Response.StatusCode;
            var completionTime = DateTime.Now.ToString("HH:mm:ss");
            var resourceType = ResolveResourceType(context, route);
            var durationMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "{Time} {Method} {Route} {ResType} {Status} {Duration}ms",
                completionTime,
                method,
                route,
                resourceType,
                statusCode,
                durationMs);
        }
    }

    private static string ResolveResourceType(HttpContext context, string route)
    {
        var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (actionDescriptor is not null)
        {
            return actionDescriptor.ControllerName switch
            {
                "ApiUser" or "User" => "user",
                "ApiJob" or "JobPosting" => "job",
                "ApiApplication" => "application",
                "Announcement" or "AnnouncementAdmin" => "announcement",
                "Health" => "health",
                "Home" => "home",
                "Alumni" => "alumni",
                _ => actionDescriptor.ControllerName.ToLowerInvariant()
            };
        }

        if (MatchesPrefix(route, "/api/users") || MatchesPrefix(route, "/users") || MatchesPrefix(route, "/user"))
            return "user";

        if (MatchesPrefix(route, "/api/jobs") || MatchesPrefix(route, "/jobs") || MatchesPrefix(route, "/jobposting"))
            return "job";

        if (MatchesPrefix(route, "/api/applications"))
            return "application";

        if (MatchesPrefix(route, "/announcement") || MatchesPrefix(route, "/announcementadmin"))
            return "announcement";

        if (MatchesPrefix(route, "/api/health"))
            return "health";

        if (route == "/" || MatchesPrefix(route, "/about") || MatchesPrefix(route, "/home"))
            return "home";

        if (MatchesPrefix(route, "/hello") || MatchesPrefix(route, "/sum"))
            return "alumni";

        return "unknown";
    }

    private static bool MatchesPrefix(string path, string prefix)
    {
        return path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase);
    }
}
