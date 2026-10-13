using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("proyectos")]
public class Proyecto
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("ubicacion")]
    public string? Ubicacion { get; set; }

    [Column("regimen_default_id")]
    public int? RegimenDefaultId { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("RegimenDefaultId")]
    public virtual RegimenLaboral? RegimenDefault { get; set; }

    public virtual ICollection<EmpleadoProyecto> EmpleadosProyectos { get; set; } = new List<EmpleadoProyecto>();
    public virtual ICollection<PeriodoCarga> PeriodosCargas { get; set; } = new List<PeriodoCarga>();
}
