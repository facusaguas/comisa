namespace GestorNomina.Web.Models.Enums;

/// <summary>
/// Tipos de novedades que pueden aplicarse a un empleado en liquidación
/// </summary>
public enum TipoNovedadLiquidacion
{
    Embargo = 1,
    InicioRoster = 2,
    FinRoster = 3,
    Enfermedad = 4,
    Baja = 5,
    Vacaciones = 6,
    Anticipo = 7
}
