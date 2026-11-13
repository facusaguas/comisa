namespace GestorNomina.Web.Models.Enums;

/// <summary>
/// Tipo de concepto de liquidación
/// Los valores deben coincidir con la base de datos:
/// 1 = REMUNERATIVO
/// 2 = DEDUCCION  
/// 3 = NO_REMUNERATIVO
/// </summary>
public enum TipoConcepto
{
    REMUNERATIVO = 1,
    DEDUCCION = 2,
    NO_REMUNERATIVO = 3
}
