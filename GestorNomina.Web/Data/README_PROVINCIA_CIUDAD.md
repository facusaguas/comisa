# Provincias y Ciudades - Instrucciones de Implementación

## Descripción

Se han agregado las tablas `Provincia` y `Ciudad` al sistema, junto con la actualización de la tabla `Empleado` para incluir información de domicilio y localización geográfica.

## Cambios Realizados

### Nuevas Entidades

1. **Provincia** (`provincias`)
   - `id`: Identificador único
   - `nombre`: Nombre de la provincia
   - `created_at`: Fecha de creación
   - `updated_at`: Fecha de última actualización

2. **Ciudad** (`ciudades`)
   - `id`: Identificador único
   - `nombre`: Nombre de la ciudad
   - `codigo_postal`: Código postal de la ciudad
   - `provincia_id`: Referencia a la provincia (FK)
   - `created_at`: Fecha de creación
   - `updated_at`: Fecha de última actualización

### Actualización de Empleado

Se agregaron los siguientes campos a la tabla `empleados`:

- `domicilio` (string, máximo 200 caracteres): Dirección del empleado
- `ciudad_id` (int, nullable): Referencia a la ciudad (FK)

### Relaciones

- Una **Provincia** puede tener muchas **Ciudades**
- Una **Ciudad** pertenece a una **Provincia**
- Un **Empleado** puede estar asociado a una **Ciudad**

## Aplicar la Migración

### Paso 1: Aplicar la migración a la base de datos

```powershell
# Desde la raíz del proyecto
dotnet ef database update --project GestorNomina.Web
```

### Paso 2: Poblar las tablas con datos iniciales

Ejecutar el script SQL proporcionado para cargar provincias y ciudades argentinas:

```powershell
# Si estás usando PostgreSQL:
psql -U tu_usuario -d nombre_base_datos -f GestorNomina.Web/Data/SeedProvinciasYCiudades.sql

# O desde pgAdmin:
# Abrir Query Tool y ejecutar el contenido de SeedProvinciasYCiudades.sql
```

El script incluye:
- Las 24 provincias de Argentina
- Principales ciudades de cada provincia con sus códigos postales

## Uso en el Código

### Ejemplo de consulta con ciudad y provincia

```csharp
var empleado = await _context.Empleados
    .Include(e => e.Ciudad)
        .ThenInclude(c => c.Provincia)
    .FirstOrDefaultAsync(e => e.Id == empleadoId);

if (empleado?.Ciudad != null)
{
    var ciudadNombre = empleado.Ciudad.Nombre;
    var codigoPostal = empleado.Ciudad.CodigoPostal;
    var provinciaNombre = empleado.Ciudad.Provincia.Nombre;
}
```

### Actualizar empleado con domicilio y ciudad

```csharp
var empleado = await _context.Empleados.FindAsync(empleadoId);
if (empleado != null)
{
    empleado.Domicilio = "Calle Falsa 123";
    empleado.CiudadId = 1; // ID de la ciudad correspondiente
    empleado.UpdatedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
}
```

## ViewModels Sugeridos

### Para el formulario de Empleado

```csharp
public class EmpleadoFormViewModel
{
    // ... otros campos existentes
    
    public string? Domicilio { get; set; }
    public int? CiudadId { get; set; }
    public int? ProvinciaId { get; set; } // Para el filtro en cascada
    
    // Para los dropdowns
    public List<SelectListItem> Provincias { get; set; } = new();
    public List<SelectListItem> Ciudades { get; set; } = new();
}
```

## Endpoints Sugeridos para AJAX

Para implementar un dropdown en cascada (Provincia → Ciudad):

```csharp
// En un controller apropiado (ej: EmpleadosController o nuevo LocationController)
[HttpGet]
public async Task<IActionResult> GetCiudadesByProvincia(int provinciaId)
{
    var ciudades = await _context.Ciudades
        .Where(c => c.ProvinciaId == provinciaId)
        .OrderBy(c => c.Nombre)
        .Select(c => new { 
            id = c.Id, 
            nombre = c.Nombre,
            codigoPostal = c.CodigoPostal 
        })
        .ToListAsync();
    
    return Json(ciudades);
}
```

## Agregar Más Ciudades

Para agregar más ciudades, puedes usar este template SQL:

```sql
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Nombre Ciudad', 'CP', id, NOW(), NOW() 
FROM provincias 
WHERE nombre = 'Nombre Provincia';
```

## Verificación

Para verificar que todo se cargó correctamente:

```sql
-- Ver todas las provincias con sus ciudades
SELECT 
    p.nombre as provincia,
    COUNT(c.id) as cantidad_ciudades
FROM provincias p
LEFT JOIN ciudades c ON p.id = c.provincia_id
GROUP BY p.id, p.nombre
ORDER BY p.nombre;

-- Ver detalle de ciudades por provincia
SELECT 
    p.nombre as provincia,
    c.nombre as ciudad,
    c.codigo_postal
FROM ciudades c
INNER JOIN provincias p ON c.provincia_id = p.id
ORDER BY p.nombre, c.nombre;
```

## Notas Importantes

1. Los campos `domicilio` y `ciudad_id` son **opcionales (nullable)** para no afectar empleados existentes
2. El código postal tiene un máximo de 10 caracteres para soportar formatos como "C1000" de CABA
3. La relación Ciudad → Provincia usa `DeleteBehavior.Restrict` para evitar eliminación accidental de provincias con ciudades asociadas
4. La relación Empleado → Ciudad usa `DeleteBehavior.SetNull` para que si se elimina una ciudad, los empleados no se vean afectados

## Próximos Pasos Sugeridos

1. Actualizar el formulario de creación/edición de empleados para incluir los nuevos campos
2. Implementar el dropdown en cascada Provincia → Ciudad con JavaScript
3. Agregar validaciones de domicilio si es necesario
4. Considerar agregar un índice en `ciudades.nombre` si se hacen búsquedas frecuentes
