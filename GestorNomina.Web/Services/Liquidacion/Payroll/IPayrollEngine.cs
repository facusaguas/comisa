using GestorNomina.Web.Models.Entities;

namespace GestorNomina.Web.Services.Liquidacion.Payroll;

/// <summary>
/// Motor de liquidación de haberes
/// </summary>
public interface IPayrollEngine
{
    /// <summary>
    /// Procesa la liquidación de un empleado para un periodo y quincena
    /// </summary>
    /// <param name="empleadoId">ID del empleado</param>
    /// <param name="periodo">Periodo en formato YYYY-MM</param>
    /// <param name="quincena">Número de quincena (1 o 2)</param>
    /// <returns>Recibo generado</returns>
    Task<Recibo> ProcesarLiquidacionAsync(int empleadoId, string periodo, int quincena);

    /// <summary>
    /// Procesa la liquidación de todos los empleados activos para un periodo y quincena
    /// </summary>
    /// <param name="periodo">Periodo en formato YYYY-MM</param>
    /// <param name="quincena">Número de quincena (1 o 2)</param>
    /// <returns>Lista de recibos generados</returns>
    Task<List<Recibo>> ProcesarLiquidacionMasivaAsync(string periodo, int quincena);
}
