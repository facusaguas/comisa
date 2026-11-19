# Instrucciones para Ejecutar el Script SQL Manual

## Descripción

Este script crea las tablas `provincias` y `ciudades`, y agrega las columnas `domicilio` y `ciudad_id` a la tabla `empleados` **sin usar migraciones de Entity Framework**.

## Archivo del Script

📄 **`CrearProvinciaCiudadYActualizarEmpleado.sql`**

## ¿Qué hace el script?

1. ✅ Crea la tabla `provincias` con 24 provincias argentinas
2. ✅ Crea la tabla `ciudades` con códigos postales
3. ✅ Agrega columnas `domicilio` y `ciudad_id` a la tabla `empleados`
4. ✅ Crea todas las foreign keys necesarias
5. ✅ Crea índices para mejor rendimiento
6. ✅ Pobla las tablas con datos de todas las provincias y +60 ciudades principales
7. ✅ Es **idempotente**: puede ejecutarse múltiples veces sin errores

## Cómo Ejecutar el Script

### Opción 1: Desde la línea de comandos (psql)

```powershell
# Desde la carpeta del proyecto
psql -U tu_usuario -d nombre_base_datos -f "GestorNomina.Web\Data\CrearProvinciaCiudadYActualizarEmpleado.sql"
```

### Opción 2: Desde pgAdmin

1. Abre **pgAdmin**
2. Conéctate a tu servidor PostgreSQL
3. Selecciona tu base de datos
4. Click derecho en la base de datos → **Query Tool**
5. Abre el archivo `CrearProvinciaCiudadYActualizarEmpleado.sql`
6. Click en el botón **Execute** (▶️) o presiona F5

### Opción 3: Desde DBeaver

1. Abre **DBeaver**
2. Conéctate a tu base de datos PostgreSQL
3. Click derecho en la conexión → **SQL Editor** → **New SQL Script**
4. Copia y pega el contenido del archivo `CrearProvinciaCiudadYActualizarEmpleado.sql`
5. Click en **Execute SQL Script** (▶️) o presiona Ctrl+Enter

### Opción 4: Desde Visual Studio Code con extensión PostgreSQL

1. Instala la extensión **PostgreSQL** de Chris Kolkman
2. Conecta a tu base de datos
3. Abre el archivo `CrearProvinciaCiudadYActualizarEmpleado.sql`
4. Click derecho → **Run Query**

## ¿Qué hacer después de ejecutar el script?

### 1. Verificar que se crearon las tablas

```sql
-- Ver todas las provincias
SELECT * FROM provincias ORDER BY nombre;

-- Ver todas las ciudades con sus provincias
SELECT 
    c.nombre as ciudad, 
    c.codigo_postal, 
    p.nombre as provincia
FROM ciudades c
INNER JOIN provincias p ON c.provincia_id = p.id
ORDER BY p.nombre, c.nombre;

-- Ver estructura de empleados actualizada
SELECT 
    column_name, 
    data_type, 
    character_maximum_length,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'empleados'
ORDER BY ordinal_position;
```

### 2. Actualizar el DbContext (ya está hecho)

El `ApplicationDbContext.cs` ya tiene configurados los DbSets:

```csharp
public DbSet<Provincia> Provincias { get; set; }
public DbSet<Ciudad> Ciudades { get; set; }
```

### 3. Sincronizar el modelo de Entity Framework

Dado que agregaste las tablas manualmente, Entity Framework necesita saber que ya existen. Tienes dos opciones:

#### Opción A: Crear una migración vacía (recomendado)

```powershell
# Crear una migración que refleje el estado actual
dotnet ef migrations add SincronizarProvinciaCiudad --project GestorNomina.Web

# Revisar la migración generada y limpiarla si tiene comandos duplicados
# Luego marcarla como aplicada sin ejecutarla
dotnet ef database update --project GestorNomina.Web
```

#### Opción B: No usar migraciones para estas tablas

Si no quieres usar migraciones para estas tablas, simplemente asegúrate de que el código C# coincida con la estructura de la base de datos.

### 4. Actualizar el formulario de empleados

Ahora puedes usar estos campos en tu aplicación:

