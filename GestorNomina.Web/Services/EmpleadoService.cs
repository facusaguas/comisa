using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmpleadoService> _logger;

    public EmpleadoService(ApplicationDbContext context, ILogger<EmpleadoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<EmpleadoViewModel>> GetAllAsync()
    {
        try
        {
            var empleados = await _context.Empleados
                .Include(e => e.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Proyecto)
                .Include(e => e.Regimen)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            return empleados.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los empleados");
            throw;
        }
    }

    public async Task<List<EmpleadoViewModel>> GetActivosAsync()
    {
        try
        {
            var empleados = await _context.Empleados
                .Include(e => e.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Proyecto)
                .Include(e => e.Regimen)
                .Where(e => e.Activo)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            return empleados.Select(MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleados activos");
            throw;
        }
    }

    public async Task<EmpleadoViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var empleado = await _context.Empleados
                .Include(e => e.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Proyecto)
                .Include(e => e.Regimen)
                .FirstOrDefaultAsync(e => e.Id == id);

            return empleado != null ? MapToViewModel(empleado) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleado con ID {EmpleadoId}", id);
            throw;
        }
    }

    public async Task<EmpleadoViewModel?> GetByLegajoAsync(string legajo)
    {
        try
        {
            var empleado = await _context.Empleados
                .Include(e => e.EmpleadosProyectos.Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow))
                    .ThenInclude(ep => ep.Proyecto)
                .Include(e => e.Regimen)
                .FirstOrDefaultAsync(e => e.Legajo == legajo);

            return empleado != null ? MapToViewModel(empleado) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleado con Legajo {Legajo}", legajo);
            throw;
        }
    }

    public async Task<(bool Success, string Message, int? EmpleadoId)> CreateAsync(EmpleadoViewModel model)
    {
        try
        {
            if (await LegajoExistsAsync(model.Legajo))
            {
                return (false, "Ya existe un empleado con ese legajo", null);
            }

            var empleado = new Empleado
            {
                Legajo = model.Legajo.Trim(),
                Nombre = model.Nombre.Trim(),
                Apellido = model.Apellido.Trim(),
                DocumentoIdentidad = model.DocumentoIdentidad?.Trim(),
                Cargo = model.Cargo?.Trim(),
                RegimenId = model.RegimenId,
                FechaIngreso = model.FechaIngreso.HasValue 
                    ? DateTime.SpecifyKind(model.FechaIngreso.Value, DateTimeKind.Utc) 
                    : (DateTime?)null,
                Activo = model.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Empleado creado: {EmpleadoId} - {Nombre} {Apellido}", 
                empleado.Id, empleado.Nombre, empleado.Apellido);

            return (true, "Empleado creado exitosamente", empleado.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            return (false, "Error al crear el empleado", null);
        }
    }

    public async Task<(bool Success, string Message)> UpdateAsync(EmpleadoViewModel model)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(model.Id);
            if (empleado == null)
            {
                return (false, "Empleado no encontrado");
            }

            if (await LegajoExistsAsync(model.Legajo, model.Id))
            {
                return (false, "Ya existe otro empleado con ese legajo");
            }

            empleado.Legajo = model.Legajo.Trim();
            empleado.Nombre = model.Nombre.Trim();
            empleado.Apellido = model.Apellido.Trim();
            empleado.DocumentoIdentidad = model.DocumentoIdentidad?.Trim();
            empleado.Cargo = model.Cargo?.Trim();
            empleado.RegimenId = model.RegimenId;
            empleado.FechaIngreso = model.FechaIngreso.HasValue 
                ? DateTime.SpecifyKind(model.FechaIngreso.Value, DateTimeKind.Utc) 
                : (DateTime?)null;
            empleado.Activo = model.Activo;
            empleado.UpdatedAt = DateTime.UtcNow;

            _context.Empleados.Update(empleado);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Empleado actualizado: {EmpleadoId}", empleado.Id);
            return (true, "Empleado actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar empleado {EmpleadoId}", model.Id);
            return (false, "Error al actualizar el empleado");
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        try
        {
            var empleado = await _context.Empleados
                .Include(e => e.EmpleadosProyectos)
                .Include(e => e.HorasRegistrosRaw)
                .Include(e => e.HorasRegistrosAprobados)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empleado == null)
            {
                return (false, "Empleado no encontrado");
            }

            var tieneAsignacionesActivas = empleado.EmpleadosProyectos
                .Any(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow);
            if (tieneAsignacionesActivas)
            {
                return (false, "No se puede eliminar el empleado porque tiene asignaciones activas");
            }

            var tieneRegistrosHoras = empleado.HorasRegistrosRaw.Any() || empleado.HorasRegistrosAprobados.Any();
            if (tieneRegistrosHoras)
            {
                empleado.Activo = false;
                empleado.UpdatedAt = DateTime.UtcNow;
                _context.Empleados.Update(empleado);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Empleado desactivado: {EmpleadoId}", id);
                return (true, "Empleado desactivado (tiene registros de horas)");
            }

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Empleado eliminado: {EmpleadoId}", id);
            return (true, "Empleado eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar empleado {EmpleadoId}", id);
            return (false, "Error al eliminar el empleado");
        }
    }

    public async Task<(bool Success, string Message)> ToggleStatusAsync(int id)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return (false, "Empleado no encontrado");
            }

            empleado.Activo = !empleado.Activo;
            empleado.UpdatedAt = DateTime.UtcNow;

            _context.Empleados.Update(empleado);
            await _context.SaveChangesAsync();

            var estado = empleado.Activo ? "activado" : "desactivado";
            _logger.LogInformation("Empleado {Estado}: {EmpleadoId}", estado, id);

            return (true, $"Empleado {estado} exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del empleado {EmpleadoId}", id);
            return (false, "Error al cambiar el estado del empleado");
        }
    }

    public async Task<bool> LegajoExistsAsync(string legajo, int? excludeId = null)
    {
        try
        {
            var query = _context.Empleados.Where(e => e.Legajo == legajo);

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar legajo {Legajo}", legajo);
            throw;
        }
    }

    public async Task<List<string>> GetProyectosAsignadosAsync(int empleadoId)
    {
        try
        {
            var proyectos = await _context.Empleados
                .Where(e => e.Id == empleadoId)
                .SelectMany(e => e.EmpleadosProyectos)
                .Where(ep => ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow)
                .Select(ep => $"{ep.Proyecto.Codigo} - {ep.Proyecto.Nombre}")
                .ToListAsync();

            return proyectos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proyectos del empleado {EmpleadoId}", empleadoId);
            throw;
        }
    }

    private EmpleadoViewModel MapToViewModel(Empleado empleado)
    {
        var viewModel = new EmpleadoViewModel
        {
            Id = empleado.Id,
            Legajo = empleado.Legajo,
            Nombre = empleado.Nombre,
            Apellido = empleado.Apellido,
            DocumentoIdentidad = empleado.DocumentoIdentidad,
            Cargo = empleado.Cargo,
            RegimenId = empleado.RegimenId,
            FechaIngreso = empleado.FechaIngreso,
            Activo = empleado.Activo,
            CreatedAt = empleado.CreatedAt,
            UpdatedAt = empleado.UpdatedAt
        };

        if (empleado.Regimen != null)
        {
            viewModel.RegimenNombre = empleado.Regimen.Nombre;
        }

        if (empleado.EmpleadosProyectos != null && empleado.EmpleadosProyectos.Any())
        {
            viewModel.ProyectosAsignados = empleado.EmpleadosProyectos
                .Where(ep => (ep.FechaHasta == null || ep.FechaHasta > DateTime.UtcNow) && ep.Proyecto != null)
                .Select(ep => $"{ep.Proyecto.Codigo} - {ep.Proyecto.Nombre}")
                .ToList();
        }

        return viewModel;
    }
}
