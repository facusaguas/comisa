using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

/// <summary>
/// Servicio para procesar carga masiva de empleados desde PDFs
/// </summary>
public interface IPdfEmpleadoService
{
    /// <summary>
    /// Extrae datos de un archivo PDF
    /// </summary>
    Task<(bool Success, string Nombre, string Apellido, string Cuit, DateTime? FechaInicio, decimal? RetribPactada)> 
        ExtraerDatosDelPdfAsync(IFormFile archivo);
    
    /// <summary>
    /// Procesa el empleado: verifica CUIT, crea o actualiza
    /// </summary>
    Task<ResultadoProcesamentoPdfViewModel> ProcesarEmpleadoAsync(
        string nombre, 
        string apellido, 
        string cuit, 
        DateTime? fechaInicio,
        decimal? retribPactada);
    
    /// <summary>
    /// Procesa múltiples archivos PDF
    /// </summary>
    Task<List<ResultadoProcesamentoPdfViewModel>> ProcesarMultiplesPdfsAsync(List<IFormFile> archivos);
}
