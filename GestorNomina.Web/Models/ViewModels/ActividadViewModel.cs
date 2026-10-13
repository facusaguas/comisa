using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para la gestión de actividades
/// </summary>
public class ActividadViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(50, ErrorMessage = "El código no puede exceder los 50 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Display(Name = "Es Hora Efectiva")]
    public bool EsHoraEfectiva { get; set; }

    [Display(Name = "Es Licencia")]
    public bool EsLicencia { get; set; }

    [Display(Name = "Es Franco")]
    public bool EsFranco { get; set; }

    // Propiedades de visualización
    [Display(Name = "Tipo")]
    public string TipoActividad
    {
        get
        {
            if (EsHoraEfectiva) return "Hora Efectiva";
            if (EsLicencia) return "Licencia";
            if (EsFranco) return "Franco";
            return "Otro";
        }
    }

    [Display(Name = "Badge Color")]
    public string BadgeColor
    {
        get
        {
            if (EsHoraEfectiva) return "success";
            if (EsLicencia) return "warning";
            if (EsFranco) return "info";
            return "secondary";
        }
    }
}
