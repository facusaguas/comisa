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
    private readonly IExcelExportService _excelExportService;
    private readonly ILogger<ProyectosController> _logger;

    public ProyectosController(
        IProyectoService proyectoService,
        IRegimenLaboralService regimenService,
        IExcelExportService excelExportService,
        ILogger<ProyectosController> logger)
    {
        _proyectoService = proyectoService;
        _regimenService = regimenService;
        _excelExportService = excelExportService;
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
    /// Obtiene estadísticas del proyecto (cantidad de empleados por tipo de contratación)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEstadisticasProyecto(int id)
    {
        try
        {
            var estadisticas = await _proyectoService.GetEstadisticasProyectoAsync(id);
            return Json(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del proyecto {ProyectoId}", id);
            return Json(new { success = false, message = "Error al obtener estadísticas" });
        }
    }

    /// <summary>
    /// Genera y descarga un archivo Excel con los empleados del proyecto para una quincena específica
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> DescargarExcelEmpleados(int proyectoId, int mes, int anio, int quincena)
    {
        try
        {
            var bytes = await _excelExportService.GenerarExcelEmpleadosProyectoAsync(proyectoId, mes, anio, quincena);
            
            var proyecto = await _proyectoService.GetByIdAsync(proyectoId);
            var nombreMes = new System.Globalization.CultureInfo("es-ES")
                .DateTimeFormat.GetMonthName(mes);
            var nombreArchivo = $"Empleados_{proyecto?.Codigo ?? "Proyecto"}_{nombreMes}_{anio}_Q{quincena}.xlsx";

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar Excel de empleados para proyecto {ProyectoId}", proyectoId);
            SetErrorMessage("Error al generar el archivo Excel");
            return RedirectToAction(nameof(Details), new { id = proyectoId });
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
