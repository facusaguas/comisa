using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

public interface IAuthService
{
    /// <summary>
    /// Valida las credenciales del usuario y retorna el usuario si son correctas
    /// </summary>
    Task<Usuario?> ValidateCredentialsAsync(string email, string password);

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    Task<(bool Success, string Message, Usuario? Usuario)> RegisterUserAsync(RegisterViewModel model);

    /// <summary>
    /// Cambia la contraseña de un usuario
    /// </summary>
    Task<(bool Success, string Message)> ChangePasswordAsync(int usuarioId, string currentPassword, string newPassword);

    /// <summary>
    /// Actualiza el perfil de un usuario
    /// </summary>
    Task<(bool Success, string Message)> UpdateProfileAsync(int usuarioId, ProfileViewModel model);

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    Task<Usuario?> GetUserByIdAsync(int usuarioId);

    /// <summary>
    /// Obtiene un usuario por su email
    /// </summary>
    Task<Usuario?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Hashea una contraseña usando BCrypt
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifica si una contraseña coincide con el hash almacenado
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// Valida si un email ya existe en el sistema
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
}
