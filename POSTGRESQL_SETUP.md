# Configuración de PostgreSQL para Gestor de Nómina

## 📋 Requisitos Previos

- PostgreSQL 12 o superior instalado
- Acceso a pgAdmin o línea de comandos psql

## 🔧 Instalación de PostgreSQL (Windows)

### Opción 1: Descarga Oficial

1. Descargar desde: https://www.postgresql.org/download/windows/
2. Ejecutar el instalador
3. Durante la instalación:
   - Puerto por defecto: **5432**
   - Contraseña del superusuario (postgres): **Anotar para uso posterior**
   - Locale: Spanish, Argentina o el de tu preferencia

### Opción 2: Using Chocolatey (Windows)

```powershell
choco install postgresql
```

### Opción 3: Docker (Recomendado para Desarrollo)

```powershell
docker run --name postgres-gestor-nomina `
  -e POSTGRES_PASSWORD=postgres `
  -e POSTGRES_DB=gestor_nomina_dev `
  -p 5432:5432 `
  -d postgres:15
```

## 🗄️ Crear Base de Datos

### Usando psql (Línea de Comandos)

```bash
# Conectarse a PostgreSQL
psql -U postgres

# Crear base de datos de desarrollo
CREATE DATABASE gestor_nomina_dev;

# Crear base de datos de producción
CREATE DATABASE gestor_nomina;

# Verificar bases de datos creadas
\l

# Conectarse a la base de datos
\c gestor_nomina_dev

# Salir
\q
```

### Usando pgAdmin

1. Abrir pgAdmin
2. Conectarse al servidor PostgreSQL (localhost)
3. Click derecho en "Databases" → "Create" → "Database"
4. Nombre: `gestor_nomina_dev`
5. Owner: `postgres`
6. Click "Save"

## 🚀 Aplicar Migraciones

Una vez que PostgreSQL esté configurado y las bases de datos creadas:

### 1. Verificar la Cadena de Conexión

Editar `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gestor_nomina_dev;Username=postgres;Password=TU_CONTRASEÑA_AQUI"
  }
}
```

### 2. Aplicar la Migración

Desde la raíz del proyecto:

```bash
dotnet ef database update --project GestorNomina.Web/GestorNomina.Web.csproj --context ApplicationDbContext
```

### 3. Verificar que las Tablas se Crearon

```sql
-- Conectarse a la base de datos
psql -U postgres -d gestor_nomina_dev

-- Listar todas las tablas
\dt

-- Debería mostrar 13 tablas:
-- - actividades
-- - empleado_proyecto
-- - empleados
-- - feriados
-- - horas_registros_aprobados
-- - horas_registros_raw
-- - periodos_carga
-- - proyectos
-- - regimenes_laborales
-- - reglas_normativas
-- - roles
-- - usuarios
-- - validaciones_registro
```

## 📊 Cargar Datos Iniciales

Después de aplicar las migraciones, ejecutar el script de datos iniciales:

```bash
psql -U postgres -d gestor_nomina_dev -f GestorNomina.Web/Data/SeedData.sql
```

O desde pgAdmin:
1. Conectarse a `gestor_nomina_dev`
2. Tools → Query Tool
3. Abrir el archivo `SeedData.sql`
4. Ejecutar (F5)

## 🔍 Verificación Post-Instalación

### 1. Verificar Tablas y Datos

```sql
-- Contar tablas
SELECT COUNT(*) FROM information_schema.tables 
WHERE table_schema = 'public';

-- Verificar roles
SELECT * FROM roles;

-- Verificar regímenes laborales
SELECT * FROM regimenes_laborales;

-- Verificar actividades
SELECT COUNT(*) FROM actividades;

-- Verificar reglas normativas
SELECT COUNT(*) FROM reglas_normativas WHERE activa = true;
```

### 2. Probar la Conexión desde .NET

Ejecutar el proyecto:

```bash
dotnet run --project GestorNomina.Web/GestorNomina.Web.csproj
```

Si no hay errores de conexión, PostgreSQL está correctamente configurado.

## 🛠️ Comandos Útiles de PostgreSQL

### Gestión de Base de Datos

```sql
-- Listar bases de datos
\l

-- Conectarse a una base de datos
\c gestor_nomina_dev

-- Listar tablas
\dt

-- Describir una tabla
\d empleados

-- Ver todos los índices
\di

-- Ver todas las relaciones (foreign keys)
SELECT
    tc.table_name, 
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY';
```

### Backup y Restore

```bash
# Backup
pg_dump -U postgres -d gestor_nomina_dev -F c -b -v -f backup.dump

# Restore
pg_restore -U postgres -d gestor_nomina_dev -v backup.dump
```

## ⚠️ Solución de Problemas

### Error: "password authentication failed"

1. Verificar que la contraseña en `appsettings.json` sea correcta
2. Revisar el archivo `pg_hba.conf` (cambiar `md5` a `trust` temporalmente para desarrollo)

### Error: "could not connect to server"

1. Verificar que PostgreSQL esté corriendo:
   ```powershell
   Get-Service -Name postgresql*
   ```
2. Verificar el puerto 5432:
   ```powershell
   netstat -an | Select-String "5432"
   ```

### Error: "database does not exist"

Crear la base de datos manualmente:
```bash
createdb -U postgres gestor_nomina_dev
```

### Las tablas no se crean

1. Verificar que la migración se haya generado:
   ```bash
   ls GestorNomina.Web/Migrations/
   ```
2. Eliminar y recrear la migración:
   ```bash
   dotnet ef migrations remove --project GestorNomina.Web/GestorNomina.Web.csproj
   dotnet ef migrations add InitialCreate --project GestorNomina.Web/GestorNomina.Web.csproj
   dotnet ef database update --project GestorNomina.Web/GestorNomina.Web.csproj
   ```

## 📚 Recursos Adicionales

- Documentación oficial de PostgreSQL: https://www.postgresql.org/docs/
- Npgsql (PostgreSQL para .NET): https://www.npgsql.org/
- Entity Framework Core: https://docs.microsoft.com/en-us/ef/core/
- pgAdmin: https://www.pgadmin.org/

## 🔐 Seguridad en Producción

**IMPORTANTE**: Las credenciales en los archivos de configuración son solo para desarrollo.

Para producción:
1. Usar **User Secrets** en desarrollo
2. Usar **Azure Key Vault** o **AWS Secrets Manager** en producción
3. Nunca commitear contraseñas al control de versiones
4. Cambiar la contraseña del usuario `postgres` por defecto
5. Crear un usuario específico con permisos limitados para la aplicación

```sql
-- Crear usuario específico para la aplicación
CREATE USER gestor_app WITH PASSWORD 'contraseña_segura';

-- Otorgar permisos solo a la base de datos específica
GRANT ALL PRIVILEGES ON DATABASE gestor_nomina TO gestor_app;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO gestor_app;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO gestor_app;
```

Luego actualizar la cadena de conexión:
```
Host=localhost;Port=5432;Database=gestor_nomina;Username=gestor_app;Password=contraseña_segura
```
