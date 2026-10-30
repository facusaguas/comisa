namespace GestorNomina.Web.Models.Enums;

/// <summary>
/// Códigos de conceptos de liquidación para Tango Gestión
/// </summary>
public enum CodigoConceptoTango
{
    // CONCEPTOS REMUNERATIVOS
    HorasNormales = 1001,
    HorasExtras50 = 1002,
    HorasExtras100 = 1003,
    HorasNocturnas = 1004,
    HorasNocturnas50 = 1005,
    HorasNocturnas100 = 1006,
    PlusAltura = 1010,
    Presentismo = 1020,
    AdicionalMontajeFenix = 1025,
    FeriadoLicencia = 1030,
    Enfermedad = 1031,
    CompensacionEspecial = 1033,
    BonificacionEspecialNeto = 1050,
    PlusElectrico = 1090,

    // CONCEPTOS NO REMUNERATIVOS
    PremioFinalObra = 1034,

    // DEDUCCIONES
    Jubilacion = 5001,
    Ley19032 = 5002,
    ObraSocial = 5003,
    CuotaSindical = 5004,
    SeguroDeVida = 5010,
    Embargo = 5011
}
