using GestorNomina.Web.Filters;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorNomina.Web.Controllers;

[Authorize]
[AdminOnlyAttribute]
public class ActividadesController : BaseController
{
    private readonly IActividadService _actividadService;
    private readonly ILogger<ActividadesController> _logger;

    public ActividadesController(IActividadService actividadService, ILogger<ActividadesController> logger)
    {
        _actividadService = actividadService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewData["PageTitle"] = "Actividades";
            ViewData["PagePretitle"] = "CONFIGURACIÓN";
            var actividades = await _actividadService.GetAllAsync();
            return View(actividades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar actividades");
            SetErrorMessage("Error al cargar la lista de actividades");
            return View(new List<ActividadViewModel>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var actividad = await _actividadService.GetByIdAsync(id);
            if (actividad == null)
            {
                SetErrorMessage("Actividad no encontrada");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Detalle de Actividad";
            ViewData["PagePretitle"] = "ACTIVIDADES";
            return View(actividad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actividad {ActividadId}", id);
            SetErrorMessage("Error al cargar la actividad");
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        ViewData["PageTitle"] = "Nueva Actividad";
        ViewData["PagePretitle"] = "ACTIVIDADES";
        return View(new ActividadViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActividadViewModel model)
    {
        ViewData["PageTitle"] = "Nueva Actividad";
        ViewData["PagePretitle"] = "ACTIVIDADES";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var (success, message, actividadId) = await _actividadService.CreateAsync(model);
            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Details), new { id = actividadId });
            }
            SetErrorMessage(message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear actividad");
            SetErrorMessage("Error al crear la actividad");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var actividad = await _actividadService.GetByIdAsync(id);
            if (actividad == null)
            {
                SetErrorMessage("Actividad no encontrada");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Editar Actividad";
            ViewData["PagePretitle"] = "ACTIVIDADES";
            return View(actividad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar actividad {ActividadId}", id);
            SetErrorMessage("Error al cargar la actividad");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActividadViewModel model)
    {
        if (id != model.Id)
            return RedirectToAction(nameof(Index));

        ViewData["PageTitle"] = "Editar Actividad";
        ViewData["PagePretitle"] = "ACTIVIDADES";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var (success, message) = await _actividadService.UpdateAsync(model);
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
            _logger.LogError(ex, "Error al actualizar actividad {ActividadId}", id);
            SetErrorMessage("Error al actualizar la actividad");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var actividad = await _actividadService.GetByIdAsync(id);
            if (actividad == null)
            {
                SetErrorMessage("Actividad no encontrada");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Eliminar Actividad";
            ViewData["PagePretitle"] = "ACTIVIDADES";
            return View(actividad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar actividad {ActividadId}", id);
            SetErrorMessage("Error al cargar la actividad");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var (success, message) = await _actividadService.DeleteAsync(id);
            if (success)
                SetSuccessMessage(message);
            else
                SetErrorMessage(message);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar actividad {ActividadId}", id);
            SetErrorMessage("Error al eliminar la actividad");
            return RedirectToAction(nameof(Index));
        }
    }
}
