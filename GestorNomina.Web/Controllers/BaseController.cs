using Microsoft.AspNetCore.Mvc;
using GestorNomina.Web.Services;

namespace GestorNomina.Web.Controllers;

/// <summary>
/// Controlador base con métodos helper para todos los controladores
/// </summary>
public abstract class BaseController : Controller
{
    /// <summary>
    /// Obtiene el ID del usuario autenticado actual
    /// </summary>
    protected int? CurrentUserId => SessionHelper.GetUserId(HttpContext.Session);

    /// <summary>
    /// Obtiene el email del usuario autenticado actual
    /// </summary>
    protected string? CurrentUserEmail => SessionHelper.GetUserEmail(HttpContext.Session);

    /// <summary>
    /// Obtiene el nombre completo del usuario autenticado actual
    /// </summary>
    protected string? CurrentUserName => SessionHelper.GetUserName(HttpContext.Session);

    /// <summary>
    /// Obtiene el rol del usuario autenticado actual
    /// </summary>
    protected string? CurrentUserRole => SessionHelper.GetUserRole(HttpContext.Session);

    /// <summary>
    /// Obtiene el ID del rol del usuario autenticado actual
    /// </summary>
    protected int? CurrentUserRoleId => SessionHelper.GetUserRoleId(HttpContext.Session);

    /// <summary>
    /// Verifica si el usuario actual está autenticado
    /// </summary>
    protected bool IsAuthenticated => SessionHelper.IsAuthenticated(HttpContext.Session);

    /// <summary>
    /// Verifica si el usuario actual tiene un rol específico
    /// </summary>
    protected bool HasRole(string roleName) => SessionHelper.HasRole(HttpContext.Session, roleName);

    /// <summary>
    /// Verifica si el usuario actual tiene alguno de los roles especificados
    /// </summary>
    protected bool HasAnyRole(params string[] roleNames) => SessionHelper.HasAnyRole(HttpContext.Session, roleNames);

    /// <summary>
    /// Verifica si el usuario actual es Administrador
    /// </summary>
    protected bool IsAdmin => HasRole("Administrador");

    /// <summary>
    /// Verifica si el usuario actual es Supervisor
    /// </summary>
    protected bool IsSupervisor => HasRole("Supervisor");

    /// <summary>
    /// Verifica si el usuario actual es Operador
    /// </summary>
    protected bool IsOperador => HasRole("Operador");

    /// <summary>
    /// Establece un mensaje de éxito en TempData
    /// </summary>
    protected void SetSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    /// <summary>
    /// Establece un mensaje de error en TempData
    /// </summary>
    protected void SetErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    /// <summary>
    /// Establece un mensaje de advertencia en TempData
    /// </summary>
    protected void SetWarningMessage(string message)
    {
        TempData["WarningMessage"] = message;
    }

    /// <summary>
    /// Establece un mensaje informativo en TempData
    /// </summary>
    protected void SetInfoMessage(string message)
    {
        TempData["InfoMessage"] = message;
    }

    /// <summary>
    /// Redirige al login con la URL actual como returnUrl
    /// </summary>
    protected IActionResult RedirectToLogin()
    {
        var returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
        return RedirectToAction("Login", "Auth", new { returnUrl });
    }

    /// <summary>
    /// Redirige a la página de acceso denegado
    /// </summary>
    protected IActionResult RedirectToAccessDenied()
    {
        return RedirectToAction("AccesoDenegado", "Auth");
    }
}
