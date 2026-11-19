using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Helper para calcular conceptos que dependen del sueldo base
/// Permite recalcular después de ajustes en el StepNetoConvenido
/// </summary>
public class ConceptosCalculator
{
    private readonly ApplicationDbContext? _context;

    public ConceptosCalculator(ApplicationDbContext? context = null)
    {
        _context = context;
    }

    /// <summary>
    /// Calcula el sueldo básico (horas normales)
    /// Usa las horas del contexto que ya fueron ajustadas en StepNetoConvenido
    /// </summary>
    public decimal CalcularSueldoBasico(PayrollContext context, decimal valorHora)
    {
        return context.HSNOR * valorHora;
    }

    /// <summary>
    /// Calcula horas nocturnas normales
    /// Usa las horas del contexto
    /// </summary>
    public decimal CalcularHorasNocturnas(PayrollContext context, decimal valorHora)
    {
        return context.HSNOCT * valorHora * 1.13m;
    }

    /// <summary>
    /// Recalcula todos los conceptos que dependen del sueldo base
    /// Debe llamarse después de cualquier ajuste de horas en StepNetoConvenido
    /// </summary>
    public async Task RecalcularConceptosDependientesAsync(PayrollContext context, decimal valorHora)
    {
        // Calcular nuevo sueldo base y nocturnas
        var sueldoBasico = CalcularSueldoBasico(context, valorHora);
        var hsNocturnas = CalcularHorasNocturnas(context, valorHora);

        // Buscar y actualizar/agregar conceptos dependientes

        // 1. Plus Altura 15%
        if (context.Empleado.AplicaPlusAltura && sueldoBasico > 0)
        {
            var plusAltura = sueldoBasico * 0.15m;
            ActualizarOAgregarItem(
                context.ItemsRemunerativos,
                CodigoConceptoTango.PlusAltura,
                1,
                plusAltura,
                plusAltura,
                "Plus Altura 15%"
            );
        }
        else
        {
            EliminarItem(context.ItemsRemunerativos, CodigoConceptoTango.PlusAltura);
        }

        // 2. Plus Eléctrico 15%
        if (context.Empleado.AplicaPlusElectrico && sueldoBasico > 0)
        {
            var plusElectrico = sueldoBasico * 0.15m;
            ActualizarOAgregarItem(
                context.ItemsRemunerativos,
                CodigoConceptoTango.PlusElectrico,
                1,
                plusElectrico,
                plusElectrico,
                "Plus Eléctrico 15%"
            );
        }
        else
        {
            EliminarItem(context.ItemsRemunerativos, CodigoConceptoTango.PlusElectrico);
        }

        // 3. Presentismo UOCRA 20%
        var basePresent = sueldoBasico + hsNocturnas;
        if (basePresent > 0)
        {
            var presentismo = basePresent * 0.20m;
            ActualizarOAgregarItem(
                context.ItemsRemunerativos,
                CodigoConceptoTango.Presentismo,
                1,
                presentismo,
                presentismo,
                "Presentismo UOCRA 20%"
            );
        }
        else
        {
            EliminarItem(context.ItemsRemunerativos, CodigoConceptoTango.Presentismo);
        }

        // 4. Adicional Montaje UOCRA (Fenix) 25%
        var baseAdicional = sueldoBasico + hsNocturnas;
        if (baseAdicional > 0)
        {
            var adicionalMontaje = baseAdicional * 0.25m;
            ActualizarOAgregarItem(
                context.ItemsRemunerativos,
                CodigoConceptoTango.AdicionalMontajeFenix,
                1,
                adicionalMontaje,
                adicionalMontaje,
                "Adicional Montaje UOCRA (Fenix) 25%"
            );
        }
       
        // 5. Compensación Especial (1033)
        // Se suma los días de la quincena actual y la inmediata anterior
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

        var tieneFinRoster = context.Novedades.Any(n => n.TipoNovedad == TipoNovedadLiquidacion.FinRoster);
        if (tieneFinRoster)
        {
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
            ActualizarOAgregarItem(
                context.ItemsRemunerativos,
                CodigoConceptoTango.CompensacionEspecial,
                1,
                compensacion,
                compensacion,
                $"Compensación Especial ({diasRoster} días - {horas:N2} hs)"
            );

            // Premio Final de Obra (NO REMUNERATIVO - no cambia con horas)
            if (context.Empleado.Proyecto?.PremioFinalObra != null && context.Empleado.Proyecto.PremioFinalObra > 0)
            {
                var premioObra = context.Empleado.Proyecto.PremioFinalObra.Value;
                ActualizarOAgregarItem(
                    context.ItemsNoRemunerativos,
                    CodigoConceptoTango.PremioFinalObra,
                    1,
                    premioObra,
                    premioObra,
                    "Premio Final de Obra (No Remunerativo)"
                );
            }
        }
        else
        {
            EliminarItem(context.ItemsRemunerativos, CodigoConceptoTango.CompensacionEspecial);
            EliminarItem(context.ItemsNoRemunerativos, CodigoConceptoTango.PremioFinalObra);
        }

        // 6. Feriados / Licencias
        if (context.DiasFeriado > 0)
        {
            var valorDiaFeriado = valorHora * 8;
            var totalFeriados = context.DiasFeriado * valorDiaFeriado;
            ActualizarOAgregarItem(
                context.ItemsRemunerativos,
                CodigoConceptoTango.FeriadoLicencia,
                context.DiasFeriado,
                valorDiaFeriado,
                totalFeriados,
                "Feriados / Licencias"
            );
        }
        else
        {
            EliminarItem(context.ItemsRemunerativos, CodigoConceptoTango.FeriadoLicencia);
        }

        // 7. Enfermedad / Licencias
        var diasEnfermedad = context.Novedades
            .Where(n => n.TipoNovedad == TipoNovedadLiquidacion.Enfermedad)
            .Count();
        
        if (diasEnfermedad > 0)
        {
            var valorDiaEnfermedad = valorHora * 8;
            var totalEnfermedad = diasEnfermedad * valorDiaEnfermedad;
            ActualizarOAgregarItem(
                context.ItemsRemunerativos,
                CodigoConceptoTango.Enfermedad,
                diasEnfermedad,
                valorDiaEnfermedad,
                totalEnfermedad,
                "Enfermedad / Licencias"
            );
        }
        else
        {
            EliminarItem(context.ItemsRemunerativos, CodigoConceptoTango.Enfermedad);
        }

        // Recalcular totales
        context.TotalBrutoRemunerativo = context.ItemsRemunerativos.Sum(i => i.Subtotal);
        context.TotalBrutoNoRemunerativo = context.ItemsNoRemunerativos.Sum(i => i.Subtotal);
    }

    private void ActualizarOAgregarItem(
        List<ItemLiquidacion> items,
        CodigoConceptoTango concepto,
        decimal unidades,
        decimal valorUnitario,
        decimal subtotal,
        string descripcion)
    {
        var item = items.FirstOrDefault(i => i.Concepto == concepto);
        if (item != null)
        {
            item.Unidades = unidades;
            item.ValorUnitario = valorUnitario;
            item.Subtotal = subtotal;
        }
        else
        {
            items.Add(new ItemLiquidacion
            {
                Concepto = concepto,
                Unidades = unidades,
                ValorUnitario = valorUnitario,
                Subtotal = subtotal,
                Descripcion = descripcion
            });
        }
    }

    private void EliminarItem(List<ItemLiquidacion> items, CodigoConceptoTango concepto)
    {
        var item = items.FirstOrDefault(i => i.Concepto == concepto);
        if (item != null)
        {
            items.Remove(item);
        }
    }
}
