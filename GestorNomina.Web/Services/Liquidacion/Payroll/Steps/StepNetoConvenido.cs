using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services.Liquidacion.Payroll.Steps;

/// <summary>
/// Paso 3: Motor de Igualmente y Grossing-Up (Neto Convenido)
/// Ajusta las horas normales, extras 50% y extras 100% hasta los límites del convenio
/// antes de aplicar bonificación especial
/// </summary>
public class StepNetoConvenido : IPayrollStep
{
    private readonly ICatalogResolver _catalogResolver;
    private readonly ApplicationDbContext? _context;
    private const decimal COEF_RETENCIONES_LEY = 0.195m; // 11% + 3% + 3% + 2.5%
    private const decimal COEF_GROSSING_UP = 1.24223572358584m; // Factor de ajuste bruto

    public string StepName => "Neto Convenido (Grossing-Up)";

    public StepNetoConvenido(ICatalogResolver catalogResolver)
    {
        _catalogResolver = catalogResolver;
    }

    public StepNetoConvenido(ICatalogResolver catalogResolver, ApplicationDbContext context)
    {
        _catalogResolver = catalogResolver;
        _context = context;
    }

    public async Task ExecuteAsync(PayrollContext context)
    {
        // Verificar si aplica neto convenido
        var tipoConvenidoAcuerdo = _catalogResolver.GetTipoLiquidacionId(CodigoTipoLiquidacion.DIRECTOS_CONVERSION_ACUERDO);
        var tipoIndirecto = _catalogResolver.GetTipoLiquidacionId(CodigoTipoLiquidacion.INDIRECTO);
        
        var esConvenido = context.Empleado.TipoLiquidacionId == tipoConvenidoAcuerdo || 
                          context.Empleado.TipoLiquidacionId == tipoIndirecto;

        if (!esConvenido || !context.Empleado.ValorConvenidoDiario.HasValue || context.Empleado.ValorConvenidoDiario <= 0)
        {
            return;
        }

        // 1. Calcular Neto Objetivo basado en DÍAS TRABAJADOS EN LA QUINCENA
        var netoObjetivo = context.Empleado.ValorConvenidoDiario.Value * context.DIAS;
        context.NetoConvenidoObjetivo = netoObjetivo;

        // 2. Calcular Neto Provisorio usando deducciones REALES (no aproximación)
        var brutoParcial = context.TotalBrutoRemunerativo + context.TotalBrutoNoRemunerativo;
        var deduccionesReales = context.ItemsDeducciones.Sum(i => i.Subtotal);
        var netoProvisorio = brutoParcial - deduccionesReales;

        // 3. Si ya llegamos al neto objetivo, no hacer nada
        if (netoProvisorio >= netoObjetivo)
        {
            return;
        }

        // 4. Intentar ajustar incrementando horas según los límites del convenio
        if (context.Empleado.Convenio != null)
        {
            var ajustado = TryAjustarHoras(context, netoObjetivo, deduccionesReales);
            
            if (ajustado)
            {
                // CRÍTICO: Recalcular todos los conceptos que dependen del sueldo base
                // porque las horas cambiaron
                var valorHora = context.Empleado.Categoria?.SalarioBasico ?? 0;
                if (valorHora > 0)
                {
                    var calculator = new ConceptosCalculator(_context);
                    await calculator.RecalcularConceptosDependientesAsync(context, valorHora);
                }
                
                // Recalcular neto provisorio después del ajuste de horas Y conceptos
                brutoParcial = context.TotalBrutoRemunerativo + context.TotalBrutoNoRemunerativo;
                
                // CRÍTICO: Recalcular deducciones sobre el nuevo bruto
                context.ItemsDeducciones.Clear();
                if (_context != null)
                {
                    var stepDeducciones = new StepDeducciones(_context);
                    await stepDeducciones.ExecuteAsync(context);
                }
                
                deduccionesReales = context.ItemsDeducciones.Sum(i => i.Subtotal);
                netoProvisorio = brutoParcial - deduccionesReales;
                
                // Si alcanzamos el objetivo con el ajuste de horas, terminar
                if (netoProvisorio >= netoObjetivo)
                {
                    return;
                }
            }
        }

        // 5. Si aún no llegamos al objetivo, aplicar Bonificación Especial (Grossing-Up)
        var diferenciaNeto = netoObjetivo - netoProvisorio;
        
        // Usar deducciones reales para calcular el grossing-up correctamente
        // Si la deducción es 30%, el grossing-up debe compensar eso
        var tasaDeduccion = deduccionesReales > 0 ? (deduccionesReales / brutoParcial) : COEF_RETENCIONES_LEY;
        var coefGrossingUp = 1 / (1 - tasaDeduccion);
        var ajusteBruto = diferenciaNeto * coefGrossingUp;

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
        
        // CRÍTICO: Recalcular deducciones sobre el bruto CON la bonificación
        context.ItemsDeducciones.Clear();
        if (_context != null)
        {
            var stepDeducciones = new StepDeducciones(_context);
            await stepDeducciones.ExecuteAsync(context);
        }
    }

