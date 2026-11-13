namespace GestorNomina.Web.Services;

/// <summary>
/// Interfaz para el servicio de exportación a Excel
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Genera un archivo Excel con los empleados de un proyecto para una quincena específica
    /// </summary>
    /// <param name="proyectoId">ID del proyecto</param>
    /// <param name="mes">Mes (1-12)</param>
    /// <param name="anio">Año</param>
    /// <param name="quincena">Quincena (1 o 2)</param>
    /// <returns>Bytes del archivo Excel</returns>
    Task<byte[]> GenerarExcelEmpleadosProyectoAsync(int proyectoId, int mes, int anio, int quincena);
}
