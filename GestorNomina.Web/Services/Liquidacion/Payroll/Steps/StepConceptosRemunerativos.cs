using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 2: Conceptos Remunerativos Básico y Convenio
/// </summary>
public class StepConceptosRemunerativos : IPayrollStep
{
    public string StepName => "Conceptos Remunerativos";

    public Task ExecuteAsync(PayrollContext context)
    {
        var valorHora = context.Empleado.ValorHoraBasico ?? 0;

        // 1. Sueldo Básico (1001): HSNOR * ValorHora
        var sueldoBasico = context.HSNOR * valorHora;
        if (sueldoBasico > 0)
        {
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.HorasNormales,
                Unidades = context.HSNOR,
                ValorUnitario = valorHora,
                Subtotal = sueldoBasico,
                Descripcion = "Horas Normales"
            });
        }

        // 2. Horas Extras 50% (1002): HSEXT * ValorHora * 1.5 * 1.45
        var hsExtras50 = context.HSEXT * valorHora * 1.5m * 1.45m;
        if (hsExtras50 > 0)
        {
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.HorasExtras50,
                Unidades = context.HSEXT,
                ValorUnitario = valorHora * 1.5m * 1.45m,
                Subtotal = hsExtras50,
                Descripcion = "Horas Extras 50%"
            });
        }

        // 3. Horas Extras 100% (1003): HS100 * ValorHora * 2.0 * 1.45
        var hsExtras100 = context.HS100 * valorHora * 2.0m * 1.45m;
        if (hsExtras100 > 0)
        {
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.HorasExtras100,
                Unidades = context.HS100,
                ValorUnitario = valorHora * 2.0m * 1.45m,
                Subtotal = hsExtras100,
                Descripcion = "Horas Extras 100%"
            });
        }

        // 4. Horas Nocturnas Normales (1004): HSNOCT * ValorHora * 1.13
        var hsNocturnas = context.HSNOCT * valorHora * 1.13m;
        if (hsNocturnas > 0)
        {
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.HorasNocturnas,
                Unidades = context.HSNOCT,
                ValorUnitario = valorHora * 1.13m,
                Subtotal = hsNocturnas,
                Descripcion = "Horas Nocturnas"
            });
        }

        // 5. Horas Nocturnas Extras 50% (1005): HSNOCT50 * ValorHora * 1.13 * 1.5 * 1.45
        var hsNocturnas50 = context.HSNOCT50 * valorHora * 1.13m * 1.5m * 1.45m;
        if (hsNocturnas50 > 0)
        {
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.HorasNocturnas50,
                Unidades = context.HSNOCT50,
                ValorUnitario = valorHora * 1.13m * 1.5m * 1.45m,
                Subtotal = hsNocturnas50,
                Descripcion = "Horas Nocturnas Extras 50%"
            });
        }

        // 6. Horas Nocturnas Extras 100% (1006): HSNOCT100 * ValorHora * 1.13 * 2.0 * 1.45
        var hsNocturnas100 = context.HSNOCT100 * valorHora * 1.13m * 2.0m * 1.45m;
        if (hsNocturnas100 > 0)
        {
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.HorasNocturnas100,
                Unidades = context.HSNOCT100,
                ValorUnitario = valorHora * 1.13m * 2.0m * 1.45m,
                Subtotal = hsNocturnas100,
                Descripcion = "Horas Nocturnas Extras 100%"
            });
        }

        // 7. Plus Altura 15% (1010): SueldoBasico * 0.15
        if (context.Empleado.AplicaPlusAltura && sueldoBasico > 0)
        {
            var plusAltura = sueldoBasico * 0.15m;
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.PlusAltura,
                Unidades = 1,
                ValorUnitario = plusAltura,
                Subtotal = plusAltura,
                Descripcion = "Plus Altura 15%"
            });
        }

        // 8. Presentismo UOCRA 20% (1020): (SueldoBasico + Nocturnas) * 0.20
        // Solo si no hay ausencias injustificadas
        if (context.DiasAusenciasInjustificadas == 0)
        {
            var basePresent = sueldoBasico + hsNocturnas;
            if (basePresent > 0)
            {
                var presentismo = basePresent * 0.20m;
                context.ItemsRemunerativos.Add(new ItemLiquidacion
                {
                    Concepto = CodigoConceptoTango.Presentismo,
                    Unidades = 1,
                    ValorUnitario = presentismo,
                    Subtotal = presentismo,
                    Descripcion = "Presentismo UOCRA 20%"
                });
            }
        }

        // 9. Adicional Montaje / Fénix 25% (1025): (SueldoBasico + Nocturnas) * 0.25
        if (context.Empleado.AplicaPlusFenix)
        {
            var baseFenix = sueldoBasico + hsNocturnas;
            if (baseFenix > 0)
            {
                var adicionalFenix = baseFenix * 0.25m;
                context.ItemsRemunerativos.Add(new ItemLiquidacion
                {
                    Concepto = CodigoConceptoTango.AdicionalMontajeFenix,
                    Unidades = 1,
                    ValorUnitario = adicionalFenix,
                    Subtotal = adicionalFenix,
                    Descripcion = "Adicional Montaje Fénix 25%"
                });
            }
        }

        // 10. Feriados / Licencias (1030): DiasFeriado * (ValorHora * 8)
        if (context.DiasFeriado > 0)
        {
            var valorDiaFeriado = valorHora * 8;
            var totalFeriados = context.DiasFeriado * valorDiaFeriado;
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.FeriadoLicencia,
                Unidades = context.DiasFeriado,
                ValorUnitario = valorDiaFeriado,
                Subtotal = totalFeriados,
                Descripcion = "Feriados / Licencias"
            });
        }

        // Calcular total bruto remunerativo parcial (antes de bonificación)
        context.TotalBrutoRemunerativo = context.ItemsRemunerativos.Sum(i => i.Subtotal);

        return Task.CompletedTask;
    }
}