    /// <summary>
    /// Intenta ajustar las horas incrementando normales, extras 50% y extras 100%
    /// hasta los límites configurados en el convenio para alcanzar el neto objetivo
    /// Usa las deducciones REALES para calcular neto provisorio
    /// </summary>
    private bool TryAjustarHoras(PayrollContext context, decimal netoObjetivo, decimal deduccionesReales)
    {
        var convenio = context.Empleado.Convenio;
        if (convenio == null)
            return false;

        // Obtener límites del convenio (si no están configurados, no ajustar)
        var hsNormalesMax = convenio.HsNormalesMax ?? 0;
        var hs50Max = convenio.Hs50Max ?? 0;
        var hs100Max = convenio.Hs100Max ?? 0;

        if (hsNormalesMax <= 0 && hs50Max <= 0 && hs100Max <= 0)
            return false;

        var valorHora = context.Empleado.Categoria?.SalarioBasico ?? 0;
        if (valorHora <= 0)
            return false;

        // Calcular horas actuales desde los items remunerativos
        var itemHsNormales = context.ItemsRemunerativos.FirstOrDefault(i => i.Concepto == CodigoConceptoTango.HorasNormales);
        var itemHs50 = context.ItemsRemunerativos.FirstOrDefault(i => i.Concepto == CodigoConceptoTango.HorasExtras50);
        var itemHs100 = context.ItemsRemunerativos.FirstOrDefault(i => i.Concepto == CodigoConceptoTango.HorasExtras100);

        // Inicializar con valores actuales del contexto (que vienen del Paso 1)
        var hsNormalesActuales = context.HSNOR;
        var hs50Actuales = context.HSEXT;
        var hs100Actuales = context.HS100;

        // Iterar incrementando horas hasta alcanzar el neto objetivo o los límites
        bool ajustado = false;
        const int MAX_ITERACIONES = 1000; // Protección contra bucle infinito
        int iteracion = 0;

        while (iteracion < MAX_ITERACIONES)
        {
            iteracion++;

            // Calcular neto actual usando deducciones REALES
            var brutoActual = context.ItemsRemunerativos.Sum(i => i.Subtotal);
            var netoActual = brutoActual - deduccionesReales;

            // Si ya alcanzamos el objetivo, terminar
            if (netoActual >= netoObjetivo)
                break;

            // Intentar agregar horas en orden: Normales → Extras 50% → Extras 100%
            bool agregado = false;

            // 1. Intentar agregar horas normales
            if (hsNormalesMax > 0 && hsNormalesActuales < hsNormalesMax)
            {
                hsNormalesActuales += 0.5m; // Incrementar de a media hora
                
                // Actualizar item
                if (itemHsNormales != null)
                {
                    itemHsNormales.Unidades = hsNormalesActuales;
                    itemHsNormales.Subtotal = hsNormalesActuales * valorHora;
                }
                else
                {
                    // Crear nuevo item
                    itemHsNormales = new ItemLiquidacion
                    {
                        Concepto = CodigoConceptoTango.HorasNormales,
                        Unidades = hsNormalesActuales,
                        ValorUnitario = valorHora,
                        Subtotal = hsNormalesActuales * valorHora,
                        Descripcion = "Horas Normales (Ajustadas)"
                    };
                    context.ItemsRemunerativos.Add(itemHsNormales);
                }
                
                // CRÍTICO: Actualizar el contexto
                context.HSNOR = hsNormalesActuales;
                
                agregado = true;
                ajustado = true;
            }
            // 2. Si llegamos al límite de normales, intentar extras 50%
            else if (hs50Max > 0 && hs50Actuales < hs50Max)
            {
                hs50Actuales += 0.5m;
                var valorHs50 = valorHora * 1.5m * 1.45m;
                
                if (itemHs50 != null)
                {
                    itemHs50.Unidades = hs50Actuales;
                    itemHs50.Subtotal = hs50Actuales * valorHs50;
                }
                else
                {
                    itemHs50 = new ItemLiquidacion
                    {
                        Concepto = CodigoConceptoTango.HorasExtras50,
                        Unidades = hs50Actuales,
                        ValorUnitario = valorHs50,
                        Subtotal = hs50Actuales * valorHs50,
                        Descripcion = "Horas Extras 50% (Ajustadas)"
                    };
                    context.ItemsRemunerativos.Add(itemHs50);
                }
                
                // CRÍTICO: Actualizar el contexto
                context.HSEXT = hs50Actuales;
                
                agregado = true;
                ajustado = true;
            }
            // 3. Si llegamos al límite de extras 50%, intentar extras 100%
            else if (hs100Max > 0 && hs100Actuales < hs100Max)
            {
                hs100Actuales += 0.5m;
                var valorHs100 = valorHora * 2.0m * 1.45m;
                
                if (itemHs100 != null)
                {
                    itemHs100.Unidades = hs100Actuales;
                    itemHs100.Subtotal = hs100Actuales * valorHs100;
                }
                else
                {
                    itemHs100 = new ItemLiquidacion
                    {
                        Concepto = CodigoConceptoTango.HorasExtras100,
                        Unidades = hs100Actuales,
                        ValorUnitario = valorHs100,
                        Subtotal = hs100Actuales * valorHs100,
                        Descripcion = "Horas Extras 100% (Ajustadas)"
                    };
                    context.ItemsRemunerativos.Add(itemHs100);
                }
                
                // CRÍTICO: Actualizar el contexto
                context.HS100 = hs100Actuales;
                
                agregado = true;
                ajustado = true;
            }

            // Si no pudimos agregar más horas (límites alcanzados), salir
            if (!agregado)
                break;
        }

        // Recalcular el total bruto remunerativo si hubo ajustes
        if (ajustado)
        {
            context.TotalBrutoRemunerativo = context.ItemsRemunerativos.Sum(i => i.Subtotal);
        }

        return ajustado;
    }
}
