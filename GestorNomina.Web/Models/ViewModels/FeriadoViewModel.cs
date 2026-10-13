using System.ComponentModel.DataAnnotations;

namespace GestorNomina.Web.Models.ViewModels;

/// <summary>
/// ViewModel para la gestión de feriados
/// </summary>
public class FeriadoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El tipo no puede exceder los 50 caracteres")]
    [Display(Name = "Tipo")]
    public string? Tipo { get; set; }

    [StringLength(100, ErrorMessage = "La jurisdicción no puede exceder los 100 caracteres")]
    [Display(Name = "Jurisdicción")]
    public string? Jurisdiccion { get; set; }

    // Propiedades de visualización
    [Display(Name = "Día de la Semana")]
    public string DiaSemana => Fecha.ToString("dddd");

    [Display(Name = "Mes")]
    public string Mes => Fecha.ToString("MMMM");

    [Display(Name = "Año")]
    public int Anio => Fecha.Year;

    [Display(Name = "Fecha Formateada")]
    public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");

    [Display(Name = "Badge Color")]
    public string BadgeColor
    {
        get
        {
            return Tipo?.ToLower() switch
            {
                "nacional" => "primary",
                "provincial" => "info",
                "municipal" => "secondary",
                "religioso" => "purple",
                _ => "secondary"
            };
        }
    }

    [Display(Name = "Es Pasado")]
    public bool EsPasado => Fecha < DateTime.Now.Date;
}
