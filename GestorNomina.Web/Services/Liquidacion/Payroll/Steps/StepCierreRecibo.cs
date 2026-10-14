namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 5: Cierre de Recibo y Control
/// </summary>
public class StepCierreRecibo : IPayrollStep
{
    public string StepName => "Cierre de Recibo";

    public Task ExecuteAsync(PayrollContext context)
    {
        // Calcular totales finales
        context.TotalBruto = context.TotalBrutoRemunerativo + context.TotalBrutoNoRemunerativo;
        context.TotalNeto = context.TotalBruto - context.TotalDeducciones;

        // Control de diferencia con neto convenido
        if (context.NetoConvenidoObjetivo.HasValue)
        {
            context.DiferenciaControl = context.NetoConvenidoObjetivo.Value - context.TotalNeto;
        }

        // Redondear neto hacia arriba
        context.NetoRedondeado = (int)Math.Ceiling(context.TotalNeto);

        return Task.CompletedTask;
    }
}
