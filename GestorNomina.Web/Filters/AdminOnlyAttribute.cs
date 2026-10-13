using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GestorNomina.Web.Services;

namespace GestorNomina.Web.Filters;

/// <summary>
/// Atributo que requiere que el usuario tenga el rol de Administrador
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Verificar si el usuario está autenticado
        if (!SessionHelper.IsAuthenticated(context.HttpContext.Session))
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
            return;
        }

        // Verificar que el usuario tenga el rol de Administrador
        if (!SessionHelper.HasRole(context.HttpContext.Session, "Administrador"))
        {
            context.Result = new RedirectToActionResult("AccesoDenegado", "Auth", null);
            return;
        }
    }
}
