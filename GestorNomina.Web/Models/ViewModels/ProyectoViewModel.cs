using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para la gestión de proyectos
/// </summary>
public class ProyectoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(50, ErrorMessage = "El código no puede exceder los 50 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La ubicación no puede exceder los 500 caracteres")]
    [Display(Name = "Ubicación")]
    public string? Ubicacion { get; set; }

    [Display(Name = "Régimen Laboral por Defecto")]
    public int? RegimenDefaultId { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    // Propiedades de visualización
    [Display(Name = "Estado")]
    public string EstadoTexto => Activo ? "Activo" : "Inactivo";

    // Régimen por defecto (para visualización)
    public string? RegimenDefaultNombre { get; set; }

    // Empleados asignados (para visualización)
    public int CantidadEmpleados { get; set; }
    public List<string> EmpleadosAsignados { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
