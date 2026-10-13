using GestorNomina.Web.Filters;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorNomina.Web.Controllers;

[Authorize]
[SupervisorOrAdmin]
public class FeriadosController : BaseController
{
    private readonly IFeriadoService _feriadoService;
    private readonly ILogger<FeriadosController> _logger;

    public FeriadosController(IFeriadoService feriadoService, ILogger<FeriadosController> logger)
    {
        _feriadoService = feriadoService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? anio)
    {
        try
        {
            ViewData["PageTitle"] = "Feriados";
            ViewData["PagePretitle"] = "CONFIGURACIÓN";

            var feriados = anio.HasValue 
                ? await _feriadoService.GetByAnioAsync(anio.Value)
                : await _feriadoService.GetAllAsync();

            var aniosDisponibles = await _feriadoService.GetAniosDisponiblesAsync();
            ViewBag.AniosDisponibles = aniosDisponibles;
            ViewBag.AnioSeleccionado = anio;

            return View(feriados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar feriados");
            SetErrorMessage("Error al cargar la lista de feriados");
            return View(new List<FeriadoViewModel>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var feriado = await _feriadoService.GetByIdAsync(id);
            if (feriado == null)
            {
                SetErrorMessage("Feriado no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Detalle de Feriado";
            ViewData["PagePretitle"] = "FERIADOS";
            return View(feriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener feriado {FeriadoId}", id);
            SetErrorMessage("Error al cargar el feriado");
            return RedirectToAction(nameof(Index));
        }
    }

    [AdminOnlyAttribute]
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "Nuevo Feriado";
        ViewData["PagePretitle"] = "FERIADOS";
        return View(new FeriadoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnlyAttribute]
    public async Task<IActionResult> Create(FeriadoViewModel model)
    {
        ViewData["PageTitle"] = "Nuevo Feriado";
        ViewData["PagePretitle"] = "FERIADOS";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var (success, message, feriadoId) = await _feriadoService.CreateAsync(model);
            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Index));
            }
            SetErrorMessage(message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear feriado");
            SetErrorMessage("Error al crear el feriado");
            return View(model);
        }
    }

    [AdminOnlyAttribute]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var feriado = await _feriadoService.GetByIdAsync(id);
            if (feriado == null)
            {
                SetErrorMessage("Feriado no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Editar Feriado";
            ViewData["PagePretitle"] = "FERIADOS";
            return View(feriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar feriado {FeriadoId}", id);
            SetErrorMessage("Error al cargar el feriado");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnlyAttribute]
    public async Task<IActionResult> Edit(int id, FeriadoViewModel model)
    {
        if (id != model.Id)
            return RedirectToAction(nameof(Index));

        ViewData["PageTitle"] = "Editar Feriado";
        ViewData["PagePretitle"] = "FERIADOS";

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var (success, message) = await _feriadoService.UpdateAsync(model);
            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Index));
            }
            SetErrorMessage(message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar feriado {FeriadoId}", id);
            SetErrorMessage("Error al actualizar el feriado");
            return View(model);
        }
    }

    [AdminOnlyAttribute]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var feriado = await _feriadoService.GetByIdAsync(id);
            if (feriado == null)
            {
                SetErrorMessage("Feriado no encontrado");
                return RedirectToAction(nameof(Index));
            }
            ViewData["PageTitle"] = "Eliminar Feriado";
            ViewData["PagePretitle"] = "FERIADOS";
            return View(feriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar feriado {FeriadoId}", id);
            SetErrorMessage("Error al cargar el feriado");
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
            var (success, message) = await _feriadoService.DeleteAsync(id);
            if (success)
                SetSuccessMessage(message);
            else
                SetErrorMessage(message);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar feriado {FeriadoId}", id);
            SetErrorMessage("Error al eliminar el feriado");
            return RedirectToAction(nameof(Index));
        }
    }
}
