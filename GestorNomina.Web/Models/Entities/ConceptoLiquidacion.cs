using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Catálogo de Conceptos de Liquidación
/// </summary>
[Table("conceptos_liquidacion")]
public class ConceptoLiquidacion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Column("tipo_concepto")]
    public TipoConcepto TipoConcepto { get; set; }

    // Navigation Properties
    public virtual ICollection<ReciboDetalle> ReciboDetalles { get; set; } = new List<ReciboDetalle>();
}
