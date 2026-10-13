using GestorNomina.Web.Services;

namespace GestorNomina.Web.Middleware;

/// <summary>
/// Middleware para logging de requests y auditoría de accesos
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
        // Capturar información del request
        var path = context.Request.Path;
        var method = context.Request.Method;
        var userEmail = SessionHelper.GetUserEmail(context.Session);
        var userId = SessionHelper.GetUserId(context.Session);

        // Log solo si es un usuario autenticado y no es un request de assets estáticos
        if (userId.HasValue && !path.StartsWithSegments("/css") && 
            !path.StartsWithSegments("/js") && !path.StartsWithSegments("/lib") &&
            !path.StartsWithSegments("/favicon"))
        {
            _logger.LogInformation(
                "Request: {Method} {Path} | Usuario: {UserEmail} (ID: {UserId})",
                method, path, userEmail, userId.Value
            );
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method para registrar el middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
