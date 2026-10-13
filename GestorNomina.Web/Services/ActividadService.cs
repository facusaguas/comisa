using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services;

public class ActividadService : IActividadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ActividadService> _logger;

    public ActividadService(ApplicationDbContext context, ILogger<ActividadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ActividadViewModel>> GetAllAsync()
    {
        try
        {
            var actividades = await _context.Actividades
                .OrderBy(a => a.Codigo)
                .ToListAsync();

            return actividades.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actividades");
            throw;
        }
    }

    public async Task<List<ActividadViewModel>> GetByTipoAsync(string tipo)
    {
        try
        {
            var query = _context.Actividades.AsQueryable();

            query = tipo.ToLower() switch
            {
                "efectiva" => query.Where(a => a.EsHoraEfectiva),
                "licencia" => query.Where(a => a.EsLicencia),
                "franco" => query.Where(a => a.EsFranco),
                _ => query
            };

            var actividades = await query.OrderBy(a => a.Codigo).ToListAsync();
            return actividades.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actividades por tipo {Tipo}", tipo);
            throw;
        }
    }

    public async Task<ActividadViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var actividad = await _context.Actividades.FindAsync(id);
            return actividad != null ? MapToViewModel(actividad) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actividad {ActividadId}", id);
            throw;
        }
    }

    public async Task<ActividadViewModel?> GetByCodigoAsync(string codigo)
    {
        try
        {
            var actividad = await _context.Actividades
                .FirstOrDefaultAsync(a => a.Codigo == codigo);
            return actividad != null ? MapToViewModel(actividad) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actividad con código {Codigo}", codigo);
            throw;
        }
    }

    public async Task<(bool Success, string Message, int? ActividadId)> CreateAsync(ActividadViewModel model)
    {
        try
        {
            if (await CodigoExistsAsync(model.Codigo))
            {
                return (false, "Ya existe una actividad con ese código", null);
            }

            var actividad = new Actividad
            {
                Codigo = model.Codigo.Trim().ToUpper(),
                Descripcion = model.Descripcion.Trim(),
                EsHoraEfectiva = model.EsHoraEfectiva,
                EsLicencia = model.EsLicencia,
                EsFranco = model.EsFranco
            };

            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Actividad creada: {ActividadId} - {Codigo}", actividad.Id, actividad.Codigo);
            return (true, "Actividad creada exitosamente", actividad.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear actividad");
            return (false, "Error al crear la actividad", null);
        }
    }

    public async Task<(bool Success, string Message)> UpdateAsync(ActividadViewModel model)
    {
        try
        {
            var actividad = await _context.Actividades.FindAsync(model.Id);
            if (actividad == null)
            {
                return (false, "Actividad no encontrada");
            }

            if (await CodigoExistsAsync(model.Codigo, model.Id))
            {
                return (false, "Ya existe otra actividad con ese código");
            }

            actividad.Codigo = model.Codigo.Trim().ToUpper();
            actividad.Descripcion = model.Descripcion.Trim();
            actividad.EsHoraEfectiva = model.EsHoraEfectiva;
            actividad.EsLicencia = model.EsLicencia;
            actividad.EsFranco = model.EsFranco;

            _context.Actividades.Update(actividad);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Actividad actualizada: {ActividadId}", actividad.Id);
            return (true, "Actividad actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar actividad {ActividadId}", model.Id);
            return (false, "Error al actualizar la actividad");
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        try
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return (false, "Actividad no encontrada");
            }

            var tieneRegistros = await _context.HorasRegistrosRaw.AnyAsync(h => h.ActividadId == id) ||
                                 await _context.HorasRegistrosAprobados.AnyAsync(h => h.ActividadId == id);

            if (tieneRegistros)
            {
                return (false, "No se puede eliminar la actividad porque tiene registros de horas asociados");
            }

            _context.Actividades.Remove(actividad);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Actividad eliminada: {ActividadId}", id);
            return (true, "Actividad eliminada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar actividad {ActividadId}", id);
            return (false, "Error al eliminar la actividad");
        }
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        var query = _context.Actividades.Where(a => a.Codigo == codigo);
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    private ActividadViewModel MapToViewModel(Actividad actividad)
    {
        return new ActividadViewModel
        {
            Id = actividad.Id,
            Codigo = actividad.Codigo,
            Descripcion = actividad.Descripcion,
            EsHoraEfectiva = actividad.EsHoraEfectiva,
            EsLicencia = actividad.EsLicencia,
            EsFranco = actividad.EsFranco
        };
    }
}
