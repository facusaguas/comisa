using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para la gestión de asignaciones empleado-proyecto
/// </summary>
public class AsignacionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El empleado es obligatorio")]
    [Display(Name = "Empleado")]
    public int EmpleadoId { get; set; }

    [Required(ErrorMessage = "El proyecto es obligatorio")]
    [Display(Name = "Proyecto")]
    public int ProyectoId { get; set; }

    [Required(ErrorMessage = "El régimen laboral es obligatorio")]
    [Display(Name = "Régimen Laboral")]
    public int RegimenLaboralId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Fin")]
    public DateTime? FechaFin { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
    [Display(Name = "Observaciones")]
    [DataType(DataType.MultilineText)]
    public string? Observaciones { get; set; }

    // Propiedades de visualización
    public string? EmpleadoNombre { get; set; }
    public string? EmpleadoDni { get; set; }
    public string? ProyectoNombre { get; set; }
    public string? ProyectoCodigo { get; set; }
    public string? RegimenLaboralNombre { get; set; }

    [Display(Name = "Empleado")]
    public string EmpleadoCompleto => $"{EmpleadoNombre} ({EmpleadoDni})";

    [Display(Name = "Proyecto")]
    public string ProyectoCompleto => $"{ProyectoCodigo} - {ProyectoNombre}";

    [Display(Name = "Estado")]
    public string EstadoTexto => Activo ? "Activo" : "Inactivo";

    [Display(Name = "Duración")]
    public string Duracion
    {
        get
        {
            var fin = FechaFin ?? DateTime.Now;
            var dias = (fin - FechaInicio).Days;
            if (dias < 30) return $"{dias} días";
            var meses = dias / 30;
            return $"{meses} meses";
        }
    }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// ViewModel para crear/editar asignaciones
/// </summary>
public class AsignacionFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El empleado es obligatorio")]
    [Display(Name = "Empleado")]
    public int EmpleadoId { get; set; }

    [Required(ErrorMessage = "El proyecto es obligatorio")]
    [Display(Name = "Proyecto")]
    public int ProyectoId { get; set; }

    [Required(ErrorMessage = "El régimen laboral es obligatorio")]
    [Display(Name = "Régimen Laboral")]
    public int RegimenLaboralId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Fin (Opcional)")]
    public DateTime? FechaFin { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
    [Display(Name = "Observaciones")]
    [DataType(DataType.MultilineText)]
    public string? Observaciones { get; set; }

    // Listas para dropdowns
    public List<EmpleadoViewModel> Empleados { get; set; } = new();
    public List<ProyectoViewModel> Proyectos { get; set; } = new();
    public List<RegimenLaboralViewModel> RegimenesLaborales { get; set; } = new();
}
