using GestorNomina.Web.Data;
using GestorNomina.Web.Filters;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Services.Liquidacion;
using GestorNomina.Web.Services.Liquidacion.Payroll;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Controllers;

[Authorize]
public class LiquidacionController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IExcelParteDiarioService _excelService;
    private readonly IHorasAprobacionService _aprobacionService;
    private readonly IPayrollEngine _payrollEngine;
    private readonly ITangoExportService _tangoExportService;

    public LiquidacionController(
        ApplicationDbContext context,
        IExcelParteDiarioService excelService,
        IHorasAprobacionService aprobacionService,
        IPayrollEngine payrollEngine,
        ITangoExportService tangoExportService)
    {
        _context = context;
        _excelService = excelService;
        _aprobacionService = aprobacionService;
        _payrollEngine = payrollEngine;
        _tangoExportService = tangoExportService;
    }

    // ==========================================
    // PASO 1: IMPORTACIÓN
    // ==========================================

    public IActionResult ImportarHoras()
    {
        ViewBag.Proyectos = _context.Proyectos
            .OrderBy(p => p.Nombre)
            .ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportarHoras(IFormFile archivo, string periodo, int quincena, int proyectoId)
    {
        try
        {
            if (archivo == null || archivo.Length == 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un archivo Excel";
                ViewBag.Proyectos = _context.Proyectos.OrderBy(p => p.Nombre).ToList();
                return View();
            }

            if (string.IsNullOrEmpty(periodo))
            {
                TempData["ErrorMessage"] = "Debe seleccionar un periodo";
                ViewBag.Proyectos = _context.Proyectos.OrderBy(p => p.Nombre).ToList();
                return View();
            }

            if (proyectoId <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un proyecto";
                ViewBag.Proyectos = _context.Proyectos.OrderBy(p => p.Nombre).ToList();
                return View();
            }

            using var stream = archivo.OpenReadStream();
            var cantidadImportada = await _excelService.ImportarParteDiarioAsync(stream, periodo, quincena, proyectoId);

            TempData["SuccessMessage"] = $"Se importaron {cantidadImportada} registros correctamente";
            return RedirectToAction(nameof(PeriodosCarga));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al importar: {ex.Message}";
            ViewBag.Proyectos = _context.Proyectos.OrderBy(p => p.Nombre).ToList();
            return View();
        }
    }

    // ==========================================
    // PASO 2: PERIODOS DE CARGA
    // ==========================================

    public async Task<IActionResult> PeriodosCarga()
    {
        var periodos = await _context.PeriodosCarga
            .Include(p => p.Proyecto)
            .Include(p => p.UsuarioCarga)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(periodos);
    }

    // ==========================================
    // PASO 3: REGISTROS PENDIENTES Y APROBACIÓN
    // ==========================================

    public async Task<IActionResult> RegistrosPendientes(int periodoCargaId)
    {
        var periodo = await _context.PeriodosCarga
            .Include(p => p.Proyecto)
            .FirstOrDefaultAsync(p => p.Id == periodoCargaId);

        if (periodo == null)
        {
            TempData["ErrorMessage"] = "Periodo no encontrado";
            return RedirectToAction(nameof(PeriodosCarga));
        }

        var registros = await _aprobacionService.ObtenerRegistrosPendientesAsync(periodoCargaId);

        ViewBag.Periodo = periodo;
        return View(registros);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AprobarAutomaticamente(int periodoCargaId)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var cantidadAprobada = await _aprobacionService.AprobarAutomaticamenteAsync(periodoCargaId, usuarioId);

            TempData["SuccessMessage"] = $"Se aprobaron {cantidadAprobada} registros automáticamente";
            return RedirectToAction(nameof(RegistrosAprobados), new { periodoCargaId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al aprobar: {ex.Message}";
            return RedirectToAction(nameof(RegistrosPendientes), new { periodoCargaId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AprobarConCorreccion(int registroRawId, decimal horasCorregidas, string motivoModificacion)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            await _aprobacionService.AprobarRegistroAsync(registroRawId, horasCorregidas, motivoModificacion, usuarioId);

            TempData["SuccessMessage"] = "Registro aprobado con corrección";
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ==========================================
    // PASO 4: REGISTROS APROBADOS
    // ==========================================

    public async Task<IActionResult> RegistrosAprobados(int periodoCargaId)
    {
        var periodo = await _context.PeriodosCarga
            .Include(p => p.Proyecto)
            .FirstOrDefaultAsync(p => p.Id == periodoCargaId);

        if (periodo == null)
        {
            TempData["ErrorMessage"] = "Periodo no encontrado";
            return RedirectToAction(nameof(PeriodosCarga));
        }

        var registros = await _context.HorasRegistrosAprobados
            .Include(r => r.Empleado)
            .Include(r => r.Actividad)
            .Include(r => r.UsuarioModificador)
            .Where(r => r.PeriodoCargaId == periodoCargaId)
            .OrderBy(r => r.Fecha)
            .ThenBy(r => r.Empleado.Legajo)
            .ToListAsync();

        ViewBag.Periodo = periodo;
        return View(registros);
    }

    // ==========================================
    // PASO 5: PROCESAMIENTO DE LIQUIDACIÓN
    // ==========================================

    public async Task<IActionResult> ProcesarLiquidacion()
    {
        var periodosAprobados = await _context.PeriodosCarga
            .Include(p => p.Proyecto)
            .Where(p => p.Estado == "Aprobado")
            .OrderByDescending(p => p.Anio)
            .ThenByDescending(p => p.Mes)
            .ToListAsync();

        return View(periodosAprobados);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcesarLiquidacionMasiva(string periodo, int quincena)
    {
        try
        {
            var recibos = await _payrollEngine.ProcesarLiquidacionMasivaAsync(periodo, quincena);

            TempData["SuccessMessage"] = $"Se procesaron {recibos.Count} liquidaciones correctamente";
            return RedirectToAction(nameof(Recibos), new { periodo, quincena });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al procesar liquidación: {ex.Message}";
            return RedirectToAction(nameof(ProcesarLiquidacion));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcesarLiquidacionIndividual(int empleadoId, string periodo, int quincena)
    {
        try
        {
            var recibo = await _payrollEngine.ProcesarLiquidacionAsync(empleadoId, periodo, quincena);

            TempData["SuccessMessage"] = "Liquidación procesada correctamente";
            return RedirectToAction(nameof(DetalleRecibo), new { id = recibo.Id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al procesar liquidación: {ex.Message}";
            return RedirectToAction(nameof(ProcesarLiquidacion));
        }
    }

    // ==========================================
    // PASO 6: RECIBOS
    // ==========================================

    public async Task<IActionResult> Recibos(string? periodo = null, int? quincena = null)
    {
        var query = _context.Recibos
            .Include(r => r.Empleado)
            .AsQueryable();

        if (!string.IsNullOrEmpty(periodo))
            query = query.Where(r => r.Periodo == periodo);

        if (quincena.HasValue)
            query = query.Where(r => r.Quincena == quincena.Value);

        var recibos = await query
            .OrderByDescending(r => r.Periodo)
            .ThenByDescending(r => r.Quincena)
            .ThenBy(r => r.Empleado.Legajo)
            .ToListAsync();

        ViewBag.PeriodoFiltro = periodo;
        ViewBag.QuincenaFiltro = quincena;
        return View(recibos);
    }

    public async Task<IActionResult> DetalleRecibo(int id)
    {
        var recibo = await _context.Recibos
            .Include(r => r.Empleado)
            .Include(r => r.Detalles)
                .ThenInclude(d => d.Concepto)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recibo == null)
        {
            TempData["ErrorMessage"] = "Recibo no encontrado";
            return RedirectToAction(nameof(Recibos));
        }

        return View(recibo);
    }

    // ==========================================
    // PASO 7: EXPORTACIÓN A TANGO
    // ==========================================

    public IActionResult ExportarTango()
    {
        var periodosConRecibos = _context.Recibos
            .Select(r => new { r.Periodo, r.Quincena })
            .Distinct()
            .OrderByDescending(p => p.Periodo)
            .ThenByDescending(p => p.Quincena)
            .ToList();

        return View(periodosConRecibos);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarArchivoTango(string periodo, int quincena)
    {
        try
        {
            var recibos = await _context.Recibos
                .Include(r => r.Empleado)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Concepto)
                .Where(r => r.Periodo == periodo && r.Quincena == quincena)
                .ToListAsync();

            if (!recibos.Any())
            {
                TempData["ErrorMessage"] = "No hay recibos para exportar en ese periodo/quincena";
                return RedirectToAction(nameof(ExportarTango));
            }

            var contenidoArchivo = await _tangoExportService.ExportarATango(recibos);
            var nombreArchivo = $"liquidacion_{periodo.Replace("-", "_")}_q{quincena}_{DateTime.Now:yyyyMMdd}.txt";

            return File(System.Text.Encoding.UTF8.GetBytes(contenidoArchivo), "text/plain", nombreArchivo);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al generar archivo: {ex.Message}";
            return RedirectToAction(nameof(ExportarTango));
        }
    }

    // ==========================================
    // CATÁLOGOS ABM
    // ==========================================

    #region Convenios
    public async Task<IActionResult> Convenios()
    {
        var convenios = await _context.Convenios.ToListAsync();
        return View(convenios);
    }

    public IActionResult CrearConvenio()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearConvenio(Convenio convenio)
    {
        if (ModelState.IsValid)
        {
            _context.Convenios.Add(convenio);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Convenio creado correctamente";
            return RedirectToAction(nameof(Convenios));
        }
        return View(convenio);
    }

    public async Task<IActionResult> EditarConvenio(int id)
    {
        var convenio = await _context.Convenios.FindAsync(id);
        if (convenio == null)
            return NotFound();
        return View(convenio);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarConvenio(int id, Convenio convenio)
    {
        if (id != convenio.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(convenio);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Convenio actualizado correctamente";
            return RedirectToAction(nameof(Convenios));
        }
        return View(convenio);
    }
    #endregion

    #region Categorías
    public async Task<IActionResult> Categorias()
    {
        var categorias = await _context.Categorias
            .Include(c => c.Convenio)
            .ToListAsync();
        return View(categorias);
    }

    public async Task<IActionResult> CrearCategoria()
    {
        ViewBag.Convenios = await _context.Convenios.ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCategoria(Categoria categoria)
    {
        if (ModelState.IsValid)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Categoría creada correctamente";
            return RedirectToAction(nameof(Categorias));
        }
        ViewBag.Convenios = await _context.Convenios.ToListAsync();
        return View(categoria);
    }

    public async Task<IActionResult> EditarCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return NotFound();
        ViewBag.Convenios = await _context.Convenios.ToListAsync();
        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarCategoria(int id, Categoria categoria)
    {
        if (id != categoria.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(categoria);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Categoría actualizada correctamente";
            return RedirectToAction(nameof(Categorias));
        }
        ViewBag.Convenios = await _context.Convenios.ToListAsync();
        return View(categoria);
    }
    #endregion

    #region Disciplinas
    public async Task<IActionResult> Disciplinas()
    {
        var disciplinas = await _context.Disciplinas.ToListAsync();
        return View(disciplinas);
    }

    public IActionResult CrearDisciplina()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearDisciplina(Disciplina disciplina)
    {
        if (ModelState.IsValid)
        {
            _context.Disciplinas.Add(disciplina);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Disciplina creada correctamente";
            return RedirectToAction(nameof(Disciplinas));
        }
        return View(disciplina);
    }

    public async Task<IActionResult> EditarDisciplina(int id)
    {
        var disciplina = await _context.Disciplinas.FindAsync(id);
        if (disciplina == null)
            return NotFound();
        return View(disciplina);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarDisciplina(int id, Disciplina disciplina)
    {
        if (id != disciplina.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(disciplina);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Disciplina actualizada correctamente";
            return RedirectToAction(nameof(Disciplinas));
        }
        return View(disciplina);
    }
    #endregion
}
