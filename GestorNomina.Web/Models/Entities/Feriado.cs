using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("feriados")]
public class Feriado
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("tipo")]
    public string? Tipo { get; set; }

    [MaxLength(100)]
    [Column("jurisdiccion")]
    public string? Jurisdiccion { get; set; }
}
