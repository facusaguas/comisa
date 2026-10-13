using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("reglas_normativas")]
public class ReglaNormativa
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [MaxLength(50)]
    [Column("nivel_severidad")]
    public string? NivelSeveridad { get; set; }

    [Column("activa")]
    public bool Activa { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<ValidacionRegistro> ValidacionesRegistros { get; set; } = new List<ValidacionRegistro>();
}
