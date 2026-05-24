using System.Diagnostics;

namespace BooksAPI.Middleware;

// 1. Correlation ID Middleware
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items[CorrelationIdHeader] = correlationId;
        
        // Add header safely using OnStarting
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeader))
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
            }
            return Task.CompletedTask;
        });

        await _next(context);
    }
}

// 2. Request Logging Middleware
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
        var correlationId = context.Items["X-Correlation-ID"];
        _logger.LogInformation("[{CorrelationId}] Request: {Method} {Path}{QueryString}", 
            correlationId, context.Request.Method, context.Request.Path, context.Request.QueryString);
        
        await _next(context);
    }
}

// 3. Request Timing Middleware
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        
        // Add header safely using OnStarting
        context.Response.OnStarting(() =>
        {
            sw.Stop();
            var elapsedMs = sw.ElapsedMilliseconds;
            context.Response.Headers["X-Response-Time"] = $"{elapsedMs}ms";
            
            var correlationId = context.Items["X-Correlation-ID"];
            if (elapsedMs > 500)
            {
                _logger.LogWarning("[{CorrelationId}] Slow Request: {Method} {Path} took {Elapsed}ms", 
                    correlationId, context.Request.Method, context.Request.Path, elapsedMs);
            }
            else
            {
                _logger.LogInformation("[{CorrelationId}] Request completed in {Elapsed}ms", correlationId, elapsedMs);
            }
            
            return Task.CompletedTask;
        });

        await _next(context);
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<CorrelationIdMiddleware>();
        builder.UseMiddleware<RequestLoggingMiddleware>();
        builder.UseMiddleware<RequestTimingMiddleware>();
        return builder;
    }
}
