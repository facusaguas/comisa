using GestorNomina.Web.Models.Entities;

namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Servicio para exportar recibos al formato de Tango Gestión
/// </summary>
public interface ITangoExportService
{
    /// <summary>
    /// Exporta un recibo al formato de Tango
    /// </summary>
    /// <param name="reciboId">ID del recibo</param>
    /// <returns>Contenido del archivo de exportación</returns>
    Task<string> ExportarReciboAsync(int reciboId);

    /// <summary>
    /// Exporta múltiples recibos al formato de Tango
    /// </summary>
    /// <param name="recibos">Lista de recibos a exportar</param>
    /// <returns>Contenido del archivo de exportación</returns>
    Task<string> ExportarATango(List<Recibo> recibos);

    /// <summary>
    /// Exporta múltiples recibos al formato de Tango
    /// </summary>
    /// <param name="periodo">Periodo en formato YYYY-MM</param>
    /// <param name="quincena">Número de quincena (1 o 2)</param>
    /// <returns>Contenido del archivo de exportación</returns>
    Task<string> ExportarPeriodoAsync(string periodo, int quincena);

    /// <summary>
    /// Guarda la exportación en un archivo
    /// </summary>
    /// <param name="contenido">Contenido a guardar</param>
    /// <param name="rutaArchivo">Ruta del archivo de salida</param>
    Task GuardarArchivoAsync(string contenido, string rutaArchivo);
}
