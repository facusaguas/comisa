using GestorNomina.Web.Filters;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorNomina.Web.Controllers;

[Authorize]
[SupervisorOrAdmin]
public class ProyectosController : BaseController
{
    private readonly IProyectoService _proyectoService;
    private readonly IRegimenLaboralService _regimenService;
    private readonly ILogger<ProyectosController> _logger;

    public ProyectosController(
        IProyectoService proyectoService,
        IRegimenLaboralService regimenService,
        ILogger<ProyectosController> logger)
    {
        _proyectoService = proyectoService;
        _regimenService = regimenService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewData["PageTitle"] = "Proyectos";
            ViewData["PagePretitle"] = "CONFIGURACIÓN";
            var proyectos = await _proyectoService.GetAllAsync();
            return View(proyectos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar proyectos");
            SetErrorMessage("Error al cargar la lista de proyectos");
            return View(new List<ProyectoViewModel>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                SetErrorMessage("Proyecto no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Detalle de Proyecto";
            ViewData["PagePretitle"] = "PROYECTOS";
            return View(proyecto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proyecto {ProyectoId}", id);
            SetErrorMessage("Error al cargar el proyecto");
            return RedirectToAction(nameof(Index));
        }
    }

    [AdminOnlyAttribute]
    public async Task<IActionResult> Create()
    {
        ViewData["PageTitle"] = "Nuevo Proyecto";
        ViewData["PagePretitle"] = "PROYECTOS";
        await LoadRegimenesAsync();
        return View(new ProyectoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnlyAttribute]
    public async Task<IActionResult> Create(ProyectoViewModel model)
    {
        ViewData["PageTitle"] = "Nuevo Proyecto";
        ViewData["PagePretitle"] = "PROYECTOS";

        if (!ModelState.IsValid)
        {
            await LoadRegimenesAsync();
            return View(model);
        }

        try
        {
            var (success, message, proyectoId) = await _proyectoService.CreateAsync(model);
            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Details), new { id = proyectoId });
            }
            SetErrorMessage(message);
            await LoadRegimenesAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear proyecto");
            SetErrorMessage("Error al crear el proyecto");
            await LoadRegimenesAsync();
            return View(model);
        }
    }

    [AdminOnlyAttribute]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                SetErrorMessage("Proyecto no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Editar Proyecto";
            ViewData["PagePretitle"] = "PROYECTOS";
            await LoadRegimenesAsync();
            return View(proyecto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar proyecto {ProyectoId}", id);
            SetErrorMessage("Error al cargar el proyecto");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnlyAttribute]
    public async Task<IActionResult> Edit(int id, ProyectoViewModel model)
    {
        if (id != model.Id)
            return RedirectToAction(nameof(Index));

        ViewData["PageTitle"] = "Editar Proyecto";
        ViewData["PagePretitle"] = "PROYECTOS";

        if (!ModelState.IsValid)
        {
            await LoadRegimenesAsync();
            return View(model);
        }

        try
        {
            var (success, message) = await _proyectoService.UpdateAsync(model);
            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            SetErrorMessage(message);
            await LoadRegimenesAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar proyecto {ProyectoId}", id);
            SetErrorMessage("Error al actualizar el proyecto");
            await LoadRegimenesAsync();
            return View(model);
        }
    }

    [AdminOnlyAttribute]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                SetErrorMessage("Proyecto no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Eliminar Proyecto";
            ViewData["PagePretitle"] = "PROYECTOS";
            return View(proyecto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar proyecto {ProyectoId}", id);
            SetErrorMessage("Error al cargar el proyecto");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [AdminOnlyAttribute]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var (success, message) = await _proyectoService.DeleteAsync(id);
            if (success)
                SetSuccessMessage(message);
            else
                SetErrorMessage(message);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar proyecto {ProyectoId}", id);
            SetErrorMessage("Error al eliminar el proyecto");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnlyAttribute]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var (success, message) = await _proyectoService.ToggleStatusAsync(id);
            if (success)
                SetSuccessMessage(message);
            else
                SetErrorMessage(message);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del proyecto {ProyectoId}", id);
            SetErrorMessage("Error al cambiar el estado del proyecto");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Carga la lista de regímenes laborales para los select
    /// </summary>
    private async Task LoadRegimenesAsync()
    {
        try
        {
            var regimenes = await _regimenService.GetAllAsync();
            ViewBag.Regimenes = new SelectList(regimenes, "Id", "Nombre");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar regímenes laborales");
            ViewBag.Regimenes = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }
}
