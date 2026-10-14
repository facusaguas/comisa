using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Catálogo de Conceptos de Liquidación (Tango)
/// </summary>
[Table("conceptos_liquidacion")]
public class ConceptoLiquidacion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("codigo_tango")]
    public string CodigoTango { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty; // REMUNERATIVO, NO_REMUNERATIVO, DEDUCCION

    // Navigation Properties
    public virtual ICollection<ReciboDetalle> ReciboDetalles { get; set; } = new List<ReciboDetalle>();
}
