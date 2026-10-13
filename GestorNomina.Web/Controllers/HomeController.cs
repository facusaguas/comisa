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

    public async Task<IActionResult> Index()
    {
        // Estadísticas para el dashboard
        ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
        ViewBag.UsuariosActivos = await _context.Usuarios.CountAsync(u => u.Activo);
        ViewBag.TotalEmpleados = await _context.Empleados.CountAsync();
        ViewBag.EmpleadosActivos = await _context.Empleados.CountAsync(e => e.Activo);
        ViewBag.TotalProyectos = await _context.Proyectos.CountAsync();
        ViewBag.ProyectosActivos = await _context.Proyectos.CountAsync(p => p.Activo);
        ViewBag.TotalRegimenes = await _context.RegimenesLaborales.CountAsync();
        ViewBag.TotalActividades = await _context.Actividades.CountAsync();

        // Actividad reciente - últimos usuarios creados
        ViewBag.UltimosUsuarios = await _context.Usuarios
            .Include(u => u.Rol)
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .ToListAsync();

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
