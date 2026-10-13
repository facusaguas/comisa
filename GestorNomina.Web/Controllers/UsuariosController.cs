using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;
using GestorNomina.Web.Filters;
using GestorNomina.Web.Constants;

namespace GestorNomina.Web.Controllers;

[AdminOnly]
public class UsuariosController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        ApplicationDbContext context,
        IAuthService authService,
        ILogger<UsuariosController> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Usuarios";
        ViewData["PagePretitle"] = "Administración";

        var usuarios = await _context.Usuarios
            .Include(u => u.Rol)
            .OrderBy(u => u.Apellido)
            .ThenBy(u => u.Nombre)
            .ToListAsync();

        var viewModels = usuarios.Select(u => new UsuarioViewModel
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Email = u.Email,
            Username = u.Email, // Usamos el email como username
            RolId = u.RolId,
            RolNombre = u.Rol?.Nombre,
            Activo = u.Activo,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();

        return View(viewModels);
    }

    // GET: Usuarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        var viewModel = new UsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Username = usuario.Email, // Usamos el email como username
            RolId = usuario.RolId,
            RolNombre = usuario.Rol?.Nombre,
            Activo = usuario.Activo,
            CreatedAt = usuario.CreatedAt,
            UpdatedAt = usuario.UpdatedAt
        };

        ViewData["PageTitle"] = "Detalles del Usuario";
        ViewData["PagePretitle"] = "Usuarios";

        return View(viewModel);
    }

    // GET: Usuarios/Create
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "Nuevo Usuario";
        ViewData["PagePretitle"] = "Usuarios";
        ViewBag.Roles = new SelectList(_context.Roles, "Id", "Nombre");
        
        return View(new RegisterViewModel());
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Validar que el email no exista
            if (await _authService.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "El email ya está registrado en el sistema");
                ViewData["PageTitle"] = "Nuevo Usuario";
                ViewData["PagePretitle"] = "Usuarios";
                ViewBag.Roles = new SelectList(_context.Roles, "Id", "Nombre", model.RolId);
                return View(model);
            }

            // Crear usuario usando el servicio de autenticación
            var result = await _authService.RegisterUserAsync(model);

            if (result.Success)
            {
                _logger.LogInformation("Usuario creado: {Email} por {CreadorEmail}", 
                    model.Email, CurrentUserEmail);
                
                SetSuccessMessage($"Usuario {model.Nombre} {model.Apellido} creado exitosamente");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message);
        }

        ViewData["PageTitle"] = "Nuevo Usuario";
        ViewData["PagePretitle"] = "Usuarios";
        ViewBag.Roles = new SelectList(_context.Roles, "Id", "Nombre", model.RolId);
        return View(model);
    }

    // GET: Usuarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        var model = new UsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            RolId = usuario.RolId,
            Activo = usuario.Activo,
            RolNombre = usuario.Rol?.Nombre,
            CreatedAt = usuario.CreatedAt,
            UpdatedAt = usuario.UpdatedAt
        };

        ViewData["PageTitle"] = "Editar Usuario";
        ViewData["PagePretitle"] = "Usuarios";
        ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", model.RolId);
        
        return View(model);
    }

    // POST: Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                // Verificar si el email cambió y si ya existe
                if (usuario.Email.ToLower() != model.Email.ToLower())
                {
                    if (await _authService.EmailExistsAsync(model.Email, id))
                    {
                        ModelState.AddModelError("Email", "El email ya está registrado por otro usuario");
                        ViewData["PageTitle"] = "Editar Usuario";
                        ViewData["PagePretitle"] = "Usuarios";
                        ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", model.RolId);
                        return View(model);
                    }
                }

                // Actualizar datos
                usuario.Nombre = model.Nombre.Trim();
                usuario.Apellido = model.Apellido.Trim();
                usuario.Email = model.Email.ToLower().Trim();
                usuario.RolId = model.RolId;
                usuario.Activo = model.Activo;
                usuario.UpdatedAt = DateTime.UtcNow;

                // Si se proporcionó una nueva contraseña, actualizarla
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    usuario.PasswordHash = _authService.HashPassword(model.NewPassword);
                }

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario actualizado: {Email} por {EditorEmail}", 
                    model.Email, CurrentUserEmail);

                SetSuccessMessage($"Usuario {model.Nombre} {model.Apellido} actualizado exitosamente");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UsuarioExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        ViewData["PageTitle"] = "Editar Usuario";
        ViewData["PagePretitle"] = "Usuarios";
        ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Nombre", model.RolId);
        return View(model);
    }

    // GET: Usuarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        // No permitir eliminar al usuario actual
        if (usuario.Id == CurrentUserId)
        {
            SetErrorMessage("No puedes eliminar tu propio usuario");
            return RedirectToAction(nameof(Index));
        }

        ViewData["PageTitle"] = "Eliminar Usuario";
        ViewData["PagePretitle"] = "Usuarios";

        return View(usuario);
    }

    // POST: Usuarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        
        if (usuario == null)
        {
            return NotFound();
        }

        // No permitir eliminar al usuario actual
        if (usuario.Id == CurrentUserId)
        {
            SetErrorMessage("No puedes eliminar tu propio usuario");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Usuario eliminado: {Email} por {EliminadorEmail}", 
                usuario.Email, CurrentUserEmail);

            SetSuccessMessage($"Usuario {usuario.Nombre} {usuario.Apellido} eliminado exitosamente");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario ID: {Id}", id);
            SetErrorMessage("No se puede eliminar el usuario porque tiene registros relacionados");
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Usuarios/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        
        if (usuario == null)
        {
            return NotFound();
        }

        // No permitir desactivar al usuario actual
        if (usuario.Id == CurrentUserId)
        {
            SetErrorMessage("No puedes desactivar tu propio usuario");
            return RedirectToAction(nameof(Index));
        }

        usuario.Activo = !usuario.Activo;
        usuario.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        var estado = usuario.Activo ? "activado" : "desactivado";
        _logger.LogInformation("Usuario {Estado}: {Email} por {ModificadorEmail}", 
            estado, usuario.Email, CurrentUserEmail);

        SetSuccessMessage($"Usuario {usuario.Nombre} {usuario.Apellido} {estado} exitosamente");
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> UsuarioExists(int id)
    {
        return await _context.Usuarios.AnyAsync(e => e.Id == id);
    }
}
