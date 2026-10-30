using System.ComponentModel.DataAnnotations;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Models.ViewModels;

public class ConceptoLiquidacionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de concepto es requerido")]
    [Display(Name = "Tipo de Concepto")]
    public TipoConcepto TipoConcepto { get; set; }
}