```csharp
// En tu controller o service
var empleado = new Empleado
{
    Nombre = "Juan",
    Apellido = "Pérez",
    Domicilio = "Av. Libertador 1234",
    CiudadId = 1, // ID de la ciudad
    // ... otros campos
};

_context.Empleados.Add(empleado);
await _context.SaveChangesAsync();
```

## Estructura de las Tablas Creadas

### Tabla: `provincias`

| Columna    | Tipo         | Nullable | Descripción              |
|------------|--------------|----------|--------------------------|
| id         | SERIAL       | NO       | Primary Key              |
| nombre     | VARCHAR(100) | NO       | Nombre de la provincia   |
| created_at | TIMESTAMP    | NO       | Fecha de creación        |
| updated_at | TIMESTAMP    | NO       | Fecha de actualización   |

### Tabla: `ciudades`

| Columna       | Tipo         | Nullable | Descripción              |
|---------------|--------------|----------|--------------------------|
| id            | SERIAL       | NO       | Primary Key              |
| nombre        | VARCHAR(100) | NO       | Nombre de la ciudad      |
| codigo_postal | VARCHAR(10)  | NO       | Código postal            |
| provincia_id  | INTEGER      | NO       | FK a provincias          |
| created_at    | TIMESTAMP    | NO       | Fecha de creación        |
| updated_at    | TIMESTAMP    | NO       | Fecha de actualización   |

### Tabla: `empleados` (columnas agregadas)

| Columna    | Tipo         | Nullable | Descripción              |
|------------|--------------|----------|--------------------------|
| domicilio  | VARCHAR(200) | YES      | Dirección del empleado   |
| ciudad_id  | INTEGER      | YES      | FK a ciudades            |

## Datos Incluidos

- **24 Provincias** de Argentina
- **+60 Ciudades** principales con códigos postales reales:
  - Buenos Aires: 8 ciudades
  - Córdoba: 4 ciudades
  - Santa Fe: 4 ciudades
  - CABA: 1 ciudad
  - Y todas las capitales provinciales

## Agregar Más Ciudades

Para agregar más ciudades después, usa este template:

```sql
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Nombre de la Ciudad', 'CP', id, NOW(), NOW() 
FROM provincias 
WHERE nombre = 'Nombre de la Provincia'
ON CONFLICT DO NOTHING;
```

## Troubleshooting

### Error: "relation already exists"

✅ **Solución**: El script ya maneja esto con `CREATE TABLE IF NOT EXISTS`. Es seguro ejecutarlo de nuevo.

### Error: "column already exists"

✅ **Solución**: El script ya maneja esto con bloques `DO $$`. Es seguro ejecutarlo de nuevo.

### Error: "permission denied"

❌ **Problema**: Tu usuario no tiene permisos suficientes.
✅ **Solución**: Ejecuta el script con un usuario que tenga permisos de CREATE y ALTER.

### Error: "database does not exist"

❌ **Problema**: La base de datos no existe.
✅ **Solución**: Verifica el nombre de la base de datos en tu connection string.

## Notas Importantes

1. 🔒 **El script es seguro**: Usa `IF NOT EXISTS` y `ON CONFLICT DO NOTHING` para evitar errores
2. ♻️ **Es idempotente**: Puede ejecutarse múltiples veces sin problemas
3. 📊 **Incluye datos reales**: Todas las provincias y ciudades principales de Argentina
4. 🔗 **Foreign Keys configuradas**: Con `ON DELETE RESTRICT` y `ON DELETE SET NULL` apropiadamente
5. ⚡ **Índices creados**: Para mejor rendimiento en consultas

## Próximos Pasos Sugeridos

1. ✅ Ejecutar el script SQL
2. ✅ Verificar que las tablas se crearon correctamente
3. 📝 Actualizar los formularios de empleados para incluir domicilio y ciudad
4. 🎨 Implementar un dropdown en cascada Provincia → Ciudad en el frontend
5. 📊 Agregar más ciudades según sea necesario

---

**¿Necesitas ayuda?** Revisa la sección de Troubleshooting o contacta al equipo de desarrollo.
