# Scripts de Base de Datos

Este directorio contiene los scripts SQL para inicializar y poblar la base de datos del Gestor de Nómina.

## Archivos Disponibles

### Scripts de Inicialización

- **`InitializeDatabase.sql`** - Script maestro que carga todos los datos iniciales
- **`SeedData.sql`** - Datos básicos del sistema (roles, regímenes, actividades, etc.)
- **`CreateTestUsers.sql`** - Crea usuarios de prueba para cada rol
- **`CreateAdminUser.sql`** - Crea solo el usuario administrador

### Scripts de Liquidación

- **`SeedDataLiquidacion.sql`** - Datos iniciales para el módulo de liquidación

### Scripts de Utilidad

- **`UpdateConceptosLiquidacion.sql`** - Script para actualizar la estructura de conceptos de liquidación

## Uso Rápido

### Opción 1: Usar el script PowerShell (Recomendado)

```powershell
# Desde la raíz del proyecto
.\InitializeDB.ps1
```

O con parámetros personalizados:

```powershell
.\InitializeDB.ps1 -Host "localhost" -Port "5432" -Database "gestor_nomina_dev" -User "postgres" -Password "tu_password"
```

### Opción 2: Usar psql directamente

```powershell
# Establecer password
$env:PGPASSWORD="postgres"

# Ejecutar script
psql -h localhost -U postgres -d gestor_nomina_dev -f Data\InitializeDatabase.sql
```

### Opción 3: Usar herramienta gráfica

1. Abre **pgAdmin** o **DBeaver**
2. Conecta a la base de datos `gestor_nomina_dev`
3. Abre el archivo `Data\InitializeDatabase.sql`
4. Ejecuta el script

## Usuarios de Prueba Creados

Todos los usuarios tienen la contraseña: **`Test123!`**

| Email | Rol | Descripción |
|-------|-----|-------------|
| `admin@test.com` | Administrador | Acceso completo al sistema |
| `supervisor@test.com` | Supervisor | Gestión de proyectos y validaciones |
| `operador@test.com` | Operador | Carga de planillas Excel |
| `consulta@test.com` | Consulta | Solo lectura de reportes |

## Datos Cargados

El script `InitializeDatabase.sql` carga:

1. **Roles** (4 roles)
   - Administrador
   - Supervisor
   - Operador
   - Consulta

2. **Regímenes Laborales** (5 regímenes)
   - 14x14 (Minero)
   - 7x7 (Minero)
   - 20x10 (Especial)
   - 6x1 (Semanal estándar)
   - 5x2 (Oficina)

3. **Actividades** (12 actividades)
   - Horas normales y extras
   - Licencias
   - Francos y ausencias
   - Capacitación

4. **Reglas Normativas** (10 reglas)
   - Validaciones de horas y jornadas
   - Control de límites laborales
   - Detección de inconsistencias

5. **Feriados Nacionales** (2024-2026)
   - Feriados de Argentina

6. **Usuarios de Prueba** (4 usuarios)
   - Uno por cada rol del sistema

## Notas Importantes

- **Contraseña por defecto**: Todos los usuarios de prueba tienen la contraseña `Test123!`
- **Hash BCrypt**: Los passwords están hasheados con BCrypt (work factor 11)
- **Regeneración**: Los scripts son idempotentes - se pueden ejecutar múltiples veces
- **Datos de prueba**: Recuerda cambiar o eliminar estos usuarios en producción

## Troubleshooting

### Error: "psql no se reconoce como comando"

Asegúrate de tener PostgreSQL instalado y agregado al PATH del sistema.

### Error de conexión

Verifica:
- El servidor PostgreSQL está corriendo
- El puerto es el correcto (por defecto 5432)
- Las credenciales son correctas
- La base de datos existe (se debe crear con las migraciones de EF Core)

### Base de datos no existe

Primero ejecuta:

```powershell
dotnet ef database update
```

Esto creará la base de datos y aplicará las migraciones. Luego ejecuta el script de inicialización.
