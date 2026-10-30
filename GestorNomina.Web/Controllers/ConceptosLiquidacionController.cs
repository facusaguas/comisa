using Microsoft.AspNetCore.Mvc;
using GestorNomina.Web.Filters;
using GestorNomina.Web.Services;
using GestorNomina.Web.Models.ViewModels;

namespace GestorNomina.Web.Controllers;

public class ConceptosLiquidacionController : BaseController
{
    private readonly IConceptoLiquidacionService _conceptoService;
    private readonly ILogger<ConceptosLiquidacionController> _logger;

    public ConceptosLiquidacionController(
        IConceptoLiquidacionService conceptoService,
        ILogger<ConceptosLiquidacionController> logger)
    {
        _conceptoService = conceptoService;
        _logger = logger;
    }

    // GET: ConceptosLiquidacion
    public async Task<IActionResult> Index()
    {
        try
        {
            var conceptos = await _conceptoService.GetAllAsync();
            return View(conceptos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los conceptos de liquidación");
            TempData["Error"] = "Error al cargar los conceptos de liquidación";
            return View(new List<Models.Entities.ConceptoLiquidacion>());
        }
    }

    // GET: ConceptosLiquidacion/Create
    [AdminOnly]
    public IActionResult Create()
    {
        return View(new ConceptoLiquidacionViewModel());
    }

    // POST: ConceptosLiquidacion/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnly]
    public async Task<IActionResult> Create(ConceptoLiquidacionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _conceptoService.CreateAsync(model);
            TempData["Success"] = "Concepto de liquidación creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear concepto de liquidación");
            ModelState.AddModelError(string.Empty, "Error al crear el concepto de liquidación");
            return View(model);
        }
    }

    // GET: ConceptosLiquidacion/Edit/5
    [AdminOnly]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var concepto = await _conceptoService.GetByIdAsync(id);
            if (concepto == null)
            {
                TempData["Error"] = "Concepto no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var model = new ConceptoLiquidacionViewModel
            {
                Id = concepto.Id,
                Codigo = concepto.Codigo,
                Nombre = concepto.Nombre,
                TipoConcepto = concepto.TipoConcepto
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar concepto para edición");
            TempData["Error"] = "Error al cargar el concepto";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: ConceptosLiquidacion/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnly]
    public async Task<IActionResult> Edit(int id, ConceptoLiquidacionViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _conceptoService.UpdateAsync(id, model);
            TempData["Success"] = "Concepto de liquidación actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Concepto no encontrado";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar concepto de liquidación");
            ModelState.AddModelError(string.Empty, "Error al actualizar el concepto de liquidación");
            return View(model);
        }
    }

    // POST: ConceptosLiquidacion/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnly]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _conceptoService.DeleteAsync(id);
            if (result)
            {
                TempData["Success"] = "Concepto de liquidación eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "Concepto no encontrado";
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar concepto de liquidación");
            TempData["Error"] = "Error al eliminar el concepto de liquidación";
        }

        return RedirectToAction(nameof(Index));
    }
}
