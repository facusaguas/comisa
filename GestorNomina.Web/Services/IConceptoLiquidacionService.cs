using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

public interface IConceptoLiquidacionService
{
    Task<List<ConceptoLiquidacion>> GetAllAsync();
    Task<ConceptoLiquidacion?> GetByIdAsync(int id);
    Task<ConceptoLiquidacion?> GetByCodigoAsync(string codigo);
    Task<ConceptoLiquidacion> CreateAsync(ConceptoLiquidacionViewModel model);
    Task<ConceptoLiquidacion> UpdateAsync(int id, ConceptoLiquidacionViewModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null);
}
