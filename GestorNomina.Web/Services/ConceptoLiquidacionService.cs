using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Services;

public class ConceptoLiquidacionService : IConceptoLiquidacionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConceptoLiquidacionService> _logger;

    public ConceptoLiquidacionService(
        ApplicationDbContext context,
        ILogger<ConceptoLiquidacionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ConceptoLiquidacion>> GetAllAsync()
    {
        return await _context.ConceptosLiquidacion
            .OrderBy(c => c.TipoConcepto)
            .ThenBy(c => c.Codigo)
            .ToListAsync();
    }

    public async Task<ConceptoLiquidacion?> GetByIdAsync(int id)
    {
        return await _context.ConceptosLiquidacion
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ConceptoLiquidacion?> GetByCodigoAsync(string codigo)
    {
        return await _context.ConceptosLiquidacion
            .FirstOrDefaultAsync(c => c.Codigo == codigo);
    }

    public async Task<ConceptoLiquidacion> CreateAsync(ConceptoLiquidacionViewModel model)
    {
        // Validar que no exista el código
        if (await ExisteCodigoAsync(model.Codigo))
        {
            throw new InvalidOperationException($"Ya existe un concepto con el código '{model.Codigo}'");
        }

        var concepto = new ConceptoLiquidacion
        {
            Codigo = model.Codigo.Trim().ToUpper(),
            Nombre = model.Nombre.Trim(),
            TipoConcepto = model.TipoConcepto
        };

        _context.ConceptosLiquidacion.Add(concepto);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Concepto de liquidación creado: {Codigo} - {Nombre}", concepto.Codigo, concepto.Nombre);

        return concepto;
    }

    public async Task<ConceptoLiquidacion> UpdateAsync(int id, ConceptoLiquidacionViewModel model)
    {
        var concepto = await GetByIdAsync(id);
        if (concepto == null)
        {
            throw new KeyNotFoundException($"No se encontró el concepto con ID {id}");
        }

        // Validar que no exista el código en otro registro
        if (await ExisteCodigoAsync(model.Codigo, id))
        {
            throw new InvalidOperationException($"Ya existe otro concepto con el código '{model.Codigo}'");
        }

        concepto.Codigo = model.Codigo.Trim().ToUpper();
        concepto.Nombre = model.Nombre.Trim();
        concepto.TipoConcepto = model.TipoConcepto;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Concepto de liquidación actualizado: {Codigo} - {Nombre}", concepto.Codigo, concepto.Nombre);

        return concepto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var concepto = await GetByIdAsync(id);
        if (concepto == null)
        {
            return false;
        }

        // Verificar si tiene recibos asociados
        var tieneRecibos = await _context.ReciboDetalles
            .AnyAsync(rd => rd.ConceptoId == id);

        if (tieneRecibos)
        {
            throw new InvalidOperationException("No se puede eliminar el concepto porque tiene recibos asociados");
        }

        _context.ConceptosLiquidacion.Remove(concepto);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Concepto de liquidación eliminado: {Codigo} - {Nombre}", concepto.Codigo, concepto.Nombre);

        return true;
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null)
    {
        var query = _context.ConceptosLiquidacion
            .Where(c => c.Codigo == codigo.Trim().ToUpper());

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
