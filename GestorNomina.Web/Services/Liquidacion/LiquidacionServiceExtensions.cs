using GestorNomina.Web.Services.Liquidacion.Payroll;
using Microsoft.Extensions.DependencyInjection;

namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Extensiones para configurar los servicios de liquidación
/// </summary>
public static class LiquidacionServiceExtensions
{
    /// <summary>
    /// Registra todos los servicios del módulo de liquidación
    /// </summary>
    public static IServiceCollection AddLiquidacionServices(this IServiceCollection services)
    {
        // Registrar el resolver de catálogos como Scoped (no puede ser Singleton porque usa DbContext)
        services.AddScoped<ICatalogResolver, CatalogResolver>();

        // Registrar servicios de liquidación como Scoped
        services.AddScoped<IExcelParteDiarioService, ExcelParteDiarioService>();
        services.AddScoped<IHorasAprobacionService, HorasAprobacionService>();
        services.AddScoped<IPayrollEngine, PayrollEngine>();
        services.AddScoped<ITangoExportService, TangoExportService>();

        return services;
    }
}
