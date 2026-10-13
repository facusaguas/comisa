using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para la gestión de empleados
/// </summary>
public class EmpleadoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El legajo es obligatorio")]
    [StringLength(50, ErrorMessage = "El legajo no puede exceder los 50 caracteres")]
    [Display(Name = "Legajo")]
    public string Legajo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El documento no puede exceder los 50 caracteres")]
    [Display(Name = "Documento de Identidad")]
    public string? DocumentoIdentidad { get; set; }

    [StringLength(100, ErrorMessage = "El cargo no puede exceder los 100 caracteres")]
    [Display(Name = "Cargo")]
    public string? Cargo { get; set; }

    [Display(Name = "Régimen Laboral")]
    public int? RegimenId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Ingreso")]
    public DateTime? FechaIngreso { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    // Propiedades de visualización
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto => $"{Apellido}, {Nombre}";

    [Display(Name = "Estado")]
    public string EstadoTexto => Activo ? "Activo" : "Inactivo";

    // Régimen laboral (para visualización)
    public string? RegimenNombre { get; set; }

    // Proyectos asignados (para visualización)
    public List<string> ProyectosAsignados { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
