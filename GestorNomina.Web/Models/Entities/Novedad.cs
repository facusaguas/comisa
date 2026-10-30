using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Models.Entities;

[Table("novedades")]
public class Novedad
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("empleado_id")]
    public int EmpleadoId { get; set; }

    [Required]
    [Column("periodo")]
    public DateTime Periodo { get; set; }

    [Required]
    [Column("quincena")]
    public int Quincena { get; set; }

    [Required]
    [Column("tipo_novedad")]
    public TipoNovedadLiquidacion TipoNovedad { get; set; }

    [Required]
    [Column("fecha_novedad")]
    public DateTime FechaNovedad { get; set; }

    [Column("monto_novedad", TypeName = "decimal(18,2)")]
    public decimal? MontoNovedad { get; set; }

    [Column("observaciones")]
    [MaxLength(500)]
    public string? Observaciones { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("EmpleadoId")]
    public virtual Empleado Empleado { get; set; } = null!;
}
