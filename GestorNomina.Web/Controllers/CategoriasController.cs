using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestorNomina.Web.Filters;
using GestorNomina.Web.Services;

namespace GestorNomina.Web.Controllers;

public class CategoriasController : BaseController
{
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(
        ICategoriaService categoriaService,
        ILogger<CategoriasController> logger)
    {
        _categoriaService = categoriaService;
        _logger = logger;
    }

    // GET: Categorias
    public async Task<IActionResult> Index()
    {
        try
        {
            var categorias = await _categoriaService.GetAllAsync();
            
            // Cargar convenios para el modal de actualización masiva
            var convenios = categorias
                .Where(c => c.Convenio != null)
                .Select(c => c.Convenio!)
                .DistinctBy(c => c.Id)
                .OrderBy(c => c.Nombre)
                .ToList();

            ViewBag.Convenios = new SelectList(convenios, "Id", "Nombre");

            return View(categorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las categorías");
            TempData["Error"] = "Error al cargar las categorías";
            return View(new List<Models.Entities.Categoria>());
        }
    }

    // POST: Categorias/ActualizarSalariosMasivo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarSalariosMasivo(
        int? convenioId, 
        decimal porcentajeAumento, 
        bool actualizarEmpleados = false)
    {
        try
        {
            if (porcentajeAumento <= 0)
            {
                TempData["Error"] = "El porcentaje de aumento debe ser mayor a 0";
                return RedirectToAction(nameof(Index));
            }

            if (porcentajeAumento > 100)
            {
                TempData["Warning"] = $"El porcentaje de aumento es muy alto ({porcentajeAumento}%). Confirme que es correcto.";
            }

            var (categoriasActualizadas, empleadosActualizados) = 
                await _categoriaService.ActualizarSalariosMasivoAsync(
                    convenioId, 
                    porcentajeAumento, 
                    actualizarEmpleados);

            var mensaje = $"Se actualizaron {categoriasActualizadas} categoría(s) con un aumento del {porcentajeAumento}%";
            
            if (actualizarEmpleados)
            {
                mensaje += $" y {empleadosActualizados} empleado(s)";
            }

            TempData["Success"] = mensaje;
            
            _logger.LogInformation(
                "Actualización masiva completada: {Categorias} categorías, {Empleados} empleados, {Porcentaje}% aumento",
                categoriasActualizadas, empleadosActualizados, porcentajeAumento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en actualización masiva de salarios");
            TempData["Error"] = "Error al actualizar los salarios: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
