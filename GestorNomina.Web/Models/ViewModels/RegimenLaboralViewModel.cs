using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para la gestión de regímenes laborales
/// </summary>
public class RegimenLaboralViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los días de trabajo son obligatorios")]
    [Range(1, 31, ErrorMessage = "Los días de trabajo deben estar entre 1 y 31")]
    [Display(Name = "Días de Trabajo")]
    public int DiasTrabajo { get; set; }

    [Required(ErrorMessage = "Los días de descanso son obligatorios")]
    [Range(1, 31, ErrorMessage = "Los días de descanso deben estar entre 1 y 31")]
    [Display(Name = "Días de Descanso")]
    public int DiasDescanso { get; set; }

    [Required(ErrorMessage = "Las horas máximas diarias son obligatorias")]
    [Range(1, 24, ErrorMessage = "Las horas máximas diarias deben estar entre 1 y 24")]
    [Display(Name = "Máximo Horas Diarias")]
    public decimal MaxHorasDiarias { get; set; }

    [Range(1, 168, ErrorMessage = "Las horas semanales deben estar entre 1 y 168")]
    [Display(Name = "Horas Semanales Límite")]
    public decimal? HorasSemanalesLimite { get; set; }

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
    [Display(Name = "Descripción")]
    [DataType(DataType.MultilineText)]
    public string? Descripcion { get; set; }

    // Propiedades calculadas
    [Display(Name = "Ciclo Completo")]
    public int CicloCompleto => DiasTrabajo + DiasDescanso;

    [Display(Name = "Horas Totales por Ciclo")]
    public decimal HorasTotalesCiclo => DiasTrabajo * MaxHorasDiarias;

    [Display(Name = "Formato")]
    public string Formato => $"{DiasTrabajo}x{DiasDescanso}";
}
