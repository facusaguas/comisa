using GestorNomina.Web.Models.Entities;

namespace GestorNomina.Web.Services;

public interface ICategoriaService
{
    Task<List<Categoria>> GetAllAsync();
    Task<List<Categoria>> GetByConvenioAsync(int convenioId);
    Task<Categoria?> GetByIdAsync(int id);
    Task<(int categorias, int empleados)> ActualizarSalariosMasivoAsync(
        int? convenioId, 
        decimal porcentajeAumento, 
        bool actualizarEmpleados);
}
