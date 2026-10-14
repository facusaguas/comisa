using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Detalle del Recibo de Liquidación
/// </summary>
[Table("recibo_detalles")]
public class ReciboDetalle
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("recibo_id")]
    public int ReciboId { get; set; }

    [Column("concepto_id")]
    public int ConceptoId { get; set; }

    [Column("unidades")]
    [Precision(8, 2)]
    public decimal Unidades { get; set; }

    [Column("valor_unitario")]
    [Precision(12, 4)]
    public decimal? ValorUnitario { get; set; }

    [Column("subtotal")]
    [Precision(12, 2)]
    public decimal Subtotal { get; set; }

    // Navigation Properties
    [ForeignKey("ReciboId")]
    public virtual Recibo Recibo { get; set; } = null!;

    [ForeignKey("ConceptoId")]
    public virtual ConceptoLiquidacion Concepto { get; set; } = null!;
}
