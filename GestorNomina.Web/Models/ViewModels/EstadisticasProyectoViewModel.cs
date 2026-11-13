namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para las estadísticas de un proyecto
/// </summary>
public class EstadisticasProyectoViewModel
{
    public int ProyectoId { get; set; }
    public string ProyectoNombre { get; set; } = string.Empty;
    public string ProyectoCodigo { get; set; } = string.Empty;
    public int TotalEmpleados { get; set; }
    public Dictionary<string, int> EmpleadosPorTipoContratacion { get; set; } = new();
}
