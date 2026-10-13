using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

/// <summary>
/// Interfaz para el servicio de gestión de regímenes laborales
/// </summary>
public interface IRegimenLaboralService
{
    Task<List<RegimenLaboralViewModel>> GetAllAsync();
    Task<RegimenLaboralViewModel?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int? RegimenId)> CreateAsync(RegimenLaboralViewModel model);
    Task<(bool Success, string Message)> UpdateAsync(RegimenLaboralViewModel model);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<bool> NombreExistsAsync(string nombre, int? excludeId = null);
}
