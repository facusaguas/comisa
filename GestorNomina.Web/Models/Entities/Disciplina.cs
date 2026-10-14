using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Disciplinas / Especialidades (OPERADOR, MECANICO, MONTADOR, PREVENCIONISTA)
/// </summary>
[Table("disciplinas")]
public class Disciplina
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    // Navigation Properties
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
