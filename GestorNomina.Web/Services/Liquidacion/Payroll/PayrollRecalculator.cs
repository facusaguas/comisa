using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.Enums;
using GestorNomina.Web.Services.Liquidacion.Payroll.Steps;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services.Liquidacion.Payroll;

/// <summary>
/// Servicio para recalcular recibos de liquidación después de ediciones manuales
/// </summary>
public class PayrollRecalculator
{
    private readonly ApplicationDbContext _context;
    private readonly ICatalogResolver _catalogResolver;
    private const decimal COEF_RETENCIONES_LEY = 0.195m; // 11% + 3% + 3% + 2.5%

    public PayrollRecalculator(ApplicationDbContext context, ICatalogResolver catalogResolver)
    {
        _context = context;
        _catalogResolver = catalogResolver;
    }

    /// <summary>
    /// Recalcula el recibo completo basándose en ediciones manuales de horas
    /// </summary>
    public async Task<Recibo> RecalcularDesdeHorasAsync(
        int reciboId,
        decimal hsnor,
        decimal hsext,
        decimal hs100,
        decimal hsnoct,
        decimal hsnoct50,
        decimal hsnoct100,
        int usuarioId)
    {
        var recibo = await CargarReciboCompletoAsync(reciboId);
        
        // Crear contexto de liquidación desde el recibo existente
        var context = await CrearContextoDesdeReciboAsync(recibo);
        
        // Actualizar las horas en el contexto
        context.HSNOR = hsnor;
        context.HSEXT = hsext;
        context.HS100 = hs100;
        context.HSNOCT = hsnoct;
        context.HSNOCT50 = hsnoct50;
        context.HSNOCT100 = hsnoct100;

        var valorHora = context.Empleado.Categoria?.SalarioBasico ?? 0;
        if (valorHora <= 0)
        {
            throw new InvalidOperationException("El empleado no tiene configurado el salario básico");
        }

        // Limpiar items remunerativos para recalcular
        context.ItemsRemunerativos.Clear();
        
        // Ejecutar pasos de recálculo
        var stepConceptos = new StepConceptosRemunerativos(_context);
        await stepConceptos.ExecuteAsync(context);
        
        var stepNetoConvenido = new StepNetoConvenido(_catalogResolver, _context);
        await stepNetoConvenido.ExecuteAsync(context);
        
        // Recalcular deducciones
        context.ItemsDeducciones.Clear();
        var stepDeducciones = new StepDeducciones(_context);
        await stepDeducciones.ExecuteAsync(context);
        
        // Cierre del recibo
        var stepCierre = new StepCierreRecibo();
        await stepCierre.ExecuteAsync(context);

        // Actualizar recibo en la base de datos
        await ActualizarReciboAsync(recibo, context, usuarioId, "MANUAL");
        
        return recibo;
    }

    /// <summary>
    /// Agrega un nuevo concepto al recibo
    /// </summary>
    public async Task<ReciboDetalle> AgregarConceptoAsync(
        int reciboId,
        int conceptoId,
        decimal unidades,
        decimal? valorUnitario,
        int usuarioId)
    {
        var recibo = await CargarReciboCompletoAsync(reciboId);
        var concepto = await _context.ConceptosLiquidacion.FindAsync(conceptoId);
        
        if (concepto == null)
            throw new InvalidOperationException($"No se encontró el concepto con ID {conceptoId}");

        // Verificar si ya existe el concepto y es obligatorio (no se puede duplicar)
        var existeObligatorio = recibo.Detalles.Any(d => 
            d.ConceptoId == conceptoId && 
            d.Concepto.EsObligatorio);
        
        if (existeObligatorio)
            throw new InvalidOperationException($"El concepto obligatorio '{concepto.Nombre}' ya existe en el recibo");

        // Calcular subtotal
        var subtotal = valorUnitario.HasValue ? unidades * valorUnitario.Value : unidades;

        // Crear nuevo detalle
        var nuevoDetalle = new ReciboDetalle
        {
            ReciboId = reciboId,
            ConceptoId = conceptoId,
            Unidades = unidades,
            ValorUnitario = valorUnitario,
            Subtotal = subtotal,
            EditadoManualmente = true
        };

        _context.ReciboDetalles.Add(nuevoDetalle);
        
        // Marcar recibo como editado manualmente
        recibo.ModoEdicion = "MANUAL";
        recibo.EditadoPorUsuarioId = usuarioId;
        recibo.FechaUltimaEdicion = DateTime.UtcNow;

        // Recalcular totales
        await RecalcularTotalesAsync(recibo);
        
        await _context.SaveChangesAsync();

        // Recargar con navegación
        await _context.Entry(nuevoDetalle).Reference(d => d.Concepto).LoadAsync();
        
        return nuevoDetalle;
    }

