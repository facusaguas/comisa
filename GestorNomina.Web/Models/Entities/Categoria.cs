using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Categorías Profesionales (OFICIAL ESPECIALIZADO, SUPERVISOR, etc.)
/// </summary>
[Table("categorias")]
public class Categoria
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

    [Column("convenio_id")]
    public int? ConvenioId { get; set; }

    // Navigation Properties
    [ForeignKey("ConvenioId")]
    public virtual Convenio? Convenio { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
