using GestorNomina.Web.Data;
using GestorNomina.Web.Filters;
using GestorNomina.Web.Models.ViewModels;
using GestorNomina.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Controllers;

[Authorize]
[AdminOnlyAttribute]
public class EmpleadosController : BaseController
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IRegimenLaboralService _regimenService;
    private readonly IProyectoService _proyectoService;
    private readonly IPdfEmpleadoService _pdfEmpleadoService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmpleadosController> _logger;

    public EmpleadosController(
        IEmpleadoService empleadoService,
        IRegimenLaboralService regimenService,
        IProyectoService proyectoService,
        IPdfEmpleadoService pdfEmpleadoService,
        ApplicationDbContext context,
        ILogger<EmpleadosController> logger)
    {
        _empleadoService = empleadoService;
        _regimenService = regimenService;
        _proyectoService = proyectoService;
        _pdfEmpleadoService = pdfEmpleadoService;
        _context = context;
        _logger = logger;
    }

    // GET: Empleados
    public async Task<IActionResult> Index(string? nombre, string? apellido, string? cuil, int? proyectoId, bool? activo, int page = 1)
    {
        try
        {
            ViewData["PageTitle"] = "Empleados";
            ViewData["PagePretitle"] = "CONFIGURACIÓN";

            const int pageSize = 10;

            // Obtener todos los empleados y aplicar filtros
            var todosEmpleados = await _empleadoService.GetAllAsync();
            
            // Aplicar filtros
            var empleadosFiltrados = todosEmpleados.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
                ViewBag.FiltroNombre = nombre;
            }

            if (!string.IsNullOrWhiteSpace(apellido))
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Apellido.Contains(apellido, StringComparison.OrdinalIgnoreCase));
                ViewBag.FiltroApellido = apellido;
            }

            if (!string.IsNullOrWhiteSpace(cuil))
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Cuil != null && e.Cuil.Contains(cuil));
                ViewBag.FiltroCuil = cuil;
            }

            if (proyectoId.HasValue && proyectoId.Value > 0)
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.ProyectoId == proyectoId.Value);
                ViewBag.FiltroProyectoId = proyectoId.Value;
            }

            if (activo.HasValue)
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Activo == activo.Value);
                ViewBag.FiltroActivo = activo.Value;
            }

            var listaFiltrada = empleadosFiltrados.ToList();

            // Calcular paginación
            var totalEmpleados = listaFiltrada.Count;
            var totalPages = (int)Math.Ceiling(totalEmpleados / (double)pageSize);
            
            // Validar página
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var empleadosPaginados = listaFiltrada
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pasar datos de paginación a la vista
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalEmpleados = totalEmpleados;
            ViewBag.PageSize = pageSize;

            // Cargar proyectos para el filtro
            var proyectos = await _proyectoService.GetAllAsync();
            ViewBag.Proyectos = proyectos.Where(p => p.Activo).ToList();

            return View(empleadosPaginados);
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

            // Cargar convenios
            var convenios = await _context.Convenios
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            ViewBag.Convenios = new SelectList(convenios, "Id", "Nombre");

            // Cargar tipos de liquidación
            var tiposLiquidacion = await _context.TiposLiquidacion
                .OrderBy(t => t.Nombre)
                .ToListAsync();
            ViewBag.TiposLiquidacion = new SelectList(tiposLiquidacion, "Id", "Nombre");

            // Cargar categorías (todas, filtraremos por convenio en el cliente)
            var categorias = await _context.Categorias
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");

            // Cargar disciplinas
            var disciplinas = await _context.Disciplinas
                .OrderBy(d => d.Nombre)
                .ToListAsync();
            ViewBag.Disciplinas = new SelectList(disciplinas, "Id", "Nombre");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar listas de selección");
            ViewBag.Regimenes = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Proyectos = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Convenios = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.TiposLiquidacion = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Categorias = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Disciplinas = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }

    // API: Obtener categorías por convenio
    [HttpGet]
    public async Task<IActionResult> GetCategoriasByConvenio(int convenioId)
    {
        try
        {
            var categorias = await _context.Categorias
                .Where(c => c.ConvenioId == convenioId)
                .OrderBy(c => c.Nombre)
                .Select(c => new { id = c.Id, nombre = c.Nombre })
                .ToListAsync();

            return Json(categorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías por convenio {ConvenioId}", convenioId);
            return Json(new List<object>());
        }
    }

    // GET: Empleados/CargarDesdePDF
    public IActionResult CargarDesdePDF()
    {
        ViewData["PageTitle"] = "Cargar Empleados desde PDF";
        ViewData["PagePretitle"] = "EMPLEADOS";
        return View(new CargarEmpleadosPdfViewModel());
    }

    // POST: Empleados/ProcesarPDFs
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcesarPDFs(CargarEmpleadosPdfViewModel model)
    {
        ViewData["PageTitle"] = "Cargar Empleados desde PDF";
        ViewData["PagePretitle"] = "EMPLEADOS";

        if (model.Archivos == null || model.Archivos.Count == 0)
        {
            ModelState.AddModelError("Archivos", "Debe seleccionar al menos un archivo PDF");
            return View("CargarDesdeP DF", model);
        }

        try
        {
            // Procesar los PDFs
            var resultados = await _pdfEmpleadoService.ProcesarMultiplesPdfsAsync(model.Archivos.ToList());

            // Contar resultados
            var exitosos = resultados.Count(r => r.Exitoso);
            var fallidos = resultados.Count(r => !r.Exitoso);

            SetSuccessMessage($"Procesamiento completado: {exitosos} empleados procesados, {fallidos} errores");

            return View("ResultadosProcesamiento", resultados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar PDFs");
            SetErrorMessage("Error al procesar los archivos PDF");
            return View("CargarDesdeP DF", model);
        }
    }
}
