using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services;

public class RegimenLaboralService : IRegimenLaboralService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegimenLaboralService> _logger;

    public RegimenLaboralService(ApplicationDbContext context, ILogger<RegimenLaboralService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RegimenLaboralViewModel>> GetAllAsync()
    {
        try
        {
            var regimenes = await _context.RegimenesLaborales
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return regimenes.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener regímenes laborales");
            throw;
        }
    }

    public async Task<RegimenLaboralViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var regimen = await _context.RegimenesLaborales.FindAsync(id);
            return regimen != null ? MapToViewModel(regimen) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener régimen {RegimenId}", id);
            throw;
        }
    }

    public async Task<(bool Success, string Message, int? RegimenId)> CreateAsync(RegimenLaboralViewModel model)
    {
        try
        {
            if (await NombreExistsAsync(model.Nombre))
            {
                return (false, "Ya existe un régimen laboral con ese nombre", null);
            }

            var regimen = new RegimenLaboral
            {
                Nombre = model.Nombre.Trim(),
                DiasTrabajo = model.DiasTrabajo,
                DiasDescanso = model.DiasDescanso,
                MaxHorasDiarias = model.MaxHorasDiarias,
                HorasSemanalesLimite = model.HorasSemanalesLimite,
                Descripcion = model.Descripcion?.Trim()
            };

            _context.RegimenesLaborales.Add(regimen);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Régimen laboral creado: {RegimenId} - {Nombre}", regimen.Id, regimen.Nombre);
            return (true, "Régimen laboral creado exitosamente", regimen.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear régimen laboral");
            return (false, "Error al crear el régimen laboral", null);
        }
    }

    public async Task<(bool Success, string Message)> UpdateAsync(RegimenLaboralViewModel model)
    {
        try
        {
            var regimen = await _context.RegimenesLaborales.FindAsync(model.Id);
            if (regimen == null)
            {
                return (false, "Régimen laboral no encontrado");
            }

            if (await NombreExistsAsync(model.Nombre, model.Id))
            {
                return (false, "Ya existe otro régimen laboral con ese nombre");
            }

            regimen.Nombre = model.Nombre.Trim();
            regimen.DiasTrabajo = model.DiasTrabajo;
            regimen.DiasDescanso = model.DiasDescanso;
            regimen.MaxHorasDiarias = model.MaxHorasDiarias;
            regimen.HorasSemanalesLimite = model.HorasSemanalesLimite;
            regimen.Descripcion = model.Descripcion?.Trim();

            _context.RegimenesLaborales.Update(regimen);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Régimen laboral actualizado: {RegimenId}", regimen.Id);
            return (true, "Régimen laboral actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar régimen {RegimenId}", model.Id);
            return (false, "Error al actualizar el régimen laboral");
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        try
        {
            var regimen = await _context.RegimenesLaborales
                .Include(r => r.EmpleadosProyectosConRegimenEspecifico)
                .Include(r => r.Empleados)
                .Include(r => r.ProyectosConRegimenDefault)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (regimen == null)
            {
                return (false, "Régimen laboral no encontrado");
            }

            var tieneAsignaciones = regimen.EmpleadosProyectosConRegimenEspecifico.Any() ||
                                   regimen.Empleados.Any() ||
                                   regimen.ProyectosConRegimenDefault.Any();

            if (tieneAsignaciones)
            {
                return (false, "No se puede eliminar el régimen porque tiene asignaciones asociadas");
            }

            _context.RegimenesLaborales.Remove(regimen);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Régimen laboral eliminado: {RegimenId}", id);
            return (true, "Régimen laboral eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar régimen {RegimenId}", id);
            return (false, "Error al eliminar el régimen laboral");
        }
    }

    public async Task<bool> NombreExistsAsync(string nombre, int? excludeId = null)
    {
        var query = _context.RegimenesLaborales.Where(r => r.Nombre == nombre);
        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    private RegimenLaboralViewModel MapToViewModel(RegimenLaboral regimen)
    {
        return new RegimenLaboralViewModel
        {
            Id = regimen.Id,
            Nombre = regimen.Nombre,
            DiasTrabajo = regimen.DiasTrabajo,
            DiasDescanso = regimen.DiasDescanso,
            MaxHorasDiarias = regimen.MaxHorasDiarias,
            HorasSemanalesLimite = regimen.HorasSemanalesLimite,
            Descripcion = regimen.Descripcion
        };
    }
}
