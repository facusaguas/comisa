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

    /// <summary>
    /// Indica si este detalle fue editado manualmente por un usuario
    /// </summary>
    [Column("editado_manualmente")]
    public bool EditadoManualmente { get; set; }

    /// <summary>
    /// Valor original del subtotal antes de editar (para auditoría)
    /// </summary>
    [Column("valor_original")]
    [Precision(12, 2)]
    public decimal? ValorOriginal { get; set; }

    // Navigation Properties
    [ForeignKey("ReciboId")]
    public virtual Recibo Recibo { get; set; } = null!;

    [ForeignKey("ConceptoId")]
    public virtual ConceptoLiquidacion Concepto { get; set; } = null!;
}
