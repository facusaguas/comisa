using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Tabla de Recibos de Liquidación
/// </summary>
[Table("recibos")]
public class Recibo
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("empleado_id")]
    public int EmpleadoId { get; set; }

    [Required]
    [MaxLength(7)]
    [Column("periodo")]
    public string Periodo { get; set; } = string.Empty;

    [Column("quincena")]
    public int Quincena { get; set; }

    [Column("dias_trabajados")]
    [Precision(6, 2)]
    public decimal DiasTrabajados { get; set; }

    [Column("total_bruto")]
    [Precision(12, 2)]
    public decimal TotalBruto { get; set; }

    [Column("total_deducciones")]
    [Precision(12, 2)]
    public decimal TotalDeducciones { get; set; }

    [Column("total_neto")]
    [Precision(12, 2)]
    public decimal TotalNeto { get; set; }

    [Column("neto_convenido_objetivo")]
    [Precision(12, 2)]
    public decimal? NetoConvenidoObjetivo { get; set; }

    [Column("diferencia_control")]
    [Precision(12, 4)]
    public decimal? DiferenciaControl { get; set; }

    [Column("neto_redondeado")]
    public int NetoRedondeado { get; set; }

    [Column("fecha_calculo")]
    public DateTime FechaCalculo { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("EmpleadoId")]
    public virtual Empleado Empleado { get; set; } = null!;

    public virtual ICollection<ReciboDetalle> Detalles { get; set; } = new List<ReciboDetalle>();
}
