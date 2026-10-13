using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("validaciones_registro")]
public class ValidacionRegistro
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("registro_raw_id")]
    public int RegistroRawId { get; set; }

    [Required]
    [Column("regla_id")]
    public int ReglaId { get; set; }

    [Required]
    [MaxLength(1000)]
    [Column("mensaje_observacion")]
    public string MensajeObservacion { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Column("propuesta_sugerida")]
    public string? PropuestaSugerida { get; set; }

    [Column("valor_sugerido_horas")]
    public decimal? ValorSugeridoHoras { get; set; }

    [Column("actividad_sugerida_id")]
    public int? ActividadSugeridaId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("estado")]
    public string Estado { get; set; } = "Pendiente";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("RegistroRawId")]
    public virtual HoraRegistroRaw RegistroRaw { get; set; } = null!;

    [ForeignKey("ReglaId")]
    public virtual ReglaNormativa Regla { get; set; } = null!;

    [ForeignKey("ActividadSugeridaId")]
    public virtual Actividad? ActividadSugerida { get; set; }
}
