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
    private readonly IProyectoService _proyectoService;
    private readonly ILogger<NovedadesController> _logger;

    public NovedadesController(
        INovedadService novedadService,
        IEmpleadoService empleadoService,
        IProyectoService proyectoService,
        ILogger<NovedadesController> logger)
    {
        _novedadService = novedadService;
        _empleadoService = empleadoService;
        _proyectoService = proyectoService;
        _logger = logger;
    }

    // GET: Novedades
    public async Task<IActionResult> Index(
        string? legajo, 
        string? cuil, 
        int? proyectoId, 
        DateTime? periodo, 
        int? quincena,
        TipoNovedadLiquidacion? tipoNovedad)
    {
        try
        {
            List<Models.Entities.Novedad> novedades;

            // Si hay algún filtro aplicado, usar el método de filtrado
            if (!string.IsNullOrWhiteSpace(legajo) || 
                !string.IsNullOrWhiteSpace(cuil) || 
                proyectoId.HasValue || 
                periodo.HasValue || 
                quincena.HasValue ||
                tipoNovedad.HasValue)
            {
                novedades = await _novedadService.GetFilteredAsync(
                    legajo, 
                    cuil, 
                    proyectoId, 
                    periodo, 
                    quincena,
                    tipoNovedad);

                // Preparar información de filtros aplicados
                var filtrosAplicados = new List<string>();
                
                if (!string.IsNullOrWhiteSpace(legajo))
                    filtrosAplicados.Add($"Legajo: {legajo}");
                
                if (!string.IsNullOrWhiteSpace(cuil))
                    filtrosAplicados.Add($"CUIL: {cuil}");
                
                if (proyectoId.HasValue)
                {
                    var proyecto = await _proyectoService.GetByIdAsync(proyectoId.Value);
                    if (proyecto != null)
                        filtrosAplicados.Add($"Proyecto: {proyecto.Nombre}");
                }
                
                if (periodo.HasValue)
                    filtrosAplicados.Add($"Periodo: {periodo.Value:yyyy-MM}");
                
                if (quincena.HasValue)
                    filtrosAplicados.Add($"Quincena: {quincena.Value}");

                if (tipoNovedad.HasValue)
                    filtrosAplicados.Add($"Tipo: {GetTipoNovedadNombre(tipoNovedad.Value)}");
                
                ViewBag.FiltrosAplicados = filtrosAplicados;
            }
            else
            {
                novedades = await _novedadService.GetAllAsync();
            }

            // Cargar dropdowns para filtros
            await CargarFiltrosDropdown();

            // Mantener valores de filtros en ViewBag para el formulario
            ViewBag.FiltroLegajo = legajo;
            ViewBag.FiltroCuil = cuil;
            ViewBag.FiltroProyectoId = proyectoId;
            ViewBag.FiltroPeriodo = periodo?.ToString("yyyy-MM");
            ViewBag.FiltroQuincena = quincena;
            ViewBag.FiltroTipoNovedad = tipoNovedad;

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

    private async Task CargarFiltrosDropdown()
    {
        // Cargar proyectos para el dropdown de filtros
        var proyectos = await _proyectoService.GetAllAsync();
        ViewBag.Proyectos = new SelectList(
            proyectos.OrderBy(p => p.Nombre),
            "Id",
            "Nombre"
        );

        // Cargar tipos de novedad
        ViewBag.TiposNovedad = new SelectList(
            Enum.GetValues(typeof(TipoNovedadLiquidacion))
                .Cast<TipoNovedadLiquidacion>()
                .Select(t => new { Value = (int)t, Text = GetTipoNovedadNombre(t) }),
            "Value",
            "Text"
        );

        // Quincenas
        ViewBag.Quincenas = new SelectList(new[] { 1, 2 });
    }

    private string GetTipoNovedadNombre(TipoNovedadLiquidacion tipo)
    {
        return tipo switch
        {
            TipoNovedadLiquidacion.Embargo => "Embargo",
            TipoNovedadLiquidacion.InicioRoster => "Inicio Roster",
            TipoNovedadLiquidacion.FinRoster => "Fin Roster",
            TipoNovedadLiquidacion.Enfermedad => "Enfermedad",
            TipoNovedadLiquidacion.Baja => "Baja",
            TipoNovedadLiquidacion.Vacaciones => "Vacaciones",
            TipoNovedadLiquidacion.Anticipo => "Anticipo",
            _ => tipo.ToString()
        };
    }
}
