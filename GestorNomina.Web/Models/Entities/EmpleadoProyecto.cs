using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("empleado_proyecto")]
public class EmpleadoProyecto
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("empleado_id")]
    public int EmpleadoId { get; set; }

    [Required]
    [Column("proyecto_id")]
    public int ProyectoId { get; set; }

    [Column("fecha_desde")]
    public DateTime FechaDesde { get; set; }

    [Column("fecha_hasta")]
    public DateTime? FechaHasta { get; set; }

    [Column("regimen_especifico_id")]
    public int? RegimenEspecificoId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("EmpleadoId")]
    public virtual Empleado Empleado { get; set; } = null!;

    [ForeignKey("ProyectoId")]
    public virtual Proyecto Proyecto { get; set; } = null!;

    [ForeignKey("RegimenEspecificoId")]
    public virtual RegimenLaboral? RegimenEspecifico { get; set; }
}
