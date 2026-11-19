using GestorNomina.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Data;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 2: Conceptos Remunerativos Básico y Convenio
/// </summary>
public class StepConceptosRemunerativos : IPayrollStep
{
    private readonly ApplicationDbContext? _context;

    public string StepName => "Conceptos Remunerativos";

    public StepConceptosRemunerativos()
    {
        // Constructor sin parámetros para compatibilidad
    }

    public StepConceptosRemunerativos(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync(PayrollContext context)
    {
        // Determinar el valor hora según el tipo de liquidación
        // TODOS los tipos ahora usan Categoria.SalarioBasico
        decimal valorHora;

        if (context.Empleado.Categoria?.SalarioBasico == null || context.Empleado.Categoria.SalarioBasico <= 0)
        {
            throw new InvalidOperationException(
                $"El empleado {context.Empleado.Legajo} no tiene configurado el salario básico en su categoría");
        }

        valorHora = context.Empleado.Categoria.SalarioBasico.Value;

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

        // 8. Plus Eléctrico 15% (1090): SueldoBasico * 0.15
        if (context.Empleado.AplicaPlusElectrico && sueldoBasico > 0)
        {
            var plusElectrico = sueldoBasico * 0.15m;
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.PlusElectrico,
                Unidades = 1,
                ValorUnitario = plusElectrico,
                Subtotal = plusElectrico,
                Descripcion = "Plus Eléctrico 15%"
            });
        }

        // 9. Presentismo UOCRA 20% (1020): (SueldoBasico + Nocturnas) * 0.20
        // Ya no requiere ausencias = 0, se paga siempre
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

        // 10. Adicional Montaje UOCRA (Fenix) 25% (1025): (SueldoBasico + Nocturnas) * 0.25
        var baseAdicional = sueldoBasico + hsNocturnas;
        if (baseAdicional > 0)
        {
            var adicionalMontaje = baseAdicional * 0.25m;
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.AdicionalMontajeFenix,
                Unidades = 1,
                ValorUnitario = adicionalMontaje,
                Subtotal = adicionalMontaje,
                Descripcion = "Adicional Montaje UOCRA (Fenix) 25%"
            });
        }

        // 11. Compensación Especial (1033): ValorHora * horas * 1.45
        // Se suma los días de la quincena actual y la inmediata anterior
        var tieneFinRoster = context.Novedades.Any(n => n.TipoNovedad == TipoNovedadLiquidacion.FinRoster);
        if (tieneFinRoster)
        {
            decimal diasRoster = context.DIAS;
            
            if (context.Quincena == 2 && _context != null)
            {
                // Si es quincena 2, buscar quincena 1 del mismo mes
                var reciboQuincena1 = await _context.Recibos
                    .FirstOrDefaultAsync(r => r.EmpleadoId == context.Empleado.Id
                        && r.Periodo == context.Periodo
                        && r.Quincena == 1);
                
                if (reciboQuincena1 != null)
                {
                    diasRoster += reciboQuincena1.DiasTrabajados;
                }
            }
            else if (context.Quincena == 1 && _context != null)
            {
                // Si es quincena 1, buscar quincena 2 del mes anterior
                // Periodo formato: "2024-11-01"
                if (DateTime.TryParse(context.Periodo, out var periodoDate))
                {
                    var periodoAnterior = periodoDate.AddMonths(-1).ToString("yyyy-MM-dd");
                    var reciboQuincena2MesAnterior = await _context.Recibos
                        .FirstOrDefaultAsync(r => r.EmpleadoId == context.Empleado.Id
                            && r.Periodo == periodoAnterior
                            && r.Quincena == 2);
                    
                    if (reciboQuincena2MesAnterior != null)
                    {
                        diasRoster += reciboQuincena2MesAnterior.DiasTrabajados;
                    }
                }
            }

            // Calcular horas según la lógica especificada
            decimal horas;
            if (diasRoster >= 14 && diasRoster <= 20)
            {
                horas = 40;
            }
            else
            {
                horas = (diasRoster * 40) / 20;
            }

            var compensacion = valorHora * horas * 1.45m;
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.CompensacionEspecial,
                Unidades = 1,
                ValorUnitario = compensacion,
                Subtotal = compensacion,
                Descripcion = $"Compensación Especial ({diasRoster} días - {horas:N2} hs)"
            });

            // 12. Premio Final de Obra (1034): Valor fijo del proyecto (NO REMUNERATIVO)
            // Solo se paga junto con la compensación especial
            if (context.Empleado.Proyecto?.PremioFinalObra != null && context.Empleado.Proyecto.PremioFinalObra > 0)
            {
                var premioObra = context.Empleado.Proyecto.PremioFinalObra.Value;
                context.ItemsNoRemunerativos.Add(new ItemLiquidacion
                {
                    Concepto = CodigoConceptoTango.PremioFinalObra,
                    Unidades = 1,
                    ValorUnitario = premioObra,
                    Subtotal = premioObra,
                    Descripcion = "Premio Final de Obra (No Remunerativo)"
                });
            }
        }

        // 13. Feriados / Licencias (1030): DiasFeriado * (ValorHora * 8)
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

        // 14. Enfermedad / Licencias (1031): DiasEnfermo * (ValorHora * 8)
        var diasEnfermedad = context.Novedades
            .Where(n => n.TipoNovedad == TipoNovedadLiquidacion.Enfermedad)
            .Count();
        
        if (diasEnfermedad > 0)
        {
            var valorDiaEnfermedad = valorHora * 8;
            var totalEnfermedad = diasEnfermedad * valorDiaEnfermedad;
            context.ItemsRemunerativos.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.Enfermedad,
                Unidades = diasEnfermedad,
                ValorUnitario = valorDiaEnfermedad,
                Subtotal = totalEnfermedad,
                Descripcion = "Enfermedad / Licencias"
            });
        }

        // Calcular total bruto remunerativo parcial (antes de bonificación)
        context.TotalBrutoRemunerativo = context.ItemsRemunerativos.Sum(i => i.Subtotal);
        
        // Calcular total bruto no remunerativo
        context.TotalBrutoNoRemunerativo = context.ItemsNoRemunerativos.Sum(i => i.Subtotal);
    }
}
