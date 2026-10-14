using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 1: Agregador de Horas - Analiza los registros y calcula totalizadores
/// Nota: Por ahora todas las horas se consideran diurnas. Si se necesita distinguir
/// horas nocturnas, agregar campo "es_nocturna" a la tabla actividades.
/// </summary>
public class StepAgregadorHoras : IPayrollStep
{
    private readonly ICatalogResolver _catalogResolver;
    private const decimal DIVISOR_JORNADA = 11m; // Horas por día para cálculo de días trabajados

    public string StepName => "Agregador de Horas";

    public StepAgregadorHoras(ICatalogResolver catalogResolver)
    {
        _catalogResolver = catalogResolver;
    }

    public Task ExecuteAsync(PayrollContext context)
    {
        // Agrupar por fecha para análisis diario
        var registrosPorFecha = context.Registros
            .GroupBy(r => r.Fecha.Date)
            .OrderBy(g => g.Key)
            .ToList();

        decimal totalHoras = 0;

        foreach (var grupo in registrosPorFecha)
        {
            var fecha = grupo.Key;
            var registrosDia = grupo.ToList();

            // Sumar horas del día
            var horasTotalesDia = registrosDia.Sum(r => r.HorasFinales);

            // Verificar tipo de día
            var diaSemana = fecha.DayOfWeek;
            var esSabado = diaSemana == DayOfWeek.Saturday;
            var esDomingo = diaSemana == DayOfWeek.Sunday;
            
            // TODO: Implementar lógica de feriados (consultar tabla feriados)
            // var esFeriado = ...

            if (esDomingo)
            {
                // Domingo: todo es HS100
                context.HS100 += horasTotalesDia;
                totalHoras += horasTotalesDia;
                continue;
            }

            if (esSabado)
            {
                // Sábado: hasta 4hs son HSEXT (50%), el resto HS100
                if (horasTotalesDia <= 4)
                {
                    context.HSEXT += horasTotalesDia;
                }
                else
                {
                    context.HSEXT += 4;
                    context.HS100 += (horasTotalesDia - 4);
                }

                totalHoras += horasTotalesDia;
                continue;
            }

            // Lunes a Viernes
            if (horasTotalesDia <= 8)
            {
                context.HSNOR += horasTotalesDia;
            }
            else
            {
                context.HSNOR += 8;
                context.HSEXT += (horasTotalesDia - 8);
            }

            totalHoras += horasTotalesDia;
        }

        // Calcular días trabajados
        context.DIAS = totalHoras > 0 ? totalHoras / DIVISOR_JORNADA : 0;

        return Task.CompletedTask;
    }
}
