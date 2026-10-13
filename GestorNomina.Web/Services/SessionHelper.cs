using Microsoft.AspNetCore.Http;
using GestorNomina.Web.Models.Entities;

namespace GestorNomina.Web.Services;

public static class SessionHelper
{
    private const string SessionKeyUserId = "UsuarioId";
    private const string SessionKeyUserName = "UsuarioNombre";
    private const string SessionKeyUserEmail = "UsuarioEmail";
    private const string SessionKeyUserRole = "UsuarioRol";
    private const string SessionKeyUserRoleId = "UsuarioRolId";

    /// <summary>
    /// Establece la sesión del usuario
    /// </summary>
    public static void SetUserSession(ISession session, Usuario usuario)
    {
        session.SetInt32(SessionKeyUserId, usuario.Id);
        session.SetString(SessionKeyUserName, $"{usuario.Nombre} {usuario.Apellido}");
        session.SetString(SessionKeyUserEmail, usuario.Email);
        session.SetString(SessionKeyUserRole, usuario.Rol?.Nombre ?? "Usuario");
        session.SetInt32(SessionKeyUserRoleId, usuario.RolId);
    }

    /// <summary>
    /// Limpia la sesión del usuario
    /// </summary>
    public static void ClearUserSession(ISession session)
    {
        session.Clear();
    }

    /// <summary>
    /// Obtiene el ID del usuario de la sesión
    /// </summary>
    public static int? GetUserId(ISession session)
    {
        return session.GetInt32(SessionKeyUserId);
    }

    /// <summary>
    /// Obtiene el nombre completo del usuario de la sesión
    /// </summary>
    public static string? GetUserName(ISession session)
    {
        return session.GetString(SessionKeyUserName);
    }

    /// <summary>
    /// Obtiene el email del usuario de la sesión
    /// </summary>
    public static string? GetUserEmail(ISession session)
    {
        return session.GetString(SessionKeyUserEmail);
    }

    /// <summary>
    /// Obtiene el rol del usuario de la sesión
    /// </summary>
    public static string? GetUserRole(ISession session)
    {
        return session.GetString(SessionKeyUserRole);
    }

    /// <summary>
    /// Obtiene el ID del rol del usuario de la sesión
    /// </summary>
    public static int? GetUserRoleId(ISession session)
    {
        return session.GetInt32(SessionKeyUserRoleId);
    }

    /// <summary>
    /// Verifica si el usuario está autenticado
    /// </summary>
    public static bool IsAuthenticated(ISession session)
    {
        return GetUserId(session).HasValue;
    }

    /// <summary>
    /// Verifica si el usuario tiene un rol específico
    /// </summary>
    public static bool HasRole(ISession session, string roleName)
    {
        var userRole = GetUserRole(session);
        return userRole != null && userRole.Equals(roleName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifica si el usuario tiene alguno de los roles especificados
    /// </summary>
    public static bool HasAnyRole(ISession session, params string[] roleNames)
    {
        var userRole = GetUserRole(session);
        return userRole != null && roleNames.Any(r => r.Equals(userRole, StringComparison.OrdinalIgnoreCase));
    }
}
