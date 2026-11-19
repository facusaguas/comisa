using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Models;
using GestorNomina.Web.Data;
using GestorNomina.Web.Filters;

namespace GestorNomina.Web.Controllers;

[Authorize]
public class HomeController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? mes, int? anio)
    {
        try
        {
            // Usar el mes/año seleccionado o el actual
            var mesSeleccionado = mes ?? DateTime.Now.Month;
            var anioSeleccionado = anio ?? DateTime.Now.Year;
            
            ViewBag.MesSeleccionado = mesSeleccionado;
            ViewBag.AnioSeleccionado = anioSeleccionado;
            ViewBag.NombreMesSeleccionado = new DateTime(anioSeleccionado, mesSeleccionado, 1)
                .ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-AR"));

            // KPIs principales
            ViewBag.TotalEmpleados = await _context.Empleados.CountAsync();
            ViewBag.EmpleadosActivos = await _context.Empleados.CountAsync(e => e.Activo);
            ViewBag.TotalProyectos = await _context.Proyectos.CountAsync();
            ViewBag.ProyectosActivos = await _context.Proyectos.CountAsync(p => p.Activo);
            
            // Horas del mes seleccionado
            var horasMesSeleccionado = await _context.HorasRegistrosAprobados
                .Where(h => h.Fecha.Month == mesSeleccionado && h.Fecha.Year == anioSeleccionado)
                .SumAsync(h => (double?)h.HorasFinales) ?? 0;
            ViewBag.HorasMesActual = Math.Round(horasMesSeleccionado, 0);

            // Total horas acumuladas
            var totalHoras = await _context.HorasRegistrosAprobados
                .SumAsync(h => (double?)h.HorasFinales) ?? 0;
            ViewBag.TotalHorasAcumuladas = Math.Round(totalHoras, 0);

            // Períodos activos del mes seleccionado
            var periodosActivos = await _context.PeriodosCarga
                .CountAsync(p => p.Mes == mesSeleccionado && p.Anio == anioSeleccionado);
            ViewBag.PeriodosActivos = periodosActivos;

            // Evolución de horas (últimos 6 meses desde el mes seleccionado)
            // Convertir a UTC para PostgreSQL
            var fechaInicio = DateTime.SpecifyKind(new DateTime(anioSeleccionado, mesSeleccionado, 1).AddMonths(-5), DateTimeKind.Utc);
            var fechaFin = DateTime.SpecifyKind(new DateTime(anioSeleccionado, mesSeleccionado, 1).AddMonths(1).AddDays(-1), DateTimeKind.Utc);
            
            var horasPorMes = await _context.HorasRegistrosAprobados
                .Where(h => h.Fecha >= fechaInicio && h.Fecha <= fechaFin)
                .GroupBy(h => new { h.Fecha.Year, h.Fecha.Month })
                .Select(g => new
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    Total = g.Sum(h => (double)h.HorasFinales)
                })
                .OrderBy(x => x.Anio).ThenBy(x => x.Mes)
                .ToListAsync();
            
            ViewBag.HorasPorMes = horasPorMes;

            // Top 5 proyectos con más horas (todos los tiempos)
            var topProyectos = await _context.HorasRegistrosAprobados
                .Include(h => h.PeriodoCarga)
                .ThenInclude(p => p.Proyecto)
                .GroupBy(h => new 
                { 
                    ProyectoId = h.PeriodoCarga.ProyectoId,
                    Codigo = h.PeriodoCarga.Proyecto.Codigo,
                    Nombre = h.PeriodoCarga.Proyecto.Nombre
                })
                .Select(g => new
                {
                    Codigo = g.Key.Codigo,
                    Nombre = g.Key.Nombre,
                    TotalHoras = g.Sum(h => (double)h.HorasFinales)
                })
                .OrderByDescending(x => x.TotalHoras)
                .Take(5)
                .ToListAsync();
            
            ViewBag.TopProyectos = topProyectos;

            // Top 5 empleados del mes seleccionado
            var topEmpleados = await _context.HorasRegistrosAprobados
                .Include(h => h.Empleado)
                .Where(h => h.Fecha.Month == mesSeleccionado && h.Fecha.Year == anioSeleccionado)
                .GroupBy(h => new 
                { 
                    EmpleadoId = h.EmpleadoId,
                    Nombre = h.Empleado.Nombre,
                    Apellido = h.Empleado.Apellido
                })
                .Select(g => new
                {
                    NombreCompleto = g.Key.Nombre + " " + g.Key.Apellido,
                    TotalHoras = g.Sum(h => (double)h.HorasFinales)
                })
                .OrderByDescending(x => x.TotalHoras)
                .Take(5)
                .ToListAsync();
            
            ViewBag.TopEmpleados = topEmpleados;

            // Distribución de empleados por régimen
            var empleadosPorRegimen = await _context.Empleados
                .Include(e => e.Regimen)
                .Where(e => e.Activo && e.RegimenId.HasValue)
                .GroupBy(e => e.Regimen!.Nombre)
                .Select(g => new
                {
                    Regimen = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToListAsync();
            
            ViewBag.EmpleadosPorRegimen = empleadosPorRegimen;

            // Novedades recientes del mes seleccionado
            var inicioMes = DateTime.SpecifyKind(new DateTime(anioSeleccionado, mesSeleccionado, 1), DateTimeKind.Utc);
            var finMes = DateTime.SpecifyKind(inicioMes.AddMonths(1).AddDays(-1).Date.AddHours(23).AddMinutes(59).AddSeconds(59), DateTimeKind.Utc);
            
            var novedades = await _context.Novedades
                .Include(n => n.Empleado)
                .Where(n => n.FechaNovedad >= inicioMes && n.FechaNovedad <= finMes)
                .OrderByDescending(n => n.FechaNovedad)
                .Take(5)
                .ToListAsync();
            
            ViewBag.NovedadesRecientes = novedades;

            // Estadísticas adicionales
            ViewBag.TotalActividades = await _context.Actividades.CountAsync();
            ViewBag.TotalRegimenes = await _context.RegimenesLaborales.CountAsync();
            ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();

            // Info de debug
            ViewBag.TotalHorasRegistradas = await _context.HorasRegistrosAprobados.CountAsync();
            ViewBag.DebugInfo = $"Mes: {mesSeleccionado}/{anioSeleccionado}, Total registros: {ViewBag.TotalHorasRegistradas}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el dashboard");
            // Inicializar valores por defecto en caso de error
            ViewBag.EmpleadosActivos = 0;
            ViewBag.ProyectosActivos = 0;
            ViewBag.HorasMesActual = 0;
            ViewBag.PeriodosActivos = 0;
            ViewBag.ErrorMessage = ex.Message + " | " + ex.StackTrace;
            ViewBag.MesSeleccionado = mes ?? DateTime.Now.Month;
            ViewBag.AnioSeleccionado = anio ?? DateTime.Now.Year;
            ViewBag.NombreMesSeleccionado = new DateTime(ViewBag.AnioSeleccionado, ViewBag.MesSeleccionado, 1)
                .ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-AR"));
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
