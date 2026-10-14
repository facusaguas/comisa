namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Servicio para importar el parte diario desde Excel
/// </summary>
public interface IExcelParteDiarioService
{
    /// <summary>
    /// Importa el parte diario desde un archivo Excel
    /// </summary>
    /// <param name="filePath">Ruta del archivo Excel</param>
    /// <param name="periodo">Periodo en formato YYYY-MM</param>
    /// <param name="quincena">Número de quincena (1 o 2)</param>
    /// <param name="proyectoId">ID del proyecto al que pertenecen las horas</param>
    /// <returns>Número de registros importados</returns>
    Task<int> ImportarParteDiarioAsync(string filePath, string periodo, int quincena, int proyectoId);

    /// <summary>
    /// Importa el parte diario desde un stream
    /// </summary>
    /// <param name="stream">Stream del archivo Excel</param>
    /// <param name="periodo">Periodo en formato YYYY-MM</param>
    /// <param name="quincena">Número de quincena (1 o 2)</param>
    /// <param name="proyectoId">ID del proyecto al que pertenecen las horas</param>
    /// <returns>Número de registros importados</returns>
    Task<int> ImportarParteDiarioAsync(Stream stream, string periodo, int quincena, int proyectoId);
}
