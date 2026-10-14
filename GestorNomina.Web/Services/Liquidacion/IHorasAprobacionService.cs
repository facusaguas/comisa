namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Servicio para aprobar y corregir horas desde registros raw a registros aprobados
/// </summary>
public interface IHorasAprobacionService
{
    /// <summary>
    /// Copia todos los registros raw de un periodo a la tabla de aprobados sin modificaciones
    /// </summary>
    Task<int> AprobarAutomaticamenteAsync(int periodoCargaId, int usuarioId);

    /// <summary>
    /// Aprueba un registro individual con o sin corrección
    /// </summary>
    Task AprobarRegistroAsync(int registroRawId, decimal? horasCorregidas, string? motivoModificacion, int usuarioId);

    /// <summary>
    /// Aprueba múltiples registros en lote
    /// </summary>
    Task<int> AprobarRegistrosEnLoteAsync(List<int> registrosRawIds, int usuarioId);

    /// <summary>
    /// Corrige un registro ya aprobado
    /// </summary>
    Task CorregirRegistroAprobadoAsync(int registroAprobadoId, decimal nuevasHoras, string motivoModificacion, int usuarioId);

    /// <summary>
    /// Obtiene registros raw pendientes de aprobación para un periodo
    /// </summary>
    Task<List<Models.Entities.HoraRegistroRaw>> ObtenerRegistrosPendientesAsync(int periodoCargaId);

    /// <summary>
    /// Verifica si un periodo tiene registros aprobados
    /// </summary>
    Task<bool> TieneRegistrosAprobadosAsync(int periodoCargaId);
}
