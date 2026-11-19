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

            // Cargar provincias
            var provincias = await _context.Provincias
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            ViewBag.Provincias = new SelectList(provincias, "Id", "Nombre");

            // Cargar ciudades (todas, filtraremos por provincia en el cliente)
            var ciudades = await _context.Ciudades
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            ViewBag.Ciudades = new SelectList(ciudades, "Id", "Nombre");
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
            ViewBag.Provincias = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Ciudades = new SelectList(Enumerable.Empty<SelectListItem>());
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

    // API: Obtener ciudades por provincia
    [HttpGet]
    public async Task<IActionResult> GetCiudadesByProvincia(int provinciaId)
    {
        try
        {
            var ciudades = await _context.Ciudades
                .Where(c => c.ProvinciaId == provinciaId)
                .OrderBy(c => c.Nombre)
                .Select(c => new { 
                    id = c.Id, 
                    nombre = c.Nombre,
                    codigoPostal = c.CodigoPostal 
                })
                .ToListAsync();

            return Json(ciudades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ciudades por provincia {ProvinciaId}", provinciaId);
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

    // GET: Empleados/ExportarExcel
    [HttpGet]
    public async Task<IActionResult> ExportarExcel(string? nombre, string? apellido, string? cuil, int? proyectoId, bool? activo)
    {
        try
        {
            // Obtener todos los empleados y aplicar los mismos filtros que en Index
            var todosEmpleados = await _empleadoService.GetAllAsync();
            
            // Aplicar filtros
            var empleadosFiltrados = todosEmpleados.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(apellido))
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Apellido.Contains(apellido, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(cuil))
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Cuil != null && e.Cuil.Contains(cuil));
            }

            if (proyectoId.HasValue && proyectoId.Value > 0)
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.ProyectoId == proyectoId.Value);
            }

            if (activo.HasValue)
            {
                empleadosFiltrados = empleadosFiltrados.Where(e => 
                    e.Activo == activo.Value);
            }

            var empleados = empleadosFiltrados.OrderBy(e => e.Apellido).ThenBy(e => e.Nombre).ToList();

            // Crear el archivo Excel usando ClosedXML
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Empleados");

            // Encabezados - Datos Básicos
            int col = 1;
            worksheet.Cell(1, col++).Value = "ID";
            worksheet.Cell(1, col++).Value = "Legajo";
            worksheet.Cell(1, col++).Value = "Apellido";
            worksheet.Cell(1, col++).Value = "Nombre";
            worksheet.Cell(1, col++).Value = "Documento";
            worksheet.Cell(1, col++).Value = "CUIL";
            worksheet.Cell(1, col++).Value = "Domicilio";
            worksheet.Cell(1, col++).Value = "Ciudad";
            worksheet.Cell(1, col++).Value = "Código Postal";
            worksheet.Cell(1, col++).Value = "Provincia";
            worksheet.Cell(1, col++).Value = "Cargo";
            
            // Fechas
            worksheet.Cell(1, col++).Value = "Fecha Ingreso";
            worksheet.Cell(1, col++).Value = "Fecha Alta";
            worksheet.Cell(1, col++).Value = "Fecha Baja";
            worksheet.Cell(1, col++).Value = "Forma Pago";
            worksheet.Cell(1, col++).Value = "Estado";
            
            // Régimen y Proyecto
            worksheet.Cell(1, col++).Value = "Régimen Laboral";
            worksheet.Cell(1, col++).Value = "Proyecto Actual";
            
            // Datos de Liquidación
            worksheet.Cell(1, col++).Value = "Convenio";
            worksheet.Cell(1, col++).Value = "Tipo Liquidación";
            worksheet.Cell(1, col++).Value = "Categoría";
            worksheet.Cell(1, col++).Value = "Disciplina";
            worksheet.Cell(1, col++).Value = "Valor Hora Básico";
            worksheet.Cell(1, col++).Value = "Valor Convenido Diario";
            worksheet.Cell(1, col++).Value = "Plus Altura";
            worksheet.Cell(1, col++).Value = "Plus Eléctrico";
            
            // Auditoría
            worksheet.Cell(1, col++).Value = "Fecha Creación";
            worksheet.Cell(1, col++).Value = "Fecha Actualización";

            int totalColumns = col - 1;

            // Aplicar estilo a los encabezados
            var headerRange = worksheet.Range(1, 1, 1, totalColumns);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

            // Llenar datos
            int row = 2;
            foreach (var emp in empleados)
            {
                col = 1;
                
                // Datos Básicos
                worksheet.Cell(row, col++).Value = emp.Id;
                worksheet.Cell(row, col++).Value = emp.Legajo;
                worksheet.Cell(row, col++).Value = emp.Apellido;
                worksheet.Cell(row, col++).Value = emp.Nombre;
                worksheet.Cell(row, col++).Value = emp.DocumentoIdentidad ?? "";
                worksheet.Cell(row, col++).Value = emp.Cuil ?? "";
                worksheet.Cell(row, col++).Value = emp.Domicilio ?? "";
                worksheet.Cell(row, col++).Value = emp.CiudadNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.CodigoPostal ?? "";
                worksheet.Cell(row, col++).Value = emp.ProvinciaNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.Cargo ?? "";
                
                // Fechas
                worksheet.Cell(row, col++).Value = emp.FechaIngreso?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cell(row, col++).Value = emp.FechaAlta?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cell(row, col++).Value = emp.FechaBaja?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cell(row, col++).Value = emp.FormaPago == Models.Enums.FormaPago.Quincenal ? "Quincenal" : "Mensual";
                
                // Estado
                string estado = emp.Activo == true ? "Activo" : (emp.Activo == false ? "Inactivo" : "Pendiente");
                worksheet.Cell(row, col++).Value = estado;
                
                // Régimen y Proyecto
                worksheet.Cell(row, col++).Value = emp.RegimenNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.ProyectoNombre ?? "";
                
                // Datos de Liquidación
                worksheet.Cell(row, col++).Value = emp.ConvenioNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.TipoLiquidacionNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.CategoriaNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.DisciplinaNombre ?? "";
                
                // Valores monetarios
                if (emp.ValorHoraBasico.HasValue)
                {
                    worksheet.Cell(row, col).Value = emp.ValorHoraBasico.Value;
                    worksheet.Cell(row, col).Style.NumberFormat.Format = "$#,##0.00";
                }
                col++;
                
                if (emp.ValorConvenidoDiario.HasValue)
                {
                    worksheet.Cell(row, col).Value = emp.ValorConvenidoDiario.Value;
                    worksheet.Cell(row, col).Style.NumberFormat.Format = "$#,##0.00";
                }
                col++;
                
                // Plus
                worksheet.Cell(row, col++).Value = emp.AplicaPlusAltura ? "Sí" : "No";
                worksheet.Cell(row, col++).Value = emp.AplicaPlusElectrico ? "Sí" : "No";
                
                // Auditoría
                worksheet.Cell(row, col++).Value = emp.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, col++).Value = emp.UpdatedAt.ToString("dd/MM/yyyy HH:mm");
                
                row++;
            }

            // Ajustar ancho de columnas
            worksheet.Columns().AdjustToContents();

            // Guardar en un stream
            using var stream = new System.IO.MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Generar nombre del archivo
            var fileName = $"Empleados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar empleados a Excel");
            SetErrorMessage("Error al exportar los datos a Excel");
            return RedirectToAction(nameof(Index));
        }
    }
}
