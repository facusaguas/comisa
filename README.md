# 🏗️ Sistema de Gestión y Validación de Horas Laborales

Sistema web completo para la gestión, validación y auditoría de horas laborales en proyectos mineros y de alta montaña, desarrollado con ASP.NET Core MVC y PostgreSQL.

## 📊 Estado del Proyecto

| Fase | Estado | Progreso |
|------|--------|----------|
| **PASO 1** - Arquitectura y Base de Datos | ✅ Completado | 100% |
| **PASO 2** - Layout Tabler + Autenticación | ⏳ Pendiente | 0% |
| **PASO 3** - CRUDs de Configuración | ⏳ Pendiente | 0% |
| **PASO 4** - Módulo de Ingesta Excel | ⏳ Pendiente | 0% |
| **PASO 5** - Motor de Validación Normativa | ⏳ Pendiente | 0% |
| **PASO 6** - UI de Corrección Interactiva | ⏳ Pendiente | 0% |
| **PASO 7** - Exportación y Reportes | ⏳ Pendiente | 0% |

## 🚀 Stack Tecnológico

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core MVC** - Arquitectura web
- **Entity Framework Core 8.0.10** - ORM
- **Npgsql 8.0.5** - PostgreSQL provider

### Base de Datos
- **PostgreSQL 12+** - Motor de base de datos relacional
- **13 tablas interrelacionadas** con integridad referencial completa

### Frontend (Próximo paso)
- **Tabler UI** - Framework de diseño
- **Bootstrap 5** - Framework CSS
- **HTML5, CSS3, JavaScript**

## 📁 Estructura del Proyecto

```
GestorNomina/
├── .gitignore
├── README.md
├── PASO_1_README.md
├── POSTGRESQL_SETUP.md
├── GestorNomina.sln
└── GestorNomina.Web/
    ├── Controllers/
    ├── Data/
    │   ├── ApplicationDbContext.cs
    │   ├── SeedData.sql
    │   ├── Configurations/
    │   └── Migrations/
    │       └── [TIMESTAMP]_InitialCreate.cs
    ├── Models/
    │   └── Entities/
    │       ├── Actividad.cs
    │       ├── Empleado.cs
    │       ├── EmpleadoProyecto.cs
    │       ├── Feriado.cs
    │       ├── HoraRegistroAprobado.cs
    │       ├── HoraRegistroRaw.cs
    │       ├── PeriodoCarga.cs
    │       ├── Proyecto.cs
    │       ├── RegimenLaboral.cs
    │       ├── ReglaNormativa.cs
    │       ├── Rol.cs
    │       ├── Usuario.cs
    │       └── ValidacionRegistro.cs
    ├── Views/
    ├── wwwroot/
    ├── appsettings.json
    ├── appsettings.Development.json
    └── Program.cs
```

## 🗄️ Modelo de Datos

### Entidades Principales

1. **Usuarios y Seguridad**
   - `roles` - Roles del sistema
   - `usuarios` - Usuarios y autenticación

2. **Configuración de Nómina**
   - `regimenes_laborales` - Esquemas de trabajo (14x14, 7x7)
   - `proyectos` - Proyectos mineros
   - `empleados` - Maestro de empleados
   - `empleado_proyecto` - Asignaciones N:M

3. **Validación y Normativa**
   - `feriados` - Calendario de feriados
   - `reglas_normativas` - Reglas de validación
   - `actividades` - Códigos de actividades/horas

4. **Procesamiento de Horas**
   - `periodos_carga` - Control de archivos Excel
   - `horas_registros_raw` - Datos crudos (staging)
   - `horas_registros_aprobados` - Datos validados
   - `validaciones_registro` - Observaciones y correcciones

## 🎯 Características Principales

### ✅ Implementado (PASO 1)

- ✅ Arquitectura ASP.NET Core MVC con .NET 8
- ✅ Entity Framework Core con PostgreSQL
- ✅ 13 entidades con relaciones completas
- ✅ Índices optimizados para consultas
- ✅ Migraciones de base de datos generadas
- ✅ Script de datos iniciales (SeedData.sql)
- ✅ Configuración de sesiones
- ✅ Documentación completa

### 🔜 Próximos Pasos

- ⏳ Layout con Tabler UI
- ⏳ Sistema de autenticación y autorización
- ⏳ CRUDs para maestros (Proyectos, Empleados, etc.)
- ⏳ Parser de Excel (formato horizontal → vertical)
- ⏳ Motor de validación normativa
- ⏳ Interfaz de corrección interactiva
- ⏳ Exportación a Excel
- ⏳ Dashboard y reportes

## 🔧 Configuración e Instalación

### 1. Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### 2. Clonar el Repositorio

```bash
git clone <url-del-repositorio>
cd GestorNomina
```

### 3. Configurar PostgreSQL

