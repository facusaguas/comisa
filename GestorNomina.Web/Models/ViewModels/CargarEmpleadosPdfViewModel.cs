using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para cargar empleados desde archivos PDF
/// </summary>
public class CargarEmpleadosPdfViewModel
{
    [Required(ErrorMessage = "Debe seleccionar al menos un archivo PDF")]
    [Display(Name = "Archivos PDF")]
    public List<IFormFile> Archivos { get; set; } = new();
}

/// <summary>
/// ViewModel para mostrar resultado de procesamiento de un PDF
/// </summary>
public class ResultadoProcesamentoPdfViewModel
{
    public string Archivo { get; set; } = string.Empty;
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    
    // Datos extraídos del PDF
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Cuit { get; set; }
    public DateTime? FechaInicio { get; set; }
    public decimal? RetribPactada { get; set; }
    
    // Resultado de la operación
    public string? Legajo { get; set; }
    public bool EsEmpleadoNuevo { get; set; }
    public string? TipoOperacion { get; set; } // "Creado", "Reactivado", "Actualizado"
}
