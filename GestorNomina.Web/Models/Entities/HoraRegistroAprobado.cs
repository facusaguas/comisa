using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("horas_registros_aprobados")]
public class HoraRegistroAprobado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("registro_raw_id")]
    public int RegistroRawId { get; set; }

    [Required]
    [Column("periodo_carga_id")]
    public int PeriodoCargaId { get; set; }

    [Required]
    [Column("empleado_id")]
    public int EmpleadoId { get; set; }

    [Required]
    [Column("actividad_id")]
    public int ActividadId { get; set; }

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Required]
    [Column("horas_finales")]
    public decimal HorasFinales { get; set; }

    [Column("fue_modificado")]
    public bool FueModificado { get; set; } = false;

    [MaxLength(1000)]
    [Column("motivo_modificacion")]
    public string? MotivoModificacion { get; set; }

    [Column("usuario_modificador_id")]
    public int? UsuarioModificadorId { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("RegistroRawId")]
    public virtual HoraRegistroRaw RegistroRaw { get; set; } = null!;

    [ForeignKey("PeriodoCargaId")]
    public virtual PeriodoCarga PeriodoCarga { get; set; } = null!;

    [ForeignKey("EmpleadoId")]
    public virtual Empleado Empleado { get; set; } = null!;

    [ForeignKey("ActividadId")]
    public virtual Actividad Actividad { get; set; } = null!;

    [ForeignKey("UsuarioModificadorId")]
    public virtual Usuario? UsuarioModificador { get; set; }
}
