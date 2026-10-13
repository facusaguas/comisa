using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GestorNomina.Web.Services;

namespace GestorNomina.Web.Filters;

/// <summary>
/// Atributo de autorización personalizado basado en sesiones
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string[]? Roles { get; set; }

    public AuthorizeAttribute()
    {
    }

    public AuthorizeAttribute(params string[] roles)
    {
        Roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Verificar si el usuario está autenticado
        if (!SessionHelper.IsAuthenticated(context.HttpContext.Session))
        {
            // Redirigir al login con la URL de retorno
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
            return;
        }

        // Si se especificaron roles, verificar que el usuario tenga alguno de ellos
        if (Roles != null && Roles.Length > 0)
        {
            if (!SessionHelper.HasAnyRole(context.HttpContext.Session, Roles))
            {
                // Redirigir a acceso denegado
                context.Result = new RedirectToActionResult("AccesoDenegado", "Auth", null);
                return;
            }
        }
    }
}
