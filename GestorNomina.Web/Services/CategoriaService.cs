using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;

namespace GestorNomina.Web.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(
        ApplicationDbContext context,
        ILogger<CategoriaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Categoria>> GetAllAsync()
    {
        return await _context.Categorias
            .Include(c => c.Convenio)
            .OrderBy(c => c.Convenio!.Nombre)
            .ThenBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<List<Categoria>> GetByConvenioAsync(int convenioId)
    {
        return await _context.Categorias
            .Include(c => c.Convenio)
            .Where(c => c.ConvenioId == convenioId)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        return await _context.Categorias
            .Include(c => c.Convenio)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(int categorias, int empleados)> ActualizarSalariosMasivoAsync(
        int? convenioId, 
        decimal porcentajeAumento, 
        bool actualizarEmpleados)
    {
        var categoriasActualizadas = 0;
        var empleadosActualizados = 0;

        try
        {
            // Obtener categorías a actualizar
            var queryCategoria = _context.Categorias.AsQueryable();
            
            if (convenioId.HasValue)
            {
                queryCategoria = queryCategoria.Where(c => c.ConvenioId == convenioId.Value);
            }

            var categorias = await queryCategoria.ToListAsync();

            // Actualizar salarios básicos de categorías
            foreach (var categoria in categorias)
            {
                if (categoria.SalarioBasico.HasValue && categoria.SalarioBasico.Value > 0)
                {
                    var incremento = categoria.SalarioBasico.Value * (porcentajeAumento / 100);
                    categoria.SalarioBasico = categoria.SalarioBasico.Value + incremento;
                    categoriasActualizadas++;
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Actualización masiva de salarios: {Categorias} categorías actualizadas con {Porcentaje}% de aumento",
                categoriasActualizadas, porcentajeAumento);

            // Si se debe actualizar empleados
            if (actualizarEmpleados)
            {
                // Obtener IDs de tipos de liquidación "Directo con acuerdo" e "Indirecto"
                var tiposLiquidacionCodigos = new[] { "DIRECTOS_CONVERSION_ACUERDO", "INDIRECTO" };
                
                var tiposLiquidacion = await _context.TiposLiquidacion
                    .Where(t => tiposLiquidacionCodigos.Contains(t.Codigo))
                    .Select(t => t.Id)
                    .ToListAsync();

                if (!tiposLiquidacion.Any())
                {
                    _logger.LogWarning("No se encontraron tipos de liquidación con códigos: {Codigos}", 
                        string.Join(", ", tiposLiquidacionCodigos));
                }

                // Obtener empleados a actualizar
                var queryEmpleados = _context.Empleados
                    .Where(e => tiposLiquidacion.Contains(e.TipoLiquidacionId!.Value))
                    .Where(e => e.ValorHoraBasico.HasValue || e.ValorConvenidoDiario.HasValue);

                // Si hay filtro por convenio, aplicar también a empleados
                if (convenioId.HasValue)
                {
                    queryEmpleados = queryEmpleados.Where(e => e.ConvenioId == convenioId.Value);
                }

                var empleados = await queryEmpleados.ToListAsync();

                // Actualizar valores de empleados
                foreach (var empleado in empleados)
                {
                    bool actualizado = false;

                    if (empleado.ValorHoraBasico.HasValue && empleado.ValorHoraBasico.Value > 0)
                    {
                        var incremento = empleado.ValorHoraBasico.Value * (porcentajeAumento / 100);
                        empleado.ValorHoraBasico = empleado.ValorHoraBasico.Value + incremento;
                        actualizado = true;
                    }

                    if (empleado.ValorConvenidoDiario.HasValue && empleado.ValorConvenidoDiario.Value > 0)
                    {
                        var incremento = empleado.ValorConvenidoDiario.Value * (porcentajeAumento / 100);
                        empleado.ValorConvenidoDiario = empleado.ValorConvenidoDiario.Value + incremento;
                        actualizado = true;
                    }

                    if (actualizado)
                    {
                        empleado.UpdatedAt = DateTime.UtcNow;
                        empleadosActualizados++;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Actualización masiva de empleados: {Empleados} empleados actualizados con {Porcentaje}% de aumento",
                    empleadosActualizados, porcentajeAumento);
            }

            return (categoriasActualizadas, empleadosActualizados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en actualización masiva de salarios");
            throw;
        }
    }
}
