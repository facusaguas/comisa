using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Servicio para resolver IDs de catálogos desde memoria para optimizar el rendimiento
/// </summary>
public interface ICatalogResolver
{
    /// <summary>
    /// Carga todos los catálogos en memoria
    /// </summary>
    Task LoadCatalogsAsync();

    /// <summary>
    /// Obtiene el ID del tipo de novedad por código enum
    /// </summary>
    int GetTipoNovedadId(CodigoTipoNovedad codigo);

    /// <summary>
    /// Obtiene el ID del concepto de liquidación por código enum
    /// </summary>
    int GetConceptoId(CodigoConceptoTango codigo);

    /// <summary>
    /// Obtiene el ID del tipo de liquidación por código enum
    /// </summary>
    int GetTipoLiquidacionId(CodigoTipoLiquidacion codigo);

    /// <summary>
    /// Obtiene el ID del proyecto por código
    /// </summary>
    Task<int?> GetProyectoIdByCodigo(string codigo);

    /// <summary>
    /// Obtiene el ID de la actividad por código
    /// </summary>
    Task<int?> GetActividadIdByCodigo(string codigo);

    /// <summary>
    /// Obtiene el ID de la categoría por nombre
    /// </summary>
    int GetCategoriaIdByNombre(string nombre);

    /// <summary>
    /// Obtiene el ID del convenio por código
    /// </summary>
    int GetConvenioIdByCodigo(string codigo);

    /// <summary>
    /// Obtiene el ID de la disciplina por código
    /// </summary>
    int GetDisciplinaIdByCodigo(string codigo);

    /// <summary>
    /// Verifica si los catálogos están cargados
    /// </summary>
    bool IsLoaded { get; }
}
