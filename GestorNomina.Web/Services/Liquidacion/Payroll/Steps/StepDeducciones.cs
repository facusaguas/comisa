using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 4: Deducciones y Retenciones de Ley
/// </summary>
public class StepDeducciones : IPayrollStep
{
    private readonly ApplicationDbContext? _context;
    
    private const decimal TASA_JUBILACION = 0.11m; // 11%
    private const decimal TASA_LEY_19032 = 0.03m; // 3%
    private const decimal TASA_OBRA_SOCIAL = 0.03m; // 3%
    private const decimal TASA_CUOTA_SINDICAL = 0.025m; // 2.5%
    private const decimal IMPORTE_SEGURO_VIDA = 19989.90m; // Importe fijo

    public string StepName => "Deducciones";

    public StepDeducciones()
    {
        // Constructor sin parámetros para compatibilidad
    }

    public StepDeducciones(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync(PayrollContext context)
    {
        var totalBruto = context.TotalBrutoRemunerativo;
        var valorHoraBasico = context.Empleado.Categoria?.SalarioBasico ?? 0;

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
        //var topeMinimoQuincenal = topeMinimoOS * 0.5m;
        
        var obraSocial = Math.Max(obraSocialCalculada, topeMinimoOS);
        
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
        // Solo se paga UNA VEZ AL MES y siempre en la SEGUNDA QUINCENA
        // EXCEPCIÓN: Si hay novedad de Baja en la quincena actual, se paga en esa quincena
        bool pagarSeguroVida = false;
        
        // Verificar si tiene novedad de Baja en esta quincena
        var tieneBajaEnQuincena = context.Novedades.Any(n => n.TipoNovedad == TipoNovedadLiquidacion.Baja);
        
        if (tieneBajaEnQuincena)
        {
            // Si tiene baja en esta quincena, pagar el seguro
            pagarSeguroVida = true;
        }
        else if (context.Quincena == 2)
        {
            // Si es segunda quincena y NO tiene baja, verificar que no se haya pagado ya
            bool yaPagadoEnQuincena1 = false;
            
            if (_context != null)
            {
                yaPagadoEnQuincena1 = await _context.ReciboDetalles
                    .Include(rd => rd.Recibo)
                    .Include(rd => rd.Concepto)
                    .AnyAsync(rd => rd.Recibo.EmpleadoId == context.Empleado.Id
                        && rd.Recibo.Periodo == context.Periodo
                        && rd.Concepto.Codigo == ((int)CodigoConceptoTango.SeguroDeVida).ToString()
                        && rd.Recibo.Quincena == 1);
            }
            
            // Solo pagar si no se pagó en quincena 1
            pagarSeguroVida = !yaPagadoEnQuincena1;
        }
        
        if (pagarSeguroVida)
        {
            context.ItemsDeducciones.Add(new ItemLiquidacion
            {
                Concepto = CodigoConceptoTango.SeguroDeVida,
                Unidades = 1,
                ValorUnitario = IMPORTE_SEGURO_VIDA,
                Subtotal = IMPORTE_SEGURO_VIDA,
                Descripcion = "Seguro de Vida Obligatorio"
            });
        }

        // 6. Embargo (5011) - Si existe novedad de embargo
        var novedadesEmbargo = context.Novedades
            .Where(n => n.TipoNovedad == TipoNovedadLiquidacion.Embargo)
            .ToList();
        
        foreach (var novedad in novedadesEmbargo)
        {
            if (novedad.MontoNovedad.HasValue && novedad.MontoNovedad.Value > 0)
            {
                context.ItemsDeducciones.Add(new ItemLiquidacion
                {
                    Concepto = CodigoConceptoTango.Embargo,
                    Unidades = 1,
                    ValorUnitario = novedad.MontoNovedad.Value,
                    Subtotal = novedad.MontoNovedad.Value,
                    Descripcion = $"Embargo{(string.IsNullOrWhiteSpace(novedad.Observaciones) ? "" : $" - {novedad.Observaciones}")}"
                });
            }
        }

        // Calcular total deducciones
        context.TotalDeducciones = context.ItemsDeducciones.Sum(i => i.Subtotal);
    }
}
