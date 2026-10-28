namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para mostrar horas agrupadas por empleado en formato parte diario
/// </summary>
public class EmpleadoParteDiarioViewModel
{
    public int EmpleadoId { get; set; }
    public string Legajo { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string NombreCompleto => $"{Apellido}, {Nombre}";
    
    // Diccionario: día del mes -> horas trabajadas
    public Dictionary<int, decimal> HorasPorDia { get; set; } = new();
    
    // Total de horas del empleado en el periodo
    public decimal TotalHoras => HorasPorDia.Values.Sum();
    
    // Indica si el empleado tiene registros pendientes
    public bool TieneRegistrosPendientes { get; set; }
}

/// <summary>
/// ViewModel contenedor para la vista de Registros Pendientes
/// </summary>
public class RegistrosPendientesViewModel
{
    public int PeriodoCargaId { get; set; }
    public string Periodo { get; set; } = string.Empty; // Formato: YYYY-MM
    public string ProyectoNombre { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Mes { get; set; }
    public int Quincena { get; set; } // 1 o 2
    
    // Lista de empleados con sus horas
    public List<EmpleadoParteDiarioViewModel> Empleados { get; set; } = new();
    
    // Días de la quincena (1-15 o 16-último día del mes)
    public List<int> DiasQuincena { get; set; } = new();
    
    // Total de registros pendientes
    public int TotalRegistros { get; set; }
}
