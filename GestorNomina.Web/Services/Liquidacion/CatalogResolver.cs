using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Implementación del servicio de resolución de catálogos
/// </summary>
public class CatalogResolver : ICatalogResolver
{
    private readonly ApplicationDbContext _context;
    private Dictionary<string, int> _tiposNovedad = new();
    private Dictionary<string, int> _conceptos = new();
    private Dictionary<string, int> _tiposLiquidacion = new();
    private Dictionary<string, int> _proyectos = new();
    private Dictionary<string, int> _actividades = new();
    private Dictionary<string, int> _categorias = new();
    private Dictionary<string, int> _convenios = new();
    private Dictionary<string, int> _disciplinas = new();
    private bool _isLoaded = false;

    public bool IsLoaded => _isLoaded;

    public CatalogResolver(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LoadCatalogsAsync()
    {
        if (_isLoaded) return;

        // Cargar Tipos de Novedad
        var tiposNovedad = await _context.TiposNovedad
            .Select(tn => new { tn.Id, tn.Codigo })
            .ToListAsync();
        _tiposNovedad = tiposNovedad.ToDictionary(x => x.Codigo, x => x.Id);

        // Cargar Conceptos de Liquidación
        var conceptos = await _context.ConceptosLiquidacion
            .Select(cl => new { cl.Id, cl.CodigoTango })
            .ToListAsync();
        _conceptos = conceptos.ToDictionary(x => x.CodigoTango, x => x.Id);

        // Cargar Tipos de Liquidación
        var tiposLiquidacion = await _context.TiposLiquidacion
            .Select(tl => new { tl.Id, tl.Codigo })
            .ToListAsync();
        _tiposLiquidacion = tiposLiquidacion.ToDictionary(x => x.Codigo, x => x.Id);

        // Cargar Proyectos
        var proyectos = await _context.Proyectos
            .Select(p => new { p.Id, p.Codigo })
            .ToListAsync();
        _proyectos = proyectos.ToDictionary(x => x.Codigo, x => x.Id);

        // Cargar Actividades
        var actividades = await _context.Actividades
            .Select(a => new { a.Id, a.Codigo })
            .ToListAsync();
        _actividades = actividades.ToDictionary(x => x.Codigo, x => x.Id);

        // Cargar Categorías
        var categorias = await _context.Categorias
            .Select(c => new { c.Id, c.Nombre })
            .ToListAsync();
        _categorias = categorias.ToDictionary(x => x.Nombre, x => x.Id);

        // Cargar Convenios
        var convenios = await _context.Convenios
            .Select(c => new { c.Id, c.Codigo })
            .ToListAsync();
        _convenios = convenios.ToDictionary(x => x.Codigo, x => x.Id);

        // Cargar Disciplinas
        var disciplinas = await _context.Disciplinas
            .Select(d => new { d.Id, d.Codigo })
            .ToListAsync();
        _disciplinas = disciplinas.ToDictionary(x => x.Codigo, x => x.Id);

        _isLoaded = true;
    }

    public int GetTipoNovedadId(CodigoTipoNovedad codigo)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Los catálogos no han sido cargados. Llame a LoadCatalogsAsync() primero.");

        var key = codigo.ToString();
        if (!_tiposNovedad.TryGetValue(key, out var id))
            throw new KeyNotFoundException($"No se encontró el tipo de novedad con código: {key}");

        return id;
    }

    public int GetConceptoId(CodigoConceptoTango codigo)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Los catálogos no han sido cargados. Llame a LoadCatalogsAsync() primero.");

        var key = ((int)codigo).ToString();
        if (!_conceptos.TryGetValue(key, out var id))
            throw new KeyNotFoundException($"No se encontró el concepto con código: {key}");

        return id;
    }

    public int GetTipoLiquidacionId(CodigoTipoLiquidacion codigo)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Los catálogos no han sido cargados. Llame a LoadCatalogsAsync() primero.");

        var key = codigo.ToString();
        if (!_tiposLiquidacion.TryGetValue(key, out var id))
            throw new KeyNotFoundException($"No se encontró el tipo de liquidación con código: {key}");

        return id;
    }

    public async Task<int?> GetProyectoIdByCodigo(string codigo)
    {
        if (!_isLoaded)
            await LoadCatalogsAsync();

        return _proyectos.TryGetValue(codigo, out var id) ? id : null;
    }

    public async Task<int?> GetActividadIdByCodigo(string codigo)
    {
        if (!_isLoaded)
            await LoadCatalogsAsync();

        return _actividades.TryGetValue(codigo, out var id) ? id : null;
    }

    public int GetCategoriaIdByNombre(string nombre)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Los catálogos no han sido cargados. Llame a LoadCatalogsAsync() primero.");

        if (!_categorias.TryGetValue(nombre, out var id))
            throw new KeyNotFoundException($"No se encontró la categoría con nombre: {nombre}");

        return id;
    }

    public int GetConvenioIdByCodigo(string codigo)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Los catálogos no han sido cargados. Llame a LoadCatalogsAsync() primero.");

        if (!_convenios.TryGetValue(codigo, out var id))
            throw new KeyNotFoundException($"No se encontró el convenio con código: {codigo}");

        return id;
    }

    public int GetDisciplinaIdByCodigo(string codigo)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Los catálogos no han sido cargados. Llame a LoadCatalogsAsync() primero.");

        if (!_disciplinas.TryGetValue(codigo, out var id))
            throw new KeyNotFoundException($"No se encontró la disciplina con código: {codigo}");

        return id;
    }
}