    /// <summary>
    /// Elimina un concepto del recibo (solo si no es obligatorio)
    /// </summary>
    public async Task EliminarConceptoAsync(int reciboId, int detalleId, int usuarioId)
    {
        var recibo = await CargarReciboCompletoAsync(reciboId);
        var detalle = recibo.Detalles.FirstOrDefault(d => d.Id == detalleId);
        
        if (detalle == null)
            throw new InvalidOperationException($"No se encontró el detalle con ID {detalleId}");

        if (detalle.Concepto.EsObligatorio)
            throw new InvalidOperationException($"No se puede eliminar el concepto obligatorio '{detalle.Concepto.Nombre}'");

        _context.ReciboDetalles.Remove(detalle);
        
        // Marcar recibo como editado manualmente
        recibo.ModoEdicion = "MANUAL";
        recibo.EditadoPorUsuarioId = usuarioId;
        recibo.FechaUltimaEdicion = DateTime.UtcNow;

        // Recalcular totales
        await RecalcularTotalesAsync(recibo);
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Modifica unidades y/o valor unitario de un concepto (solo si es editable)
    /// </summary>
    public async Task<ReciboDetalle> ModificarConceptoAsync(
        int reciboId,
        int detalleId,
        decimal nuevasUnidades,
        decimal? nuevoValorUnitario,
        int usuarioId)
    {
        var recibo = await CargarReciboCompletoAsync(reciboId);
        var detalle = recibo.Detalles.FirstOrDefault(d => d.Id == detalleId);
        
        if (detalle == null)
            throw new InvalidOperationException($"No se encontró el detalle con ID {detalleId}");

        if (!detalle.Concepto.EsEditable)
            throw new InvalidOperationException($"El concepto '{detalle.Concepto.Nombre}' no es editable");

        // Guardar valor original si es la primera edición
        if (!detalle.EditadoManualmente)
        {
            detalle.ValorOriginal = detalle.Subtotal;
        }

        // Actualizar valores
        detalle.Unidades = nuevasUnidades;
        detalle.ValorUnitario = nuevoValorUnitario;
        detalle.Subtotal = nuevoValorUnitario.HasValue 
            ? nuevasUnidades * nuevoValorUnitario.Value 
            : nuevasUnidades;
        detalle.EditadoManualmente = true;

        // Marcar recibo como editado manualmente
        recibo.ModoEdicion = "MANUAL";
        recibo.EditadoPorUsuarioId = usuarioId;
        recibo.FechaUltimaEdicion = DateTime.UtcNow;

        // Recalcular totales
        await RecalcularTotalesAsync(recibo);
        
        await _context.SaveChangesAsync();
        
        return detalle;
    }

    /// <summary>
    /// Recalcula los totales del recibo basándose en los detalles actuales
    /// </summary>
    private async Task RecalcularTotalesAsync(Recibo recibo)
    {
        // Asegurar que los detalles están cargados
        if (!_context.Entry(recibo).Collection(r => r.Detalles).IsLoaded)
        {
            await _context.Entry(recibo).Collection(r => r.Detalles).LoadAsync();
        }

        // Cargar conceptos si no están cargados
        foreach (var detalle in recibo.Detalles)
        {
            if (!_context.Entry(detalle).Reference(d => d.Concepto).IsLoaded)
            {
                await _context.Entry(detalle).Reference(d => d.Concepto).LoadAsync();
            }
        }

        var remunerativos = recibo.Detalles
            .Where(d => d.Concepto.TipoConcepto == TipoConcepto.REMUNERATIVO)
            .Sum(d => d.Subtotal);

        var noRemunerativos = recibo.Detalles
            .Where(d => d.Concepto.TipoConcepto == TipoConcepto.NO_REMUNERATIVO)
            .Sum(d => d.Subtotal);

        var deducciones = recibo.Detalles
            .Where(d => d.Concepto.TipoConcepto == TipoConcepto.DEDUCCION)
            .Sum(d => d.Subtotal);

        recibo.TotalBruto = remunerativos + noRemunerativos;
        recibo.TotalDeducciones = deducciones;
        recibo.TotalNeto = recibo.TotalBruto - deducciones;
        
        // Redondeo
        recibo.NetoRedondeado = (int)Math.Round(recibo.TotalNeto);
        recibo.DiferenciaControl = recibo.TotalNeto - recibo.NetoRedondeado;
    }

    private async Task<Recibo> CargarReciboCompletoAsync(int reciboId)
    {
        var recibo = await _context.Recibos
            .Include(r => r.Empleado)
                .ThenInclude(e => e.Categoria)
            .Include(r => r.Empleado.Convenio)
            .Include(r => r.Empleado.Proyecto)
            .Include(r => r.Detalles)
                .ThenInclude(d => d.Concepto)
            .FirstOrDefaultAsync(r => r.Id == reciboId);

        if (recibo == null)
            throw new InvalidOperationException($"No se encontró el recibo con ID {reciboId}");

        return recibo;
    }

    private async Task<PayrollContext> CrearContextoDesdeReciboAsync(Recibo recibo)
    {
        // Cargar novedades del periodo
        var fechaPeriodo = DateTime.Parse(recibo.Periodo + "-01");
        var novedades = await _context.Novedades
            .Where(n => n.EmpleadoId == recibo.EmpleadoId
                && n.Periodo.Year == fechaPeriodo.Year
                && n.Periodo.Month == fechaPeriodo.Month
                && n.Quincena == recibo.Quincena)
            .ToListAsync();

        return new PayrollContext
        {
            Empleado = recibo.Empleado,
            Periodo = recibo.Periodo,
            Quincena = recibo.Quincena,
            DIAS = recibo.DiasTrabajados,
            Registros = new List<HoraRegistroAprobado>(), // No necesitamos los registros raw
            Novedades = novedades
        };
    }

    private async Task ActualizarReciboAsync(Recibo recibo, PayrollContext context, int usuarioId, string modoEdicion)
    {
        // Actualizar totales
        recibo.DiasTrabajados = context.DIAS;
        recibo.TotalBruto = context.TotalBruto;
        recibo.TotalDeducciones = context.TotalDeducciones;
        recibo.TotalNeto = context.TotalNeto;
        recibo.NetoConvenidoObjetivo = context.NetoConvenidoObjetivo;
        recibo.DiferenciaControl = context.DiferenciaControl;
        recibo.NetoRedondeado = context.NetoRedondeado;
        recibo.ModoEdicion = modoEdicion;
        recibo.EditadoPorUsuarioId = usuarioId;
        recibo.FechaUltimaEdicion = DateTime.UtcNow;

        // Eliminar detalles anteriores
        _context.ReciboDetalles.RemoveRange(recibo.Detalles);
        
        // Agregar nuevos detalles
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
                Subtotal = item.Subtotal,
                EditadoManualmente = false // Recalculado automáticamente
            };

            _context.ReciboDetalles.Add(detalle);
        }

        await _context.SaveChangesAsync();
    }
}
