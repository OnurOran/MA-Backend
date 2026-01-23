using System.Collections.Concurrent;
using System.Net;

namespace SurveyBackend.Api.Middleware;

public sealed class LoginRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoginRateLimitingMiddleware> _logger;

    private static readonly ConcurrentDictionary<string, (int Count, DateTimeOffset LastAttempt)> _failedAttempts = new();

    private const int MaxAttemptsPerWindow = 5;
    private static readonly TimeSpan WindowDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(1);
    private static DateTimeOffset _lastCleanup = DateTimeOffset.UtcNow;

    public LoginRateLimitingMiddleware(
        RequestDelegate next,
        ILogger<LoginRateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        if (context.Request.Path.StartsWithSegments("/api/auth/login") &&
            context.Request.Method == "POST")
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var username = await TryGetUsernameFromRequestAsync(context);
            var key = $"{ipAddress}:{username}";

            if (IsRateLimited(key))
            {
                _logger.LogWarning("Rate limit exceeded for {Key}", key);
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Çok fazla başarısız giriş denemesi. Lütfen 15 dakika sonra tekrar deneyin.\"}");
                return;
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                if (context.Response.StatusCode == 401)
                {
                    RecordFailedAttempt(key);
                }
                else if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {

                    _failedAttempts.TryRemove(key, out _);
                }
            }
            finally
            {

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }

            CleanupOldEntries();
        }
        else
        {
            await _next(context);
        }
    }

    private static bool IsRateLimited(string key)
    {
        if (!_failedAttempts.TryGetValue(key, out var attempt))
            return false;

        var now = DateTimeOffset.UtcNow;
        var windowExpired = (now - attempt.LastAttempt) > WindowDuration;

        if (windowExpired)
        {
            _failedAttempts.TryRemove(key, out _);
            return false;
        }

        return attempt.Count >= MaxAttemptsPerWindow;
    }

    private static void RecordFailedAttempt(string key)
    {
        var now = DateTimeOffset.UtcNow;

        _failedAttempts.AddOrUpdate(
            key,
            (1, now),
            (_, existing) =>
            {
                var windowExpired = (now - existing.LastAttempt) > WindowDuration;
                return windowExpired ? (1, now) : (existing.Count + 1, now);
            });
    }

    private static void CleanupOldEntries()
    {
        var now = DateTimeOffset.UtcNow;

        if ((now - _lastCleanup) < CleanupInterval)
            return;

        _lastCleanup = now;

        var keysToRemove = _failedAttempts
            .Where(kvp => (now - kvp.Value.LastAttempt) > WindowDuration)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _failedAttempts.TryRemove(key, out _);
        }
    }

    private static async Task<string> TryGetUsernameFromRequestAsync(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            var usernameIndex = body.IndexOf("\"username\"", StringComparison.OrdinalIgnoreCase);
            if (usernameIndex >= 0)
            {
                var startQuote = body.IndexOf(':', usernameIndex) + 1;
                startQuote = body.IndexOf('"', startQuote) + 1;
                var endQuote = body.IndexOf('"', startQuote);

                if (startQuote > 0 && endQuote > startQuote)
                {
                    return body.Substring(startQuote, endQuote - startQuote);
                }
            }
        }
        catch
        {

        }

        return "unknown";
    }
}
