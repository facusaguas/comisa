using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Implementación del servicio de aprobación de horas
/// </summary>
public class HorasAprobacionService : IHorasAprobacionService
{
    private readonly ApplicationDbContext _context;
    private readonly ICatalogResolver _catalogResolver;

    public HorasAprobacionService(ApplicationDbContext context, ICatalogResolver catalogResolver)
    {
        _context = context;
        _catalogResolver = catalogResolver;
    }

    public async Task<int> AprobarAutomaticamenteAsync(int periodoCargaId, int usuarioId)
    {
        if (!_catalogResolver.IsLoaded)
            await _catalogResolver.LoadCatalogsAsync();

        // Obtener todos los registros raw del periodo que no tienen aprobado
        var registrosRaw = await _context.HorasRegistrosRaw
            .Include(r => r.HoraRegistroAprobado)
            .Where(r => r.PeriodoCargaId == periodoCargaId && r.HoraRegistroAprobado == null)
            .ToListAsync();

        var registrosAprobados = new List<HoraRegistroAprobado>();

        foreach (var raw in registrosRaw)
        {
            // Resolver empleado_id si no está presente
            if (!raw.EmpleadoId.HasValue)
            {
                var empleado = await _context.Empleados
                    .FirstOrDefaultAsync(e => e.Legajo == raw.EmpleadoLegajo);
                
                if (empleado != null)
                    raw.EmpleadoId = empleado.Id;
                else
                    continue; // Omitir si no se encuentra el empleado
            }

            // Resolver actividad_id si no está presente
            if (!raw.ActividadId.HasValue)
            {
                var actividadId = await _catalogResolver.GetActividadIdByCodigo(raw.ActividadCodigo);
                if (actividadId.HasValue)
                    raw.ActividadId = actividadId.Value;
                else
                    continue; // Omitir si no se encuentra la actividad
            }

            var aprobado = new HoraRegistroAprobado
            {
                RegistroRawId = raw.Id,
                PeriodoCargaId = raw.PeriodoCargaId,
                EmpleadoId = raw.EmpleadoId.Value,
                ActividadId = raw.ActividadId.Value,
                Fecha = raw.Fecha,
                HorasFinales = raw.HorasCargadas,
                FueModificado = false,
                UpdatedAt = DateTime.UtcNow
            };

            registrosAprobados.Add(aprobado);
        }

        _context.HorasRegistrosAprobados.AddRange(registrosAprobados);
        await _context.SaveChangesAsync();

        // Actualizar estado del periodo
        var periodo = await _context.PeriodosCarga.FindAsync(periodoCargaId);
        if (periodo != null)
        {
            periodo.Estado = "Aprobado";
            periodo.UpdatedAt = DateTime.UtcNow;
        }

        return registrosAprobados.Count;
    }

    public async Task AprobarRegistroAsync(int registroRawId, decimal? horasCorregidas, string? motivoModificacion, int usuarioId)
    {
        if (!_catalogResolver.IsLoaded)
            await _catalogResolver.LoadCatalogsAsync();

        var raw = await _context.HorasRegistrosRaw
            .Include(r => r.HoraRegistroAprobado)
            .FirstOrDefaultAsync(r => r.Id == registroRawId);

        if (raw == null)
            throw new InvalidOperationException($"No se encontró el registro raw con ID: {registroRawId}");

        if (raw.HoraRegistroAprobado != null)
            throw new InvalidOperationException($"El registro raw {registroRawId} ya fue aprobado");

        // Resolver IDs si no están presentes
        if (!raw.EmpleadoId.HasValue)
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.Legajo == raw.EmpleadoLegajo);
            
            if (empleado == null)
                throw new InvalidOperationException($"No se encontró empleado con legajo: {raw.EmpleadoLegajo}");
            
            raw.EmpleadoId = empleado.Id;
        }

        if (!raw.ActividadId.HasValue)
        {
            var actividadId = await _catalogResolver.GetActividadIdByCodigo(raw.ActividadCodigo);
            if (!actividadId.HasValue)
                throw new InvalidOperationException($"No se encontró actividad con código: {raw.ActividadCodigo}");
            
            raw.ActividadId = actividadId.Value;
        }

        var horasFinales = horasCorregidas ?? raw.HorasCargadas;
        var fueModificado = horasCorregidas.HasValue && horasCorregidas.Value != raw.HorasCargadas;

        var aprobado = new HoraRegistroAprobado
        {
            RegistroRawId = raw.Id,
            PeriodoCargaId = raw.PeriodoCargaId,
            EmpleadoId = raw.EmpleadoId.Value,
            ActividadId = raw.ActividadId.Value,
            Fecha = raw.Fecha,
            HorasFinales = horasFinales,
            FueModificado = fueModificado,
            MotivoModificacion = fueModificado ? motivoModificacion : null,
            UsuarioModificadorId = fueModificado ? usuarioId : null,
            UpdatedAt = DateTime.UtcNow
        };

        _context.HorasRegistrosAprobados.Add(aprobado);
        await _context.SaveChangesAsync();
    }

    public async Task<int> AprobarRegistrosEnLoteAsync(List<int> registrosRawIds, int usuarioId)
    {
        var count = 0;
        foreach (var id in registrosRawIds)
        {
            try
            {
                await AprobarRegistroAsync(id, null, null, usuarioId);
                count++;
            }
            catch
            {
                // Continuar con el siguiente si falla uno
                continue;
            }
        }
        return count;
    }

    public async Task CorregirRegistroAprobadoAsync(int registroAprobadoId, decimal nuevasHoras, string motivoModificacion, int usuarioId)
    {
        var aprobado = await _context.HorasRegistrosAprobados
            .FirstOrDefaultAsync(a => a.Id == registroAprobadoId);

        if (aprobado == null)
            throw new InvalidOperationException($"No se encontró el registro aprobado con ID: {registroAprobadoId}");

        aprobado.HorasFinales = nuevasHoras;
        aprobado.FueModificado = true;
        aprobado.MotivoModificacion = motivoModificacion;
        aprobado.UsuarioModificadorId = usuarioId;
        aprobado.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<List<HoraRegistroRaw>> ObtenerRegistrosPendientesAsync(int periodoCargaId)
    {
        return await _context.HorasRegistrosRaw
            .Include(r => r.HoraRegistroAprobado)
            .Include(r => r.Empleado)
            .Include(r => r.Actividad)
            .Where(r => r.PeriodoCargaId == periodoCargaId && r.HoraRegistroAprobado == null)
            .OrderBy(r => r.Fecha)
            .ThenBy(r => r.EmpleadoLegajo)
            .ToListAsync();
    }

    public async Task<bool> TieneRegistrosAprobadosAsync(int periodoCargaId)
    {
        return await _context.HorasRegistrosAprobados
            .AnyAsync(a => a.PeriodoCargaId == periodoCargaId);
    }
}
