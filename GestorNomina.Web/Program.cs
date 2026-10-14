using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Data;
using GestorNomina.Web.Services;
using GestorNomina.Web.Services.Liquidacion;
using GestorNomina.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// Configuración de Servicios
// ============================================

// Configuración de Controllers con Views
builder.Services.AddControllersWithViews();

// Configuración de PostgreSQL con Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    
    // Configuración adicional para desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configuración de sesiones para autenticación
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Registrar servicios de la aplicación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IRegimenLaboralService, RegimenLaboralService>();
builder.Services.AddScoped<IActividadService, ActividadService>();
builder.Services.AddScoped<IFeriadoService, FeriadoService>();

// Registrar servicios del módulo de liquidación
builder.Services.AddLiquidacionServices();

// Configuración de Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// En producción, configurar logging a archivo o servicio externo
if (builder.Environment.IsProduction())
{
    // Aquí se puede agregar Serilog, NLog, etc.
}

var app = builder.Build();

// ============================================
// Configuración del Pipeline de Middleware
// ============================================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Sesiones (debe ir antes de UseAuthorization y antes de cualquier middleware que use sesiones)
app.UseSession();

// Middleware personalizado para logging de requests (debe ir después de UseSession)
app.UseRequestLogging();

app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ruta de fallback para páginas no encontradas
app.MapFallbackToController("Error", "Home");

// ============================================
// Inicialización de Base de Datos
// ============================================

// Aplicar migraciones automáticamente en desarrollo (opcional)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Verificar si la base de datos existe y está actualizada
            if (context.Database.GetPendingMigrations().Any())
            {
                app.Logger.LogInformation("Aplicando migraciones pendientes...");
                context.Database.Migrate();
                app.Logger.LogInformation("Migraciones aplicadas exitosamente");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error al aplicar migraciones en el inicio de la aplicación");
        }
    }
}

app.Logger.LogInformation("Aplicación iniciada correctamente");
app.Logger.LogInformation("Entorno: {Environment}", app.Environment.EnvironmentName);

app.Run();
