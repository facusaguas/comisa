namespace GestorNomina.Web.Services.Liquidacion.Payroll;

/// <summary>
/// Interfaz para los pasos del pipeline de liquidación
/// </summary>
public interface IPayrollStep
{
    /// <summary>
    /// Nombre del paso
    /// </summary>
    string StepName { get; }

    /// <summary>
    /// Ejecuta el paso de liquidación
    /// </summary>
    /// <param name="context">Contexto de liquidación</param>
    Task ExecuteAsync(PayrollContext context);
}
