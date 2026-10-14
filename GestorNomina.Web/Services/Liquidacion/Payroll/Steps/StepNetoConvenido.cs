using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 3: Motor de Igualmente y Grossing-Up (Neto Convenido)
/// </summary>
public class StepNetoConvenido : IPayrollStep
{
    private readonly ICatalogResolver _catalogResolver;
    private const decimal COEF_RETENCIONES_LEY = 0.195m; // 11% + 3% + 3% + 2.5%
    private const decimal COEF_GROSSING_UP = 1.24223572358584m; // Factor de ajuste bruto

    public string StepName => "Neto Convenido (Grossing-Up)";

    public StepNetoConvenido(ICatalogResolver catalogResolver)
    {
        _catalogResolver = catalogResolver;
    }

    public Task ExecuteAsync(PayrollContext context)
    {
        // Verificar si aplica neto convenido
        var tipoConvenido = _catalogResolver.GetTipoLiquidacionId(CodigoTipoLiquidacion.DIRECTOS_CONVERSION_ACUERDO);
        var esConvenido = context.Empleado.TipoLiquidacionId == tipoConvenido;

        if (!esConvenido || !context.Empleado.ValorConvenidoDiario.HasValue || context.Empleado.ValorConvenidoDiario <= 0)
        {
            return Task.CompletedTask;
        }

        // 1. Calcular Neto Objetivo
        var netoObjetivo = context.Empleado.ValorConvenidoDiario.Value * context.DIAS;
        context.NetoConvenidoObjetivo = netoObjetivo;

        // 2. Calcular Neto Provisorio (antes de bonificación)
        var brutoParcial = context.TotalBrutoRemunerativo;
        var netoProvisorio = brutoParcial * (1 - COEF_RETENCIONES_LEY);

        // 3. Calcular Bonificación Especial si es necesario
        if (netoObjetivo > netoProvisorio)
        {
            var diferenciaNeto = netoObjetivo - netoProvisorio;
            var ajusteBruto = diferenciaNeto * COEF_GROSSING_UP;

            // Agregar la bonificación como ítem remunerativo
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.BonificacionEspecialNeto,
                Unidades = 1,
                ValorUnitario = ajusteBruto,
                Subtotal = ajusteBruto,
                Descripcion = "Bonificación Especial - Ajuste Neto Convenido"
            });

            // Recalcular total bruto remunerativo
            context.TotalBrutoRemunerativo = context.ItemsRemunerativos.Sum(i => i.Subtotal);
        }

        return Task.CompletedTask;
    }
}
