namespace Alumni.Middleware;

/// <summary>
/// Intercepts HTTP 404 responses that have not yet started writing a body
/// and returns a structured JSON response explaining why the resource was not found.
/// </summary>
public class NotFoundGuardianMiddleware
{
    private readonly RequestDelegate _next;

    public NotFoundGuardianMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestedPath = context.Request.Path.HasValue && !string.IsNullOrEmpty(context.Request.Path.Value)
            ? context.Request.Path.Value!
            : "/";

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status404NotFound
            && !context.Response.HasStarted
            && !HttpMethods.IsHead(context.Request.Method))
        {
            var payload = new
            {
                statusCode = StatusCodes.Status404NotFound,
                title = "Not Found",
                reason = "The requested resource or endpoint could not be found.",
                path = requestedPath
            };

            await context.Response.WriteAsJsonAsync(payload);
        }
    }
}
