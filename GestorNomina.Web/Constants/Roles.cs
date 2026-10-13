namespace GestorNomina.Web.Constants;

/// <summary>
/// Constantes para los nombres de roles del sistema
/// </summary>
public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Supervisor = "Supervisor";
    public const string Operador = "Operador";
    public const string Consulta = "Consulta";

    /// <summary>
    /// Obtiene todos los roles del sistema
    /// </summary>
    public static readonly string[] All = new[]
    {
        Administrador,
        Supervisor,
        Operador,
        Consulta
    };

    /// <summary>
    /// Roles con permisos administrativos
    /// </summary>
    public static readonly string[] AdminRoles = new[]
    {
        Administrador
    };

    /// <summary>
    /// Roles con permisos de supervisión
    /// </summary>
    public static readonly string[] SupervisorRoles = new[]
    {
        Administrador,
        Supervisor
    };

    /// <summary>
    /// Roles con permisos de operación
    /// </summary>
    public static readonly string[] OperadorRoles = new[]
    {
        Administrador,
        Supervisor,
        Operador
    };
}
