using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Services.Liquidacion.Payroll.Steps;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services.Liquidacion.Payroll;

/// <summary>
/// Implementación del motor de liquidación con patrón Pipeline
/// </summary>
public class PayrollEngine : IPayrollEngine
{
    private readonly ApplicationDbContext _context;
    private readonly ICatalogResolver _catalogResolver;
    private readonly List<IPayrollStep> _steps;

    public PayrollEngine(
        ApplicationDbContext context,
        ICatalogResolver catalogResolver)
    {
        _context = context;
        _catalogResolver = catalogResolver;

        // Configurar el pipeline de pasos
        _steps = new List<IPayrollStep>
        {
            new StepAgregadorHoras(catalogResolver),
            new StepConceptosRemunerativos(),
            new StepNetoConvenido(catalogResolver),
            new StepDeducciones(),
            new StepCierreRecibo()
        };
    }

    public async Task<Recibo> ProcesarLiquidacionAsync(int empleadoId, string periodo, int quincena)
    {
        // Validar catálogos
        if (!_catalogResolver.IsLoaded)
            await _catalogResolver.LoadCatalogsAsync();

        // Cargar empleado con datos de liquidación
        var empleado = await _context.Empleados
            .Include(e => e.Convenio)
            .Include(e => e.TipoLiquidacion)
            .Include(e => e.Categoria)
            .Include(e => e.Disciplina)
            .Include(e => e.Proyecto)
            .FirstOrDefaultAsync(e => e.Id == empleadoId);

        if (empleado == null)
            throw new InvalidOperationException($"No se encontró el empleado con ID {empleadoId}");

        if (!empleado.Activo)
            throw new InvalidOperationException($"El empleado {empleado.Legajo} está inactivo");

        // Validar que el empleado tenga configuración de liquidación
        if (!empleado.ValorHoraBasico.HasValue || empleado.ValorHoraBasico <= 0)
            throw new InvalidOperationException($"El empleado {empleado.Legajo} no tiene configurado el valor hora básico");

        // Cargar registros aprobados del periodo
        var registros = await _context.HorasRegistrosAprobados
            .Include(hra => hra.Actividad)
            .Include(hra => hra.Empleado)
            .Where(hra => hra.EmpleadoId == empleadoId 
                && hra.PeriodoCarga.Mes == int.Parse(periodo.Substring(5, 2))
                && hra.PeriodoCarga.Anio == int.Parse(periodo.Substring(0, 4)))
            .OrderBy(hra => hra.Fecha)
            .ToListAsync();

        if (!registros.Any())
            throw new InvalidOperationException($"No hay registros aprobados para el empleado {empleado.Legajo} en el periodo {periodo} quincena {quincena}");

        // Filtrar por quincena
        var registrosFiltrados = registros
            .Where(r => (quincena == 1 && r.Fecha.Day <= 15) || (quincena == 2 && r.Fecha.Day > 15))
            .ToList();

        if (!registrosFiltrados.Any())
            throw new InvalidOperationException($"No hay registros aprobados para el empleado {empleado.Legajo} en el periodo {periodo} quincena {quincena}");

        // Crear contexto de liquidación
        var context = new PayrollContext
        {
            Empleado = empleado,
            Periodo = periodo,
            Quincena = quincena,
            Registros = registrosFiltrados
        };

        // Ejecutar pipeline de pasos
        foreach (var step in _steps)
        {
            await step.ExecuteAsync(context);
        }

        // Crear o actualizar recibo
        var reciboExistente = await _context.Recibos
            .Include(r => r.Detalles)
            .FirstOrDefaultAsync(r => r.EmpleadoId == empleadoId 
                && r.Periodo == periodo 
                && r.Quincena == quincena);

        Recibo recibo;

        if (reciboExistente != null)
        {
            // Actualizar recibo existente
            recibo = reciboExistente;
            recibo.DiasTrabajados = context.DIAS;
            recibo.TotalBruto = context.TotalBruto;
            recibo.TotalDeducciones = context.TotalDeducciones;
            recibo.TotalNeto = context.TotalNeto;
            recibo.NetoConvenidoObjetivo = context.NetoConvenidoObjetivo;
            recibo.DiferenciaControl = context.DiferenciaControl;
            recibo.NetoRedondeado = context.NetoRedondeado;
            recibo.FechaCalculo = DateTime.UtcNow;

            // Eliminar detalles anteriores
            _context.ReciboDetalles.RemoveRange(recibo.Detalles);
        }
        else
        {
            // Crear nuevo recibo
            recibo = new Recibo
            {
                EmpleadoId = empleadoId,
                Periodo = periodo,
                Quincena = quincena,
                DiasTrabajados = context.DIAS,
                TotalBruto = context.TotalBruto,
                TotalDeducciones = context.TotalDeducciones,
                TotalNeto = context.TotalNeto,
                NetoConvenidoObjetivo = context.NetoConvenidoObjetivo,
                DiferenciaControl = context.DiferenciaControl,
                NetoRedondeado = context.NetoRedondeado,
                FechaCalculo = DateTime.UtcNow
            };

            _context.Recibos.Add(recibo);
        }

        // Guardar para obtener el ID del recibo
        await _context.SaveChangesAsync();

        // Agregar detalles del recibo
        var todosLosItems = context.ItemsRemunerativos
            .Concat(context.ItemsNoRemunerativos)
            .Concat(context.ItemsDeducciones)
            .ToList();

        foreach (var item in todosLosItems)
        {
            var conceptoId = _catalogResolver.GetConceptoId(item.Concepto);

            var detalle = new ReciboDetalle
            {
                ReciboId = recibo.Id,
                ConceptoId = conceptoId,
                Unidades = item.Unidades,
                ValorUnitario = item.ValorUnitario,
                Subtotal = item.Subtotal
            };

            _context.ReciboDetalles.Add(detalle);
        }

        await _context.SaveChangesAsync();

        // Recargar recibo con detalles
        return await _context.Recibos
            .Include(r => r.Empleado)
            .Include(r => r.Detalles)
                .ThenInclude(d => d.Concepto)
            .FirstAsync(r => r.Id == recibo.Id);
    }

    public async Task<List<Recibo>> ProcesarLiquidacionMasivaAsync(string periodo, int quincena)
    {
        var año = int.Parse(periodo.Substring(0, 4));
        var mes = int.Parse(periodo.Substring(5, 2));

        // Obtener todos los empleados activos con registros aprobados en el periodo
        var empleadosConRegistros = await _context.HorasRegistrosAprobados
            .Where(hra => hra.PeriodoCarga.Anio == año && hra.PeriodoCarga.Mes == mes)
            .Where(hra => (quincena == 1 && hra.Fecha.Day <= 15) || (quincena == 2 && hra.Fecha.Day > 15))
            .Select(hra => hra.EmpleadoId)
            .Distinct()
            .ToListAsync();

        var recibos = new List<Recibo>();

        foreach (var empleadoId in empleadosConRegistros)
        {
            try
            {
                var recibo = await ProcesarLiquidacionAsync(empleadoId, periodo, quincena);
                recibos.Add(recibo);
            }
            catch (Exception ex)
            {
                // Log del error pero continuar con los demás empleados
                Console.WriteLine($"Error procesando empleado {empleadoId}: {ex.Message}");
            }
        }

        return recibos;
    }
}
