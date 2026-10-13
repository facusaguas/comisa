using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorNomina.Web.Models.Entities;

[Table("actividades")]
public class Actividad
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
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [Column("es_hora_efectiva")]
    public bool EsHoraEfectiva { get; set; } = true;

    [Column("es_licencia")]
    public bool EsLicencia { get; set; } = false;

    [Column("es_franco")]
    public bool EsFranco { get; set; } = false;

    // Navigation Properties
    public virtual ICollection<HoraRegistroRaw> HorasRegistrosRaw { get; set; } = new List<HoraRegistroRaw>();
    public virtual ICollection<HoraRegistroAprobado> HorasRegistrosAprobados { get; set; } = new List<HoraRegistroAprobado>();
    public virtual ICollection<ValidacionRegistro> ValidacionesRegistrosConActividadSugerida { get; set; } = new List<ValidacionRegistro>();
}
