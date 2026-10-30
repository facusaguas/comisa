using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestorNomina.Web.Filters;
using GestorNomina.Web.Services;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Controllers;

public class NovedadesController : BaseController
{
    private readonly INovedadService _novedadService;
    private readonly IEmpleadoService _empleadoService;
    private readonly ILogger<NovedadesController> _logger;

    public NovedadesController(
        INovedadService novedadService,
        IEmpleadoService empleadoService,
        ILogger<NovedadesController> logger)
    {
        _novedadService = novedadService;
        _empleadoService = empleadoService;
        _logger = logger;
    }

    // GET: Novedades
    public async Task<IActionResult> Index(int? empleadoId, DateTime? periodo, int? quincena)
    {
        try
        {
            List<Models.Entities.Novedad> novedades;

            if (empleadoId.HasValue)
            {
                novedades = await _novedadService.GetByEmpleadoAsync(empleadoId.Value);
                var empleado = await _empleadoService.GetByIdAsync(empleadoId.Value);
                ViewBag.FiltroEmpleado = empleado != null ? $"{empleado.Apellido}, {empleado.Nombre} ({empleado.Legajo})" : null;
            }
            else if (periodo.HasValue && quincena.HasValue)
            {
                novedades = await _novedadService.GetByPeriodoAsync(periodo.Value, quincena.Value);
                ViewBag.FiltroPeriodo = $"{periodo.Value:yyyy-MM} - Quincena {quincena.Value}";
            }
            else
            {
                novedades = await _novedadService.GetAllAsync();
            }

            return View(novedades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las novedades");
            TempData["Error"] = "Error al cargar las novedades";
            return View(new List<Models.Entities.Novedad>());
        }
    }

    // GET: Novedades/Create
    [AdminOnly]
    public async Task<IActionResult> Create()
    {
        await CargarEmpleadosDropdown();
        return View(new NovedadViewModel { FechaNovedad = DateTime.Now, Periodo = DateTime.Now });
    }

    // POST: Novedades/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnly]
    public async Task<IActionResult> Create(NovedadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await CargarEmpleadosDropdown();
            return View(model);
        }

        try
        {
            await _novedadService.CreateAsync(model);
            TempData["Success"] = "Novedad creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarEmpleadosDropdown();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear novedad");
            ModelState.AddModelError(string.Empty, "Error al crear la novedad");
            await CargarEmpleadosDropdown();
            return View(model);
        }
    }

    // GET: Novedades/Edit/5
    [AdminOnly]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var novedad = await _novedadService.GetByIdAsync(id);
            if (novedad == null)
            {
                TempData["Error"] = "Novedad no encontrada";
                return RedirectToAction(nameof(Index));
            }

            var model = new NovedadViewModel
            {
                Id = novedad.Id,
                EmpleadoId = novedad.EmpleadoId,
                Periodo = novedad.Periodo,
                Quincena = novedad.Quincena,
                TipoNovedad = novedad.TipoNovedad,
                FechaNovedad = novedad.FechaNovedad,
                MontoNovedad = novedad.MontoNovedad,
                Observaciones = novedad.Observaciones,
                EmpleadoNombre = $"{novedad.Empleado.Apellido}, {novedad.Empleado.Nombre}",
                EmpleadoLegajo = novedad.Empleado.Legajo
            };

            await CargarEmpleadosDropdown();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar novedad para edición");
            TempData["Error"] = "Error al cargar la novedad";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Novedades/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnly]
    public async Task<IActionResult> Edit(int id, NovedadViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await CargarEmpleadosDropdown();
            return View(model);
        }

        try
        {
            await _novedadService.UpdateAsync(id, model);
            TempData["Success"] = "Novedad actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Novedad no encontrada";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar novedad");
            ModelState.AddModelError(string.Empty, "Error al actualizar la novedad");
            await CargarEmpleadosDropdown();
            return View(model);
        }
    }

    // POST: Novedades/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AdminOnly]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _novedadService.DeleteAsync(id);
            if (result)
            {
                TempData["Success"] = "Novedad eliminada exitosamente";
            }
            else
            {
                TempData["Error"] = "Novedad no encontrada";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar novedad");
            TempData["Error"] = "Error al eliminar la novedad";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarEmpleadosDropdown()
    {
        var empleados = await _empleadoService.GetActivosAsync();
        ViewBag.Empleados = new SelectList(
            empleados.OrderBy(e => e.Apellido).ThenBy(e => e.Nombre),
            "Id",
            "NombreCompleto"
        );
    }
}
