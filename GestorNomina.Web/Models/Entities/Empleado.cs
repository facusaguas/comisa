using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("empleados")]
public class Empleado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("legajo")]
    public string Legajo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("documento_identidad")]
    public string? DocumentoIdentidad { get; set; }

    [MaxLength(100)]
    [Column("cargo")]
    public string? Cargo { get; set; }

    [Column("regimen_id")]
    public int? RegimenId { get; set; }

    [Column("fecha_ingreso")]
    public DateTime? FechaIngreso { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("RegimenId")]
    public virtual RegimenLaboral? Regimen { get; set; }

    public virtual ICollection<EmpleadoProyecto> EmpleadosProyectos { get; set; } = new List<EmpleadoProyecto>();
    public virtual ICollection<HoraRegistroRaw> HorasRegistrosRaw { get; set; } = new List<HoraRegistroRaw>();
    public virtual ICollection<HoraRegistroAprobado> HorasRegistrosAprobados { get; set; } = new List<HoraRegistroAprobado>();
}
