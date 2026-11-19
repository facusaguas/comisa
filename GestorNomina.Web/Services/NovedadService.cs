using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services;

public class NovedadService : INovedadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NovedadService> _logger;

    public NovedadService(
        ApplicationDbContext context,
        ILogger<NovedadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Novedad>> GetAllAsync()
    {
        return await _context.Novedades
            .Include(n => n.Empleado)
            .OrderByDescending(n => n.Periodo)
            .ThenByDescending(n => n.Quincena)
            .ThenBy(n => n.Empleado.Apellido)
            .ToListAsync();
    }

    public async Task<List<Novedad>> GetByEmpleadoAsync(int empleadoId)
    {
        return await _context.Novedades
            .Include(n => n.Empleado)
            .Where(n => n.EmpleadoId == empleadoId)
            .OrderByDescending(n => n.Periodo)
            .ThenByDescending(n => n.Quincena)
            .ToListAsync();
    }

    public async Task<List<Novedad>> GetByPeriodoAsync(DateTime periodo, int quincena)
    {
        var periodoUtc = DateTime.SpecifyKind(periodo.Date, DateTimeKind.Utc);
        
        return await _context.Novedades
            .Include(n => n.Empleado)
            .Where(n => n.Periodo == periodoUtc && n.Quincena == quincena)
            .OrderBy(n => n.Empleado.Apellido)
            .ThenBy(n => n.Empleado.Nombre)
            .ToListAsync();
    }

    public async Task<List<Novedad>> GetFilteredAsync(
        string? legajo, 
        string? cuil, 
        int? proyectoId, 
        DateTime? periodo, 
        int? quincena, 
        TipoNovedadLiquidacion? tipoNovedad)
    {
        var query = _context.Novedades
            .Include(n => n.Empleado)
                .ThenInclude(e => e.Proyecto)
            .AsQueryable();

        // Filtro por legajo
        if (!string.IsNullOrWhiteSpace(legajo))
        {
            query = query.Where(n => n.Empleado.Legajo.Contains(legajo));
        }

        // Filtro por CUIL
        if (!string.IsNullOrWhiteSpace(cuil))
        {
            query = query.Where(n => n.Empleado.Cuil != null && n.Empleado.Cuil.Contains(cuil));
        }

        // Filtro por proyecto
        if (proyectoId.HasValue)
        {
            query = query.Where(n => n.Empleado.ProyectoId == proyectoId.Value);
        }

        // Filtro por periodo (mes/año)
        if (periodo.HasValue)
        {
            var periodoUtc = DateTime.SpecifyKind(periodo.Value.Date, DateTimeKind.Utc);
            query = query.Where(n => n.Periodo == periodoUtc);
        }

        // Filtro por quincena
        if (quincena.HasValue)
        {
            query = query.Where(n => n.Quincena == quincena.Value);
        }

        // Filtro por tipo de novedad
        if (tipoNovedad.HasValue)
        {
            query = query.Where(n => n.TipoNovedad == tipoNovedad.Value);
        }

        return await query
            .OrderByDescending(n => n.Periodo)
            .ThenByDescending(n => n.Quincena)
            .ThenBy(n => n.Empleado.Apellido)
            .ThenBy(n => n.Empleado.Nombre)
            .ToListAsync();
    }

    public async Task<Novedad?> GetByIdAsync(int id)
    {
        return await _context.Novedades
            .Include(n => n.Empleado)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Novedad> CreateAsync(NovedadViewModel model)
    {
        // Validar que el empleado existe
        var empleado = await _context.Empleados.FindAsync(model.EmpleadoId);
        if (empleado == null)
        {
            throw new KeyNotFoundException($"No se encontró el empleado con ID {model.EmpleadoId}");
        }

        var novedad = new Novedad
        {
            EmpleadoId = model.EmpleadoId,
            Periodo = DateTime.SpecifyKind(model.Periodo.Date, DateTimeKind.Utc),
            Quincena = model.Quincena,
            TipoNovedad = model.TipoNovedad,
            FechaNovedad = DateTime.SpecifyKind(model.FechaNovedad.Date, DateTimeKind.Utc),
            MontoNovedad = model.MontoNovedad,
            Observaciones = model.Observaciones?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Novedades.Add(novedad);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Novedad creada: Empleado {EmpleadoId}, Tipo {TipoNovedad}, Periodo {Periodo}/{Quincena}",
            novedad.EmpleadoId, novedad.TipoNovedad, novedad.Periodo.ToString("yyyy-MM"), novedad.Quincena);

        return novedad;
    }

    public async Task<Novedad> UpdateAsync(int id, NovedadViewModel model)
    {
        var novedad = await GetByIdAsync(id);
        if (novedad == null)
        {
            throw new KeyNotFoundException($"No se encontró la novedad con ID {id}");
        }

        // Validar que el empleado existe
        if (novedad.EmpleadoId != model.EmpleadoId)
        {
            var empleado = await _context.Empleados.FindAsync(model.EmpleadoId);
            if (empleado == null)
            {
                throw new KeyNotFoundException($"No se encontró el empleado con ID {model.EmpleadoId}");
            }
        }

        novedad.EmpleadoId = model.EmpleadoId;
        novedad.Periodo = DateTime.SpecifyKind(model.Periodo.Date, DateTimeKind.Utc);
        novedad.Quincena = model.Quincena;
        novedad.TipoNovedad = model.TipoNovedad;
        novedad.FechaNovedad = DateTime.SpecifyKind(model.FechaNovedad.Date, DateTimeKind.Utc);
        novedad.MontoNovedad = model.MontoNovedad;
        novedad.Observaciones = model.Observaciones?.Trim();
        novedad.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Novedad actualizada: ID {Id}, Empleado {EmpleadoId}, Tipo {TipoNovedad}",
            novedad.Id, novedad.EmpleadoId, novedad.TipoNovedad);

        return novedad;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var novedad = await GetByIdAsync(id);
        if (novedad == null)
        {
            return false;
        }

        _context.Novedades.Remove(novedad);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Novedad eliminada: ID {Id}, Empleado {EmpleadoId}, Tipo {TipoNovedad}",
            novedad.Id, novedad.EmpleadoId, novedad.TipoNovedad);

        return true;
    }

    public async Task<bool> ExisteNovedadAsync(int empleadoId, DateTime periodo, int quincena, TipoNovedadLiquidacion tipoNovedad, int? excludeId = null)
    {
        var periodoUtc = DateTime.SpecifyKind(periodo.Date, DateTimeKind.Utc);
        
        var query = _context.Novedades
            .Where(n => n.EmpleadoId == empleadoId 
                && n.Periodo == periodoUtc 
                && n.Quincena == quincena 
                && n.TipoNovedad == tipoNovedad);

        if (excludeId.HasValue)
        {
            query = query.Where(n => n.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