Ver guía detallada en [POSTGRESQL_SETUP.md](./POSTGRESQL_SETUP.md)

```bash
# Crear base de datos
createdb -U postgres gestor_nomina_dev
```

### 4. Configurar Cadena de Conexión

Editar `GestorNomina.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gestor_nomina_dev;Username=postgres;Password=TU_PASSWORD"
  }
}
```

### 5. Aplicar Migraciones

```bash
dotnet ef database update --project GestorNomina.Web/GestorNomina.Web.csproj
```

### 6. Cargar Datos Iniciales

```bash
psql -U postgres -d gestor_nomina_dev -f GestorNomina.Web/Data/SeedData.sql
```

### 7. Ejecutar la Aplicación

```bash
dotnet run --project GestorNomina.Web/GestorNomina.Web.csproj
```

La aplicación estará disponible en: `https://localhost:7000` (o el puerto asignado)

## 📚 Documentación

- [**PASO_1_README.md**](./PASO_1_README.md) - Documentación detallada del PASO 1
- [**POSTGRESQL_SETUP.md**](./POSTGRESQL_SETUP.md) - Guía de configuración de PostgreSQL
- [**SeedData.sql**](./GestorNomina.Web/Data/SeedData.sql) - Script de datos iniciales

## 🧪 Comandos Útiles

### Entity Framework

```bash
# Agregar nueva migración
dotnet ef migrations add NombreMigracion --project GestorNomina.Web/GestorNomina.Web.csproj

# Aplicar migraciones
dotnet ef database update --project GestorNomina.Web/GestorNomina.Web.csproj

# Revertir última migración
dotnet ef migrations remove --project GestorNomina.Web/GestorNomina.Web.csproj

# Ver SQL de migración
dotnet ef migrations script --project GestorNomina.Web/GestorNomina.Web.csproj
```

### Construcción y Ejecución

```bash
# Compilar
dotnet build

# Ejecutar
dotnet run --project GestorNomina.Web/GestorNomina.Web.csproj

# Ejecutar con watch (recarga automática)
dotnet watch run --project GestorNomina.Web/GestorNomina.Web.csproj

# Publicar
dotnet publish -c Release
```

## 🎨 Convenciones de Código

### Nomenclatura

- **C# Classes**: PascalCase (ej: `RegimenLaboral`)
- **Properties**: PascalCase (ej: `MaxHorasDiarias`)
- **Database Tables**: snake_case (ej: `regimenes_laborales`)
- **Database Columns**: snake_case (ej: `max_horas_diarias`)

### Organización de Archivos

- **Entities**: `Models/Entities/`
- **ViewModels**: `Models/ViewModels/`
- **DTOs**: `Models/DTOs/`
- **Services**: `Services/`
- **Repositories**: `Repositories/`
- **Configuration**: `Data/Configurations/`

## 🔒 Seguridad

⚠️ **IMPORTANTE**: Los archivos de configuración contienen credenciales de desarrollo.

En producción:
- Usar **User Secrets** o variables de entorno
- Implementar **Azure Key Vault** o equivalente
- Cambiar contraseñas por defecto
- Crear usuarios de base de datos específicos con permisos limitados
- Habilitar HTTPS
- Implementar rate limiting
- Configurar CORS apropiadamente

## 👥 Roles del Sistema

Los siguientes roles están precargados en `SeedData.sql`:

1. **Administrador** - Acceso completo
2. **Supervisor** - Gestión de validaciones y aprobaciones
3. **Operador** - Carga de planillas Excel
4. **Consulta** - Solo lectura

## 📊 Flujo del Sistema

```
┌─────────────────┐
│  Carga Excel    │
│  (Operador)     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ horas_registros │
│      _raw       │ ◄─── Staging (sin modificar)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Motor de        │
│ Validación      │ ◄─── reglas_normativas
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ validaciones_   │
│   registro      │ ◄─── Observaciones y propuestas
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ UI Corrección   │
│ (Supervisor)    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ horas_registros │
│   _aprobados    │ ◄─── Datos finales (relación 1:1 con raw)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Exportación     │
│    Excel        │
└─────────────────┘
```

## 🤝 Contribución

Este proyecto está en desarrollo activo. Para contribuir:

1. Fork del repositorio
2. Crear branch feature (`git checkout -b feature/NuevaCaracteristica`)
3. Commit de cambios (`git commit -m 'Agregar nueva característica'`)
4. Push al branch (`git push origin feature/NuevaCaracteristica`)
5. Crear Pull Request

## 📝 Licencia

[Especificar licencia aquí]

## 📧 Contacto

[Información de contacto del equipo]

---

**Última actualización**: PASO 1 Completado - Octubre 2026
**Próximo objetivo**: PASO 2 - Implementación de Tabler UI y Sistema de Autenticación
