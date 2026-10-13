using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GestorNomina.Web.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Usuario?> ValidateCredentialsAsync(string email, string password)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Activo);

            if (usuario == null)
            {
                _logger.LogWarning("Intento de login con email no existente o usuario inactivo: {Email}", email);
                return null;
            }

            if (!VerifyPassword(password, usuario.PasswordHash))
            {
                _logger.LogWarning("Intento de login con contraseña incorrecta para el usuario: {Email}", email);
                return null;
            }

            _logger.LogInformation("Usuario autenticado exitosamente: {Email}", email);
            return usuario;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar credenciales para el email: {Email}", email);
            return null;
        }
    }

    public async Task<(bool Success, string Message, Usuario? Usuario)> RegisterUserAsync(RegisterViewModel model)
    {
        try
        {
            // Validar que el email no exista
            if (await EmailExistsAsync(model.Email))
            {
                return (false, "El email ya está registrado en el sistema", null);
            }

            // Validar que el rol exista
            var rol = await _context.Roles.FindAsync(model.RolId);
            if (rol == null)
            {
                return (false, "El rol seleccionado no existe", null);
            }

            // Crear el nuevo usuario
            var usuario = new Usuario
            {
                Email = model.Email.ToLower().Trim(),
                Nombre = model.Nombre.Trim(),
                Apellido = model.Apellido.Trim(),
                RolId = model.RolId,
                PasswordHash = HashPassword(model.Password),
                Activo = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario registrado exitosamente: {Email}", usuario.Email);
            
            // Cargar el rol para el usuario recién creado
            await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();

            return (true, "Usuario registrado exitosamente", usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar usuario: {Email}", model.Email);
            return (false, "Error al registrar el usuario. Por favor, intente nuevamente.", null);
        }
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(int usuarioId, string currentPassword, string newPassword)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return (false, "Usuario no encontrado");
            }

            // Verificar la contraseña actual
            if (!VerifyPassword(currentPassword, usuario.PasswordHash))
            {
                return (false, "La contraseña actual es incorrecta");
            }

            // Actualizar la contraseña
            usuario.PasswordHash = HashPassword(newPassword);
            usuario.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Contraseña cambiada exitosamente para el usuario ID: {UsuarioId}", usuarioId);
            return (true, "Contraseña cambiada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña para el usuario ID: {UsuarioId}", usuarioId);
            return (false, "Error al cambiar la contraseña. Por favor, intente nuevamente.");
        }
    }

    public async Task<(bool Success, string Message)> UpdateProfileAsync(int usuarioId, ProfileViewModel model)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return (false, "Usuario no encontrado");
            }

            // Verificar si el email cambió y si ya existe
            if (usuario.Email.ToLower() != model.Email.ToLower())
            {
                if (await EmailExistsAsync(model.Email, usuarioId))
                {
                    return (false, "El email ya está registrado por otro usuario");
                }
            }

            // Actualizar los datos del perfil
            usuario.Nombre = model.Nombre.Trim();
            usuario.Apellido = model.Apellido.Trim();
            usuario.Email = model.Email.ToLower().Trim();
            usuario.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Perfil actualizado exitosamente para el usuario ID: {UsuarioId}", usuarioId);
            return (true, "Perfil actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar perfil para el usuario ID: {UsuarioId}", usuarioId);
            return (false, "Error al actualizar el perfil. Por favor, intente nuevamente.");
        }
    }

    public async Task<Usuario?> GetUserByIdAsync(int usuarioId)
    {
        try
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por ID: {UsuarioId}", usuarioId);
            return null;
        }
    }

    public async Task<Usuario?> GetUserByEmailAsync(string email)
    {
        try
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por email: {Email}", email);
            return null;
        }
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(11));
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar contraseña");
            return false;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        try
        {
            var query = _context.Usuarios.Where(u => u.Email.ToLower() == email.ToLower());
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de email: {Email}", email);
            return false;
        }
    }
}
