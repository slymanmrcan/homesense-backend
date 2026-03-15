namespace HomeSense.Api.Middleware;

public class ApiKeyMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Api-Key";

    // Sadece bu metodlar + path kombinasyonları korunuyor
    private static bool RequiresApiKey(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? string.Empty;

        return method == HttpMethods.Post && path.StartsWith("/api/readings");
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (!RequiresApiKey(context))
        {
            await next(context);
            return;
        }

        var apiKey = configuration["DeviceIngest:MasterApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "API key not configured." });
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var incomingKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Missing API key." });
            return;
        }

        if (!string.Equals(incomingKey.FirstOrDefault(), apiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid API key." });
            return;
        }

        await next(context);
    }
}