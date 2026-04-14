using System.Diagnostics;
using System.Security.Claims;

namespace ESBot.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        var ip = context.Connection.RemoteIpAddress?.ToString();
        var method = request.Method;
        var path = request.Path;
        var userAgent = request.Headers.UserAgent.ToString();
        var userId =
            context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "anonymous";
        var query = request.QueryString.ToString();
        var correlationId = context.TraceIdentifier;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception during request. " +
                "CorrelationId={CorrelationId} | HTTP Method={Method} | Path={Path} | Query={Query} | IP={IP} | User={UserId} | UserAgent={UserAgent}",
                correlationId,
                method,
                path,
                query,
                ip,
                userId,
                userAgent
            );

            throw;
        }
        finally
        {
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms | IP={IP} | User={UserId} | UA={UserAgent}",
                method,
                path,
                statusCode,
                stopwatch.ElapsedMilliseconds,
                ip,
                userId,
                userAgent
            );
        }
    }
}