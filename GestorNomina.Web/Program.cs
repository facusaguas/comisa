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
builder.Services.AddScoped<IConceptoLiquidacionService, ConceptoLiquidacionService>();
builder.Services.AddScoped<INovedadService, NovedadService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IPdfEmpleadoService, PdfEmpleadoService>();

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
            
            // Crear usuarios de prueba si no existen
            var authService = services.GetRequiredService<IAuthService>();
            await SeedTestUsersAsync(context, authService, app.Logger);
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

// ============================================
// Método para crear usuarios de prueba
// ============================================
static async Task SeedTestUsersAsync(ApplicationDbContext context, IAuthService authService, ILogger logger)
{
    try
    {
        // Verificar si ya existen usuarios de prueba
        var existingTestUser = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == "admin@test.com");
            
        if (existingTestUser != null)
        {
            logger.LogInformation("Usuarios de prueba ya existen");
            return;
        }
        
        logger.LogInformation("Creando usuarios de prueba...");
        
        // Obtener IDs de roles
        var roles = await context.Roles.ToDictionaryAsync(r => r.Nombre, r => r.Id);
        
        if (!roles.Any())
        {
            logger.LogWarning("No hay roles en la base de datos. Ejecuta el script SeedData.sql primero");
            return;
        }
        
        var testUsers = new[]
        {
            new { Email = "admin@test.com", Password = "Test123!", Nombre = "Admin", Apellido = "Prueba", RolNombre = "Administrador" },
            new { Email = "supervisor@test.com", Password = "Test123!", Nombre = "Supervisor", Apellido = "Prueba", RolNombre = "Supervisor" },
            new { Email = "operador@test.com", Password = "Test123!", Nombre = "Operador", Apellido = "Prueba", RolNombre = "Operador" },
            new { Email = "consulta@test.com", Password = "Test123!", Nombre = "Usuario", Apellido = "Consulta", RolNombre = "Consulta" }
        };
        
        foreach (var testUser in testUsers)
        {
            if (!roles.ContainsKey(testUser.RolNombre))
            {
                logger.LogWarning($"Rol '{testUser.RolNombre}' no encontrado");
                continue;
            }
            
            var usuario = new GestorNomina.Web.Models.Entities.Usuario
            {
                Email = testUser.Email,
                PasswordHash = authService.HashPassword(testUser.Password),
                Nombre = testUser.Nombre,
                Apellido = testUser.Apellido,
                RolId = roles[testUser.RolNombre],
                Activo = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            context.Usuarios.Add(usuario);
            logger.LogInformation($"Usuario creado: {testUser.Email}");
        }
        
        await context.SaveChangesAsync();
        logger.LogInformation("✓ Usuarios de prueba creados exitosamente");
        logger.LogInformation("Contraseña para todos: Test123!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al crear usuarios de prueba");
    }
}
