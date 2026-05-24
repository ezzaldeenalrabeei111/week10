using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RequestLoggerAPI.Models;

namespace RequestLoggerAPI.Middleware;

public class RequestLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestLoggerOptions _options;
    private readonly ILogger<RequestLoggerMiddleware> _logger;

    public RequestLoggerMiddleware(RequestDelegate next, IOptions<RequestLoggerOptions> options, ILogger<RequestLoggerMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString();

        // Add header safely using OnStarting
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.Response.Headers["X-Correlation-ID"] = correlationId;
            }
            return Task.CompletedTask;
        });

        await _next(context);

        sw.Stop();

        var logEntry = new RequestLogEntry
        {
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId,
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            StatusCode = context.Response.StatusCode,
            DurationMs = sw.ElapsedMilliseconds,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        };

        await LogToFileAsync(logEntry);

        if (logEntry.DurationMs > _options.SlowRequestThresholdMs)
        {
            _logger.LogWarning("Slow Request Detected: {Method} {Path} took {Duration}ms", logEntry.Method, logEntry.Path, logEntry.DurationMs);
        }
    }

    private async Task LogToFileAsync(RequestLogEntry entry)
    {
        try
        {
            var directory = Path.GetDirectoryName(_options.LogFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var jsonLine = JsonSerializer.Serialize(entry) + Environment.NewLine;
            await File.AppendAllTextAsync(_options.LogFilePath, jsonLine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write request log to file");
        }
    }
}

public static class RequestLoggerExtensions
{
    public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggerMiddleware>();
    }
}
