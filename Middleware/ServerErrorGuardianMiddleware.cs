using Microsoft.AspNetCore.WebUtilities;

namespace Alumni.Middleware;

/// <summary>
/// Catches unhandled exceptions and unstarted 5xx responses from downstream middleware
/// and endpoints, logs the error on the server, and returns a safe structured JSON 5xx response.
/// </summary>
public class ServerErrorGuardianMiddleware
{
    private const string DefaultServerErrorReason =
        "An unexpected server error occurred while processing your request.";

    private readonly RequestDelegate _next;
    private readonly ILogger<ServerErrorGuardianMiddleware> _logger;

    public ServerErrorGuardianMiddleware(
        RequestDelegate next,
        ILogger<ServerErrorGuardianMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestedPath = context.Request.Path.HasValue && !string.IsNullOrEmpty(context.Request.Path.Value)
            ? context.Request.Path.Value!
            : "/";

        try
        {
            await _next(context);

            if (context.Response.StatusCode is >= StatusCodes.Status500InternalServerError and <= 599
                && !context.Response.HasStarted)
            {
                var statusCode = context.Response.StatusCode;
                var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
                var title = string.IsNullOrWhiteSpace(reasonPhrase) ? "Server Error" : reasonPhrase;

                _logger.LogError(
                    "Server error status {StatusCode} returned while processing {Method} {Path}",
                    statusCode,
                    context.Request.Method,
                    requestedPath);

                await WriteErrorResponseAsync(context, statusCode, title, requestedPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred while processing {Method} {Path}",
                context.Request.Method,
                requestedPath);

            if (context.Response.HasStarted)
            {
                throw;
            }

            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                requestedPath);
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string title,
        string requestedPath)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        if (HttpMethods.IsHead(context.Request.Method))
        {
            return;
        }

        var payload = new
        {
            statusCode,
            title,
            reason = DefaultServerErrorReason,
            path = requestedPath
        };

        await context.Response.WriteAsJsonAsync(payload);
    }
}
