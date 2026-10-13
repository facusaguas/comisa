using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("usuarios")]
public class Usuario
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("rol_id")]
    public int RolId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("RolId")]
    public virtual Rol Rol { get; set; } = null!;

    public virtual ICollection<PeriodoCarga> PeriodosCargas { get; set; } = new List<PeriodoCarga>();
    public virtual ICollection<HoraRegistroAprobado> HorasRegistrosModificados { get; set; } = new List<HoraRegistroAprobado>();
}
