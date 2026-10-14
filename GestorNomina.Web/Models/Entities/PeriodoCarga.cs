using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("periodos_carga")]
public class PeriodoCarga
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("proyecto_id")]
    public int ProyectoId { get; set; }

    [Required]
    [Column("mes")]
    public int Mes { get; set; }

    [Required]
    [Column("anio")]
    public int Anio { get; set; }

    [Column("quincena")]
    public int? Quincena { get; set; } // 1 o 2, null si es mensual completo

    [Required]
    [MaxLength(50)]
    [Column("estado")]
    public string Estado { get; set; } = "Pendiente";

    [Column("usuario_carga_id")]
    public int? UsuarioCargaId { get; set; }

    [MaxLength(500)]
    [Column("nombre_archivo_original")]
    public string? NombreArchivoOriginal { get; set; }

    [MaxLength(100)]
    [Column("hash_archivo")]
    public string? HashArchivo { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("ProyectoId")]
    public virtual Proyecto Proyecto { get; set; } = null!;

    [ForeignKey("UsuarioCargaId")]
    public virtual Usuario? UsuarioCarga { get; set; }

    public virtual ICollection<HoraRegistroRaw> HorasRegistrosRaw { get; set; } = new List<HoraRegistroRaw>();
    public virtual ICollection<HoraRegistroAprobado> HorasRegistrosAprobados { get; set; } = new List<HoraRegistroAprobado>();
}
