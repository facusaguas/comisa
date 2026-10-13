using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("horas_registros_raw")]
public class HoraRegistroRaw
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("periodo_carga_id")]
    public int PeriodoCargaId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("empleado_legajo")]
    public string EmpleadoLegajo { get; set; } = string.Empty;

    [Column("empleado_id")]
    public int? EmpleadoId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("actividad_codigo")]
    public string ActividadCodigo { get; set; } = string.Empty;

    [Column("actividad_id")]
    public int? ActividadId { get; set; }

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Required]
    [Column("horas_cargadas")]
    public decimal HorasCargadas { get; set; }

    [Column("fila_excel_origen")]
    public int? FilaExcelOrigen { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("PeriodoCargaId")]
    public virtual PeriodoCarga PeriodoCarga { get; set; } = null!;

    [ForeignKey("EmpleadoId")]
    public virtual Empleado? Empleado { get; set; }

    [ForeignKey("ActividadId")]
    public virtual Actividad? Actividad { get; set; }

    public virtual ICollection<ValidacionRegistro> ValidacionesRegistros { get; set; } = new List<ValidacionRegistro>();
    public virtual HoraRegistroAprobado? HoraRegistroAprobado { get; set; }
}
