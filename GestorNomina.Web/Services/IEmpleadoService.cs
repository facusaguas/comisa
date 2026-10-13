using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

/// <summary>
/// Interfaz para el servicio de gestión de empleados
/// </summary>
public interface IEmpleadoService
{
    Task<List<EmpleadoViewModel>> GetAllAsync();
    Task<List<EmpleadoViewModel>> GetActivosAsync();
    Task<EmpleadoViewModel?> GetByIdAsync(int id);
    Task<EmpleadoViewModel?> GetByLegajoAsync(string legajo);
    Task<(bool Success, string Message, int? EmpleadoId)> CreateAsync(EmpleadoViewModel model);
    Task<(bool Success, string Message)> UpdateAsync(EmpleadoViewModel model);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<(bool Success, string Message)> ToggleStatusAsync(int id);
    Task<bool> LegajoExistsAsync(string legajo, int? excludeId = null);
    Task<List<string>> GetProyectosAsignadosAsync(int empleadoId);
}
