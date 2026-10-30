using System.ComponentModel.DataAnnotations;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Models.ViewModels;

public class NovedadViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El empleado es requerido")]
    [Display(Name = "Empleado")]
    public int EmpleadoId { get; set; }

    [Required(ErrorMessage = "El periodo es requerido")]
    [Display(Name = "Periodo")]
    [DataType(DataType.Date)]
    public DateTime Periodo { get; set; }

    [Required(ErrorMessage = "La quincena es requerida")]
    [Display(Name = "Quincena")]
    [Range(1, 2, ErrorMessage = "La quincena debe ser 1 o 2")]
    public int Quincena { get; set; }

    [Required(ErrorMessage = "El tipo de novedad es requerido")]
    [Display(Name = "Tipo de Novedad")]
    public TipoNovedadLiquidacion TipoNovedad { get; set; }

    [Required(ErrorMessage = "La fecha de novedad es requerida")]
    [Display(Name = "Fecha de Novedad")]
    [DataType(DataType.Date)]
    public DateTime FechaNovedad { get; set; }

    [Display(Name = "Monto")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
    public decimal? MontoNovedad { get; set; }

    [Display(Name = "Observaciones")]
    [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
    [DataType(DataType.MultilineText)]
    public string? Observaciones { get; set; }

    // Para mostrar en la vista
    public string? EmpleadoNombre { get; set; }
    public string? EmpleadoLegajo { get; set; }
}
