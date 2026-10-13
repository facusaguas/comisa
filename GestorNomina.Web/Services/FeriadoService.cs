using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services;

public class FeriadoService : IFeriadoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FeriadoService> _logger;

    public FeriadoService(ApplicationDbContext context, ILogger<FeriadoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<FeriadoViewModel>> GetAllAsync()
    {
        try
        {
            var feriados = await _context.Feriados
                .OrderBy(f => f.Fecha)
                .ToListAsync();

            return feriados.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener feriados");
            throw;
        }
    }

    public async Task<List<FeriadoViewModel>> GetByAnioAsync(int anio)
    {
        try
        {
            var feriados = await _context.Feriados
                .Where(f => f.Fecha.Year == anio)
                .OrderBy(f => f.Fecha)
                .ToListAsync();

            return feriados.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener feriados del año {Anio}", anio);
            throw;
        }
    }

    public async Task<List<FeriadoViewModel>> GetByJurisdiccionAsync(string jurisdiccion)
    {
        try
        {
            var feriados = await _context.Feriados
                .Where(f => f.Jurisdiccion == jurisdiccion)
                .OrderBy(f => f.Fecha)
                .ToListAsync();

            return feriados.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener feriados de {Jurisdiccion}", jurisdiccion);
            throw;
        }
    }

    public async Task<FeriadoViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var feriado = await _context.Feriados.FindAsync(id);
            return feriado != null ? MapToViewModel(feriado) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener feriado {FeriadoId}", id);
            throw;
        }
    }

    public async Task<(bool Success, string Message, int? FeriadoId)> CreateAsync(FeriadoViewModel model)
    {
        try
        {
            var jurisdiccion = model.Jurisdiccion ?? "Argentina";
            
            if (await FechaExistsAsync(model.Fecha, jurisdiccion))
            {
                return (false, "Ya existe un feriado en esa fecha para esa jurisdicción", null);
            }

            var feriado = new Feriado
            {
                Fecha = model.Fecha.Date,
                Descripcion = model.Descripcion.Trim(),
                Tipo = model.Tipo?.Trim(),
                Jurisdiccion = jurisdiccion
            };

            _context.Feriados.Add(feriado);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Feriado creado: {FeriadoId} - {Fecha}", feriado.Id, feriado.Fecha);
            return (true, "Feriado creado exitosamente", feriado.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear feriado");
            return (false, "Error al crear el feriado", null);
        }
    }

    public async Task<(bool Success, string Message)> UpdateAsync(FeriadoViewModel model)
    {
        try
        {
            var feriado = await _context.Feriados.FindAsync(model.Id);
            if (feriado == null)
            {
                return (false, "Feriado no encontrado");
            }

            var jurisdiccion = model.Jurisdiccion ?? "Argentina";

            if (await FechaExistsAsync(model.Fecha, jurisdiccion, model.Id))
            {
                return (false, "Ya existe otro feriado en esa fecha para esa jurisdicción");
            }

            feriado.Fecha = model.Fecha.Date;
            feriado.Descripcion = model.Descripcion.Trim();
            feriado.Tipo = model.Tipo?.Trim();
            feriado.Jurisdiccion = jurisdiccion;

            _context.Feriados.Update(feriado);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Feriado actualizado: {FeriadoId}", feriado.Id);
            return (true, "Feriado actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar feriado {FeriadoId}", model.Id);
            return (false, "Error al actualizar el feriado");
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        try
        {
            var feriado = await _context.Feriados.FindAsync(id);
            if (feriado == null)
            {
                return (false, "Feriado no encontrado");
            }

            _context.Feriados.Remove(feriado);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Feriado eliminado: {FeriadoId}", id);
            return (true, "Feriado eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar feriado {FeriadoId}", id);
            return (false, "Error al eliminar el feriado");
        }
    }

    public async Task<bool> FechaExistsAsync(DateTime fecha, string jurisdiccion, int? excludeId = null)
    {
        var query = _context.Feriados
            .Where(f => f.Fecha.Date == fecha.Date && f.Jurisdiccion == jurisdiccion);

        if (excludeId.HasValue)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<List<int>> GetAniosDisponiblesAsync()
    {
        try
        {
            var anios = await _context.Feriados
                .Select(f => f.Fecha.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return anios;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener años disponibles");
            throw;
        }
    }

    private FeriadoViewModel MapToViewModel(Feriado feriado)
    {
        return new FeriadoViewModel
        {
            Id = feriado.Id,
            Fecha = feriado.Fecha,
            Descripcion = feriado.Descripcion,
            Tipo = feriado.Tipo,
            Jurisdiccion = feriado.Jurisdiccion
        };
    }
}
