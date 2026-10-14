using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Catálogo de Novedades del Parte Diario (NORMAL, FERIADO, ENFERMEDAD, VACACIONES, ROSTER, etc.)
/// </summary>
[Table("tipos_novedad")]
public class TipoNovedad
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("requiere_horas")]
    public bool RequiereHoras { get; set; } = true;

    [MaxLength(10)]
    [Column("color_hex_default")]
    public string? ColorHexDefault { get; set; }
}
