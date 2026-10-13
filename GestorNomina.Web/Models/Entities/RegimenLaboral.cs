using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("regimenes_laborales")]
public class RegimenLaboral
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("dias_trabajo")]
    public int DiasTrabajo { get; set; }

    [Column("dias_descanso")]
    public int DiasDescanso { get; set; }

    [Column("max_horas_diarias")]
    public decimal MaxHorasDiarias { get; set; }

    [Column("horas_semanales_limite")]
    public decimal? HorasSemanalesLimite { get; set; }

    [MaxLength(1000)]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    // Navigation Properties
    public virtual ICollection<Proyecto> ProyectosConRegimenDefault { get; set; } = new List<Proyecto>();
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    public virtual ICollection<EmpleadoProyecto> EmpleadosProyectosConRegimenEspecifico { get; set; } = new List<EmpleadoProyecto>();
}
