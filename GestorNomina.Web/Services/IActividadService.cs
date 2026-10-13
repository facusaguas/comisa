using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

/// <summary>
/// Interfaz para el servicio de gestión de actividades
/// </summary>
public interface IActividadService
{
    Task<List<ActividadViewModel>> GetAllAsync();
    Task<List<ActividadViewModel>> GetByTipoAsync(string tipo);
    Task<ActividadViewModel?> GetByIdAsync(int id);
    Task<ActividadViewModel?> GetByCodigoAsync(string codigo);
    Task<(bool Success, string Message, int? ActividadId)> CreateAsync(ActividadViewModel model);
    Task<(bool Success, string Message)> UpdateAsync(ActividadViewModel model);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null);
}
