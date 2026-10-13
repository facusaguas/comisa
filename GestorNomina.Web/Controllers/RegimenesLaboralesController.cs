using GestorNomina.Web.Filters;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorNomina.Web.Controllers;

[Authorize]
[AdminOnlyAttribute]
public class RegimenesLaboralesController : BaseController
{
    private readonly IRegimenLaboralService _regimenService;
    private readonly ILogger<RegimenesLaboralesController> _logger;

    public RegimenesLaboralesController(IRegimenLaboralService regimenService, ILogger<RegimenesLaboralesController> logger)
    {
        _regimenService = regimenService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewData["PageTitle"] = "Regímenes Laborales";
            ViewData["PagePretitle"] = "CONFIGURACIÓN";
            var regimenes = await _regimenService.GetAllAsync();
            return View(regimenes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar regímenes laborales");
            SetErrorMessage("Error al cargar la lista de regímenes laborales");
            return View(new List<RegimenLaboralViewModel>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var regimen = await _regimenService.GetByIdAsync(id);
            if (regimen == null)
            {
                SetErrorMessage("Régimen laboral no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Detalle de Régimen Laboral";
            ViewData["PagePretitle"] = "REGÍMENES LABORALES";
            return View(regimen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener régimen {RegimenId}", id);
            SetErrorMessage("Error al cargar el régimen laboral");
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        ViewData["PageTitle"] = "Nuevo Régimen Laboral";
        ViewData["PagePretitle"] = "REGÍMENES LABORALES";
        return View(new RegimenLaboralViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegimenLaboralViewModel model)
    {
        ViewData["PageTitle"] = "Nuevo Régimen Laboral";
        ViewData["PagePretitle"] = "REGÍMENES LABORALES";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var (success, message, regimenId) = await _regimenService.CreateAsync(model);
            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Details), new { id = regimenId });
            }
            SetErrorMessage(message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear régimen laboral");
            SetErrorMessage("Error al crear el régimen laboral");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var regimen = await _regimenService.GetByIdAsync(id);
            if (regimen == null)
            {
                SetErrorMessage("Régimen laboral no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Editar Régimen Laboral";
            ViewData["PagePretitle"] = "REGÍMENES LABORALES";
            return View(regimen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar régimen {RegimenId}", id);
            SetErrorMessage("Error al cargar el régimen laboral");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RegimenLaboralViewModel model)
    {
        if (id != model.Id)
            return RedirectToAction(nameof(Index));

        ViewData["PageTitle"] = "Editar Régimen Laboral";
        ViewData["PagePretitle"] = "REGÍMENES LABORALES";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var (success, message) = await _regimenService.UpdateAsync(model);
            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            SetErrorMessage(message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar régimen {RegimenId}", id);
            SetErrorMessage("Error al actualizar el régimen laboral");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var regimen = await _regimenService.GetByIdAsync(id);
            if (regimen == null)
            {
                SetErrorMessage("Régimen laboral no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Eliminar Régimen Laboral";
            ViewData["PagePretitle"] = "REGÍMENES LABORALES";
            return View(regimen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar régimen {RegimenId}", id);
            SetErrorMessage("Error al cargar el régimen laboral");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var (success, message) = await _regimenService.DeleteAsync(id);
            if (success)
                SetSuccessMessage(message);
            else
                SetErrorMessage(message);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar régimen {RegimenId}", id);
            SetErrorMessage("Error al eliminar el régimen laboral");
            return RedirectToAction(nameof(Index));
        }
    }
}
