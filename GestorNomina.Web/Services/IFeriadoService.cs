using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

/// <summary>
/// Interfaz para el servicio de gestión de feriados
/// </summary>
public interface IFeriadoService
{
    Task<List<FeriadoViewModel>> GetAllAsync();
    Task<List<FeriadoViewModel>> GetByAnioAsync(int anio);
    Task<List<FeriadoViewModel>> GetByJurisdiccionAsync(string jurisdiccion);
    Task<FeriadoViewModel?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int? FeriadoId)> CreateAsync(FeriadoViewModel model);
    Task<(bool Success, string Message)> UpdateAsync(FeriadoViewModel model);
    Task<(bool Success, string Message)> DeleteAsync(int id);
    Task<bool> FechaExistsAsync(DateTime fecha, string jurisdiccion, int? excludeId = null);
    Task<List<int>> GetAniosDisponiblesAsync();
}
