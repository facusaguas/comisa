using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

/// <summary>
/// Tipos de Liquidación (DIRECTOS, DIRECTOS c/acuerdo, INDIRECTO)
/// </summary>
[Table("tipos_liquidacion")]
public class TipoLiquidacion
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
