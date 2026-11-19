using System.ComponentModel.DataAnnotations;
using GestorNomina.Web.Models.Enums;

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

    [StringLength(13, ErrorMessage = "El CUIL debe tener 11 dígitos")]
    [Display(Name = "CUIL")]
    public string? Cuil { get; set; }

    [StringLength(200, ErrorMessage = "El domicilio no puede exceder los 200 caracteres")]
    [Display(Name = "Domicilio")]
    public string? Domicilio { get; set; }

    [Display(Name = "Provincia")]
    public int? ProvinciaId { get; set; }

    [Display(Name = "Ciudad")]
    public int? CiudadId { get; set; }

    [StringLength(100, ErrorMessage = "El cargo no puede exceder los 100 caracteres")]
    [Display(Name = "Cargo")]
    public string? Cargo { get; set; }

    [Display(Name = "Régimen Laboral")]
    public int? RegimenId { get; set; }

    [Display(Name = "Proyecto Actual")]
    public int? ProyectoId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Ingreso")]
    public DateTime? FechaIngreso { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Alta")]
    public DateTime? FechaAlta { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Baja")]
    public DateTime? FechaBaja { get; set; }

    [Display(Name = "Forma de Pago")]
    public FormaPago FormaPago { get; set; } = FormaPago.Quincenal;

    [Display(Name = "Activo")]
    public bool? Activo { get; set; } = true;

    // ==========================================
    // CAMPOS DE LIQUIDACIÓN
    // ==========================================

    [Display(Name = "Convenio")]
    public int? ConvenioId { get; set; }

    [Display(Name = "Tipo de Liquidación")]
    public int? TipoLiquidacionId { get; set; }

    [Display(Name = "Categoría")]
    public int? CategoriaId { get; set; }

    [Display(Name = "Disciplina")]
    public int? DisciplinaId { get; set; }

    [Display(Name = "Valor Hora Básico")]
    [DataType(DataType.Currency)]
    public decimal? ValorHoraBasico { get; set; }

    [Display(Name = "Valor Convenido Diario")]
    [DataType(DataType.Currency)]
    public decimal? ValorConvenidoDiario { get; set; }

    [Display(Name = "Aplica Plus Altura")]
    public bool AplicaPlusAltura { get; set; } = false;

    [Display(Name = "Aplica Plus Eléctrico")]
    public bool AplicaPlusElectrico { get; set; } = false;

    // ==========================================
    // PROPIEDADES DE VISUALIZACIÓN
    // ==========================================

    [Display(Name = "Nombre Completo")]
    public string NombreCompleto => $"{Apellido}, {Nombre}";

    [Display(Name = "Estado")]
    public string EstadoTexto => Activo == true ? "Activo" : (Activo == false ? "Inactivo" : "No definido");

    [Display(Name = "Forma de Pago")]
    public string FormaPagoTexto => FormaPago == FormaPago.Quincenal ? "Quincenal" : "Mensual";

    // Nombres para visualización
    public string? RegimenNombre { get; set; }
    public string? ProyectoNombre { get; set; }
    public string? ConvenioNombre { get; set; }
    public string? TipoLiquidacionNombre { get; set; }
    public string? CategoriaNombre { get; set; }
    public string? DisciplinaNombre { get; set; }
    public string? ProvinciaNombre { get; set; }
    public string? CiudadNombre { get; set; }
    public string? CodigoPostal { get; set; }

    // Proyectos asignados (para visualización)
    public List<string> ProyectosAsignados { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
