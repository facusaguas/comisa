using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services;

public class ProyectoService : IProyectoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProyectoService> _logger;

    public ProyectoService(ApplicationDbContext context, ILogger<ProyectoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ProyectoViewModel>> GetAllAsync()
    {
        try
        {
            var proyectos = await _context.Proyectos
                .Include(p => p.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Empleado)
                .Include(p => p.RegimenDefault)
                .OrderBy(p => p.Codigo)
                .ToListAsync();

            return proyectos.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proyectos");
            throw;
        }
    }

    public async Task<List<ProyectoViewModel>> GetActivosAsync()
    {
        try
        {
            var proyectos = await _context.Proyectos
                .Include(p => p.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Empleado)
                .Include(p => p.RegimenDefault)
                .Where(p => p.Activo)
                .OrderBy(p => p.Codigo)
                .ToListAsync();

            return proyectos.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proyectos activos");
            throw;
        }
    }

    public async Task<ProyectoViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var proyecto = await _context.Proyectos
                .Include(p => p.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Empleado)
                .Include(p => p.RegimenDefault)
                .FirstOrDefaultAsync(p => p.Id == id);

            return proyecto != null ? MapToViewModel(proyecto) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proyecto {ProyectoId}", id);
            throw;
        }
    }

    public async Task<ProyectoViewModel?> GetByCodigoAsync(string codigo)
    {
        try
        {
            var proyecto = await _context.Proyectos
                .Include(p => p.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Empleado)
                .Include(p => p.RegimenDefault)
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            return proyecto != null ? MapToViewModel(proyecto) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proyecto con código {Codigo}", codigo);
            throw;
        }
    }

    public async Task<(bool Success, string Message, int? ProyectoId)> CreateAsync(ProyectoViewModel model)
    {
        try
        {
            if (await CodigoExistsAsync(model.Codigo))
            {
                return (false, "Ya existe un proyecto con ese código", null);
            }

            var proyecto = new Proyecto
            {
                Codigo = model.Codigo.Trim().ToUpper(),
                Nombre = model.Nombre.Trim(),
                Ubicacion = model.Ubicacion?.Trim(),
                RegimenDefaultId = model.RegimenDefaultId,
                Activo = model.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Proyectos.Add(proyecto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proyecto creado: {ProyectoId} - {Codigo}", proyecto.Id, proyecto.Codigo);
            return (true, "Proyecto creado exitosamente", proyecto.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear proyecto");
            return (false, "Error al crear el proyecto", null);
        }
    }

    public async Task<(bool Success, string Message)> UpdateAsync(ProyectoViewModel model)
    {
        try
        {
            var proyecto = await _context.Proyectos.FindAsync(model.Id);
            if (proyecto == null)
            {
                return (false, "Proyecto no encontrado");
            }

            if (await CodigoExistsAsync(model.Codigo, model.Id))
            {
                return (false, "Ya existe otro proyecto con ese código");
            }

            proyecto.Codigo = model.Codigo.Trim().ToUpper();
            proyecto.Nombre = model.Nombre.Trim();
            proyecto.Ubicacion = model.Ubicacion?.Trim();
            proyecto.RegimenDefaultId = model.RegimenDefaultId;
            proyecto.Activo = model.Activo;
            proyecto.UpdatedAt = DateTime.UtcNow;

            _context.Proyectos.Update(proyecto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proyecto actualizado: {ProyectoId}", proyecto.Id);
            return (true, "Proyecto actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar proyecto {ProyectoId}", model.Id);
            return (false, "Error al actualizar el proyecto");
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        try
        {
            var proyecto = await _context.Proyectos
                .Include(p => p.EmpleadosProyectos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proyecto == null)
            {
                return (false, "Proyecto no encontrado");
            }

            if (proyecto.EmpleadosProyectos.Any(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
            {
                return (false, "No se puede eliminar el proyecto porque tiene empleados asignados activos");
            }

            if (proyecto.EmpleadosProyectos.Any())
            {
                proyecto.Activo = false;
                proyecto.UpdatedAt = DateTime.UtcNow;
                _context.Proyectos.Update(proyecto);
            }
            else
            {
                _context.Proyectos.Remove(proyecto);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Proyecto eliminado/desactivado: {ProyectoId}", id);
            return (true, "Proyecto eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar proyecto {ProyectoId}", id);
            return (false, "Error al eliminar el proyecto");
        }
    }

    public async Task<(bool Success, string Message)> ToggleStatusAsync(int id)
    {
        try
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return (false, "Proyecto no encontrado");
            }

            proyecto.Activo = !proyecto.Activo;
            proyecto.UpdatedAt = DateTime.UtcNow;

            _context.Proyectos.Update(proyecto);
            await _context.SaveChangesAsync();

            var estado = proyecto.Activo ? "activado" : "desactivado";
            _logger.LogInformation("Proyecto {Estado}: {ProyectoId}", estado, id);
            return (true, $"Proyecto {estado} exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del proyecto {ProyectoId}", id);
            return (false, "Error al cambiar el estado del proyecto");
        }
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        var query = _context.Proyectos.Where(p => p.Codigo == codigo);
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<List<string>> GetEmpleadosAsignadosAsync(int proyectoId)
    {
        return await _context.Proyectos
            .Where(p => p.Id == proyectoId)
            .SelectMany(p => p.EmpleadosProyectos)
            .Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow)
            .Select(ep => $"{ep.Empleado.Apellido}, {ep.Empleado.Nombre} ({ep.Empleado.Legajo})")
            .ToListAsync();
    }

    public async Task<int> GetCantidadEmpleadosAsync(int proyectoId)
    {
        return await _context.Proyectos
            .Where(p => p.Id == proyectoId)
            .SelectMany(p => p.EmpleadosProyectos)
            .Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow)
            .CountAsync();
    }

    private ProyectoViewModel MapToViewModel(Proyecto proyecto)
    {
        var viewModel = new ProyectoViewModel
        {
            Id = proyecto.Id,
            Codigo = proyecto.Codigo,
            Nombre = proyecto.Nombre,
            Ubicacion = proyecto.Ubicacion,
            RegimenDefaultId = proyecto.RegimenDefaultId,
            Activo = proyecto.Activo,
            CreatedAt = proyecto.CreatedAt,
            UpdatedAt = proyecto.UpdatedAt
        };

        if (proyecto.RegimenDefault != null)
        {
            viewModel.RegimenDefaultNombre = proyecto.RegimenDefault.Nombre;
        }

        if (proyecto.EmpleadosProyectos != null)
        {
            viewModel.CantidadEmpleados = proyecto.EmpleadosProyectos
                .Count(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow);
            viewModel.EmpleadosAsignados = proyecto.EmpleadosProyectos
                .Where(ep => (ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow) && ep.Empleado != null)
                .Select(ep => $"{ep.Empleado.Apellido}, {ep.Empleado.Nombre}")
                .ToList();
        }

        return viewModel;
    }
}
