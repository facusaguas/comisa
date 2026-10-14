using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 4: Deducciones y Retenciones de Ley
/// </summary>
public class StepDeducciones : IPayrollStep
{
    private const decimal TASA_JUBILACION = 0.11m; // 11%
    private const decimal TASA_LEY_19032 = 0.03m; // 3%
    private const decimal TASA_OBRA_SOCIAL = 0.03m; // 3%
    private const decimal TASA_CUOTA_SINDICAL = 0.025m; // 2.5%
    private const decimal IMPORTE_SEGURO_VIDA = 19989.90m; // Importe fijo

    public string StepName => "Deducciones";

    public Task ExecuteAsync(PayrollContext context)
    {
        var totalBruto = context.TotalBrutoRemunerativo;
        var valorHoraBasico = context.Empleado.ValorHoraBasico ?? 0;

        // 1. Jubilación 11% (5001)
        var jubilacion = totalBruto * TASA_JUBILACION;
        context.ItemsDeducciones.Add(new ItemLiquidacion
        {
            Concepto = CodigoConceptoTango.Jubilacion,
            Unidades = 1,
            ValorUnitario = jubilacion,
            Subtotal = jubilacion,
            Descripcion = "Jubilación 11%"
        });

        // 2. Ley 19.032 3% (5002)
        var ley19032 = totalBruto * TASA_LEY_19032;
        context.ItemsDeducciones.Add(new ItemLiquidacion
        {
            Concepto = CodigoConceptoTango.Ley19032,
            Unidades = 1,
            ValorUnitario = ley19032,
            Subtotal = ley19032,
            Descripcion = "Ley 19.032 3%"
        });

        // 3. Obra Social 3% (5003) - Con control de tope mínimo
        var obraSocialCalculada = totalBruto * TASA_OBRA_SOCIAL;
        
        // Calcular tope mínimo UOCRA: (176 * ValorHora) * 0.03
        var topeMinimoOS = (176 * valorHoraBasico) * TASA_OBRA_SOCIAL;
        
        // Ajustar al proporcional de la quincena (50% del tope mensual)
        var topeMinimoQuincenal = topeMinimoOS * 0.5m;
        
        var obraSocial = Math.Max(obraSocialCalculada, topeMinimoQuincenal);
        
        context.ItemsDeducciones.Add(new ItemLiquidacion
        {
            Concepto = CodigoConceptoTango.ObraSocial,
            Unidades = 1,
            ValorUnitario = obraSocial,
            Subtotal = obraSocial,
            Descripcion = "Obra Social 3% (con tope mínimo)"
        });

        // 4. Cuota Sindical 2.5% (5004)
        var cuotaSindical = totalBruto * TASA_CUOTA_SINDICAL;
        context.ItemsDeducciones.Add(new ItemLiquidacion
        {
            Concepto = CodigoConceptoTango.CuotaSindical,
            Unidades = 1,
            ValorUnitario = cuotaSindical,
            Subtotal = cuotaSindical,
            Descripcion = "Cuota Sindical 2.5%"
        });

        // 5. Seguro de Vida Obligatorio (5010) - Importe fijo
        context.ItemsDeducciones.Add(new ItemLiquidacion
        {
            Concepto = CodigoConceptoTango.SeguroDeVida,
            Unidades = 1,
            ValorUnitario = IMPORTE_SEGURO_VIDA,
            Subtotal = IMPORTE_SEGURO_VIDA,
            Descripcion = "Seguro de Vida Obligatorio"
        });

        // Calcular total deducciones
        context.TotalDeducciones = context.ItemsDeducciones.Sum(i => i.Subtotal);

        return Task.CompletedTask;
    }
}
