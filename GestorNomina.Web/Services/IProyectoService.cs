using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

/// <summary>
/// Interfaz para el servicio de gestión de proyectos
/// </summary>
public interface IProyectoService
{
    Task<List<ProyectoViewModel>> GetAllAsync();
    Task<List<ProyectoViewModel>> GetActivosAsync();
    Task<ProyectoViewModel?> GetByIdAsync(int id);
    Task<ProyectoViewModel?> GetByCodigoAsync(string codigo);
    Task<(bool Success, string Message, int? ProyectoId)> CreateAsync(ProyectoViewModel model);
    Task<(bool Success, string Message)> UpdateAsync(ProyectoViewModel model);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<(bool Success, string Message)> ToggleStatusAsync(int id);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
    Task<List<string>> GetEmpleadosAsignadosAsync(int proyectoId);
    Task<int> GetCantidadEmpleadosAsync(int proyectoId);
}
