using GestorNomina.Web.Filters;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorNomina.Web.Controllers;

[Authorize]
[AdminOnlyAttribute]
public class EmpleadosController : BaseController
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IRegimenLaboralService _regimenService;
    private readonly IProyectoService _proyectoService;
    private readonly ILogger<EmpleadosController> _logger;

    public EmpleadosController(
        IEmpleadoService empleadoService,
        IRegimenLaboralService regimenService,
        IProyectoService proyectoService,
        ILogger<EmpleadosController> logger)
    {
        _empleadoService = empleadoService;
        _regimenService = regimenService;
        _proyectoService = proyectoService;
        _logger = logger;
    }

    // GET: Empleados
    public async Task<IActionResult> Index()
    {
        try
        {
            ViewData["PageTitle"] = "Empleados";
            ViewData["PagePretitle"] = "CONFIGURACIÓN";

            var empleados = await _empleadoService.GetAllAsync();
            return View(empleados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de empleados");
            SetErrorMessage("Error al cargar la lista de empleados");
            return View(new List<EmpleadoViewModel>());
        }
    }

    // GET: Empleados/Details/5
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            if (empleado == null)
            {
                SetErrorMessage("Empleado no encontrado");
                return RedirectToAction(nameof(Index));
            }

            ViewData["PageTitle"] = "Detalle de Empleado";
            ViewData["PagePretitle"] = "EMPLEADOS";

            return View(empleado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle del empleado {EmpleadoId}", id);
            SetErrorMessage("Error al cargar el detalle del empleado");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Empleados/Create
    public async Task<IActionResult> Create()
    {
        ViewData["PageTitle"] = "Nuevo Empleado";
        ViewData["PagePretitle"] = "EMPLEADOS";

        await LoadSelectListsAsync();
        return View(new EmpleadoViewModel());
    }

    // POST: Empleados/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmpleadoViewModel model)
    {
        ViewData["PageTitle"] = "Nuevo Empleado";
        ViewData["PagePretitle"] = "EMPLEADOS";

        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return View(model);
        }

        try
        {
            var (success, message, empleadoId) = await _empleadoService.CreateAsync(model);

            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Details), new { id = empleadoId });
            }

            SetErrorMessage(message);
            await LoadSelectListsAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            SetErrorMessage("Error inesperado al crear el empleado");
            await LoadSelectListsAsync();
            return View(model);
        }
    }

    // GET: Empleados/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            if (empleado == null)
            {
                SetErrorMessage("Empleado no encontrado");
                return RedirectToAction(nameof(Index));
            }

            ViewData["PageTitle"] = "Editar Empleado";
            ViewData["PagePretitle"] = "EMPLEADOS";

            await LoadSelectListsAsync();
            return View(empleado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar empleado para edición {EmpleadoId}", id);
            SetErrorMessage("Error al cargar el empleado");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Empleados/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmpleadoViewModel model)
    {
        if (id != model.Id)
        {
            SetErrorMessage("ID de empleado no coincide");
            return RedirectToAction(nameof(Index));
        }

        ViewData["PageTitle"] = "Editar Empleado";
        ViewData["PagePretitle"] = "EMPLEADOS";

        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return View(model);
        }

        try
        {
            var (success, message) = await _empleadoService.UpdateAsync(model);

            if (success)
            {
                SetSuccessMessage(message);
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            SetErrorMessage(message);
            await LoadSelectListsAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar empleado {EmpleadoId}", id);
            SetErrorMessage("Error inesperado al actualizar el empleado");
            await LoadSelectListsAsync();
            return View(model);
        }
    }

    // GET: Empleados/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            if (empleado == null)
            {
                SetErrorMessage("Empleado no encontrado");
                return RedirectToAction(nameof(Index));
            }

            ViewData["PageTitle"] = "Eliminar Empleado";
            ViewData["PagePretitle"] = "EMPLEADOS";

            return View(empleado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar empleado para eliminación {EmpleadoId}", id);
            SetErrorMessage("Error al cargar el empleado");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Empleados/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var (success, message) = await _empleadoService.DeleteAsync(id);

            if (success)
            {
                SetSuccessMessage(message);
            }
            else
            {
                SetErrorMessage(message);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar empleado {EmpleadoId}", id);
            SetErrorMessage("Error inesperado al eliminar el empleado");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Empleados/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var (success, message) = await _empleadoService.ToggleStatusAsync(id);

            if (success)
            {
                SetSuccessMessage(message);
            }
            else
            {
                SetErrorMessage(message);
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del empleado {EmpleadoId}", id);
            SetErrorMessage("Error al cambiar el estado del empleado");
            return RedirectToAction(nameof(Index));
        }
    }

    // API: Verificar si Legajo existe
    [HttpGet]
    public async Task<IActionResult> CheckLegajoExists(string legajo, int? id)
    {
        try
        {
            var exists = await _empleadoService.LegajoExistsAsync(legajo, id);
            return Json(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar Legajo {Legajo}", legajo);
            return Json(new { exists = false });
        }
    }

    /// <summary>
    /// Carga las listas de selección para regímenes laborales y proyectos
    /// </summary>
    private async Task LoadSelectListsAsync()
    {
        try
        {
            // Cargar regímenes laborales
            var regimenes = await _regimenService.GetAllAsync();
            ViewBag.Regimenes = new SelectList(regimenes, "Id", "Nombre");

            // Cargar proyectos activos
            var proyectos = await _proyectoService.GetAllAsync();
            var proyectosActivos = proyectos.Where(p => p.Activo).ToList();
            ViewBag.Proyectos = new SelectList(proyectosActivos, "Id", "Nombre");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar listas de selección");
            ViewBag.Regimenes = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Proyectos = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }
}
