using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services;

public interface INovedadService
{
    Task<List<Novedad>> GetAllAsync();
    Task<List<Novedad>> GetByEmpleadoAsync(int empleadoId);
    Task<List<Novedad>> GetByPeriodoAsync(DateTime periodo, int quincena);
    Task<List<Novedad>> GetFilteredAsync(string? legajo, string? cuil, int? proyectoId, DateTime? periodo, int? quincena, TipoNovedadLiquidacion? tipoNovedad);
    Task<Novedad?> GetByIdAsync(int id);
    Task<Novedad> CreateAsync(NovedadViewModel model);
    Task<Novedad> UpdateAsync(int id, NovedadViewModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExisteNovedadAsync(int empleadoId, DateTime periodo, int quincena, TipoNovedadLiquidacion tipoNovedad, int? excludeId = null);
}
