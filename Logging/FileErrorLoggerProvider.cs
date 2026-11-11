using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace Alumni.Logging;

/// <summary>
/// Logging provider that persists error-level application logs to logs/error.logs
/// alongside the existing console logging infrastructure.
/// </summary>
[ProviderAlias("FileError")]
public sealed class FileErrorLoggerProvider : ILoggerProvider
{
    private static readonly Regex PasswordPattern = new(
        @"(Password|Pwd)\s*=\s*[^;\s]+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex BearerPattern = new(
        @"Bearer\s+[A-Za-z0-9\-._~+/]+=*",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly string _logsDirectory;
    private readonly string _logFilePath;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly object _syncLock = new();
    private readonly ConcurrentDictionary<string, FileErrorLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    public FileErrorLoggerProvider(IHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
    {
        _logsDirectory = Path.Combine(environment.ContentRootPath, "logs");
        _logFilePath = Path.Combine(_logsDirectory, "error.logs");
        _httpContextAccessor = httpContextAccessor;

        TryEnsureLogsDirectory();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new FileErrorLogger(name, this));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }

    internal void WriteError(string categoryName, LogLevel logLevel, string message, Exception? exception)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var levelText = logLevel == LogLevel.Critical ? "CRITICAL" : "ERROR";
            var httpContext = _httpContextAccessor.HttpContext;
            var requestContext = httpContext is not null
                ? $"{httpContext.Request.Method} {(httpContext.Request.Path.HasValue && !string.IsNullOrEmpty(httpContext.Request.Path.Value) ? httpContext.Request.Path.Value : "/")}"
                : categoryName;

            var sanitizedMessage = Sanitize(message);
            var builder = new StringBuilder();
            builder.AppendLine($"[{timestamp}] [{levelText}] {requestContext}");
            builder.AppendLine($"Message: {sanitizedMessage}");

            if (exception is not null)
            {
                builder.AppendLine($"Exception: {FormatException(exception)}");
            }

            builder.AppendLine();

            lock (_syncLock)
            {
                Directory.CreateDirectory(_logsDirectory);
                File.AppendAllText(_logFilePath, builder.ToString(), Encoding.UTF8);
            }
        }
        catch
        {
            // Never allow file-logging failures to crash the application or mask HTTP error handling.
        }
    }

    private void TryEnsureLogsDirectory()
    {
        try
        {
            Directory.CreateDirectory(_logsDirectory);
        }
        catch
        {
            // Ignore directory creation failures at startup; writes will retry safely.
        }
    }

    private static string FormatException(Exception exception)
    {
        var sb = new StringBuilder();
        var current = exception;
        var depth = 0;

        while (current is not null && depth < 5)
        {
            if (depth > 0)
            {
                sb.Append(" ---> ");
            }

            sb.Append(current.GetType().Name);
            if (!string.IsNullOrWhiteSpace(current.Message))
            {
                sb.Append(": ").Append(Sanitize(current.Message));
            }

            current = current.InnerException;
            depth++;
        }

        if (!string.IsNullOrWhiteSpace(exception.StackTrace))
        {
            sb.AppendLine().Append(Sanitize(exception.StackTrace));
        }

        return sb.ToString();
    }

    private static string Sanitize(string value)
    {
        var masked = PasswordPattern.Replace(value, "$1=***");
        return BearerPattern.Replace(masked, "Bearer ***");
    }

    private sealed class FileErrorLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly FileErrorLoggerProvider _provider;

        public FileErrorLogger(string categoryName, FileErrorLoggerProvider provider)
        {
            _categoryName = categoryName;
            _provider = provider;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            // Skip EF Core's pre-throw error logs so a single failed DB call isn't duplicated
            // when ServerErrorGuardianMiddleware (or Program startup) logs the caught exception.
            return logLevel >= LogLevel.Error
                && !_categoryName.StartsWith("Microsoft.EntityFrameworkCore.", StringComparison.OrdinalIgnoreCase);
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception is null)
            {
                return;
            }

            _provider.WriteError(_categoryName, logLevel, message, exception);
        }
    }
}
