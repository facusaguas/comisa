using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

    [MaxLength(13)]
    [Column("cuil")]
    public string? Cuil { get; set; }

    [MaxLength(100)]
    [Column("cargo")]
    public string? Cargo { get; set; }

    [Column("regimen_id")]
    public int? RegimenId { get; set; }

    [Column("fecha_ingreso")]
    public DateTime? FechaIngreso { get; set; }

    [Column("fecha_alta")]
    public DateTime? FechaAlta { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ==========================================
    // CAMPOS DE LIQUIDACIÓN
    // ==========================================

    [Column("convenio_id")]
    public int? ConvenioId { get; set; }

    [Column("tipo_liquidacion_id")]
    public int? TipoLiquidacionId { get; set; }

    [Column("categoria_id")]
    public int? CategoriaId { get; set; }

    [Column("disciplina_id")]
    public int? DisciplinaId { get; set; }

    [Column("proyecto_id")]
    public int? ProyectoId { get; set; }

    [Column("valor_hora_basico")]
    [Precision(12, 4)]
    public decimal? ValorHoraBasico { get; set; }

    [Column("valor_convenido_diario")]
    [Precision(12, 4)]
    public decimal? ValorConvenidoDiario { get; set; }

    [Column("aplica_plus_altura")]
    public bool AplicaPlusAltura { get; set; } = false;

    [Column("aplica_plus_fenix")]
    public bool AplicaPlusFenix { get; set; } = false;

    // Navigation Properties - Existentes
    [ForeignKey("RegimenId")]
    public virtual RegimenLaboral? Regimen { get; set; }

    public virtual ICollection<EmpleadoProyecto> EmpleadosProyectos { get; set; } = new List<EmpleadoProyecto>();
    public virtual ICollection<HoraRegistroRaw> HorasRegistrosRaw { get; set; } = new List<HoraRegistroRaw>();
    public virtual ICollection<HoraRegistroAprobado> HorasRegistrosAprobados { get; set; } = new List<HoraRegistroAprobado>();

    // Navigation Properties - Liquidación
    [ForeignKey("ConvenioId")]
    public virtual Convenio? Convenio { get; set; }

    [ForeignKey("TipoLiquidacionId")]
    public virtual TipoLiquidacion? TipoLiquidacion { get; set; }

    [ForeignKey("CategoriaId")]
    public virtual Categoria? Categoria { get; set; }

    [ForeignKey("DisciplinaId")]
    public virtual Disciplina? Disciplina { get; set; }

    [ForeignKey("ProyectoId")]
    public virtual Proyecto? Proyecto { get; set; }

    public virtual ICollection<Recibo> Recibos { get; set; } = new List<Recibo>();
}
