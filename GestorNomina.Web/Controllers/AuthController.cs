using Microsoft.AspNetCore.Mvc;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;

namespace GestorNomina.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Si ya está autenticado, redirigir al dashboard
        if (SessionHelper.IsAuthenticated(HttpContext.Session))
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuario = await _authService.ValidateCredentialsAsync(model.Email, model.Password);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos");
            return View(model);
        }

        // Establecer la sesión del usuario
        SessionHelper.SetUserSession(HttpContext.Session, usuario);

        _logger.LogInformation("Usuario {Email} ha iniciado sesión correctamente", model.Email);

        // Redirigir a la URL de retorno o al dashboard
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/Logout
    public IActionResult Logout()
    {
        var userEmail = SessionHelper.GetUserEmail(HttpContext.Session);
        
        // Limpiar la sesión
        SessionHelper.ClearUserSession(HttpContext.Session);

        _logger.LogInformation("Usuario {Email} ha cerrado sesión", userEmail);

        TempData["InfoMessage"] = "Has cerrado sesión exitosamente";
        return RedirectToAction("Login");
    }

    // GET: /Auth/MiPerfil
    [HttpGet]
    public async Task<IActionResult> MiPerfil()
    {
        var usuarioId = SessionHelper.GetUserId(HttpContext.Session);
        if (!usuarioId.HasValue)
        {
            return RedirectToAction("Login");
        }

        var usuario = await _authService.GetUserByIdAsync(usuarioId.Value);
        if (usuario == null)
        {
            TempData["ErrorMessage"] = "Usuario no encontrado";
            return RedirectToAction("Login");
        }

        var model = new ProfileViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            RolNombre = usuario.Rol?.Nombre ?? "N/A",
            CreatedAt = usuario.CreatedAt,
            UpdatedAt = usuario.UpdatedAt
        };

        return View(model);
    }

    // POST: /Auth/MiPerfil
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MiPerfil(ProfileViewModel model)
    {
        var usuarioId = SessionHelper.GetUserId(HttpContext.Session);
        if (!usuarioId.HasValue)
        {
            return RedirectToAction("Login");
        }

        if (!ModelState.IsValid)
        {
            // Recargar datos de solo lectura
            var usuario = await _authService.GetUserByIdAsync(usuarioId.Value);
            if (usuario != null)
            {
                model.RolNombre = usuario.Rol?.Nombre ?? "N/A";
                model.CreatedAt = usuario.CreatedAt;
                model.UpdatedAt = usuario.UpdatedAt;
            }
            return View(model);
        }

        var result = await _authService.UpdateProfileAsync(usuarioId.Value, model);

        if (result.Success)
        {
            // Actualizar la sesión con el nuevo nombre
            var usuarioActualizado = await _authService.GetUserByIdAsync(usuarioId.Value);
            if (usuarioActualizado != null)
            {
                SessionHelper.SetUserSession(HttpContext.Session, usuarioActualizado);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("MiPerfil");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        
        // Recargar datos de solo lectura
        var usuarioData = await _authService.GetUserByIdAsync(usuarioId.Value);
        if (usuarioData != null)
        {
            model.RolNombre = usuarioData.Rol?.Nombre ?? "N/A";
            model.CreatedAt = usuarioData.CreatedAt;
            model.UpdatedAt = usuarioData.UpdatedAt;
        }

        return View(model);
    }

    // GET: /Auth/CambiarPassword
    [HttpGet]
    public IActionResult CambiarPassword()
    {
        var usuarioId = SessionHelper.GetUserId(HttpContext.Session);
        if (!usuarioId.HasValue)
        {
            return RedirectToAction("Login");
        }

        return View(new ChangePasswordViewModel());
    }

    // POST: /Auth/CambiarPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(ChangePasswordViewModel model)
    {
        var usuarioId = SessionHelper.GetUserId(HttpContext.Session);
        if (!usuarioId.HasValue)
        {
            return RedirectToAction("Login");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.ChangePasswordAsync(usuarioId.Value, model.CurrentPassword, model.NewPassword);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("MiPerfil");
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    // GET: /Auth/AccesoDenegado
    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        return View();
    }
}
