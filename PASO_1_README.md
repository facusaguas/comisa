# PASO 1 - Configuración de la Solución .NET MVC y Base de Datos

## ✅ Completado

Este documento describe la estructura creada en el PASO 1 del proyecto de Gestión y Validación de Horas Laborales.

## 📁 Estructura del Proyecto

```
GestorNomina/
├── GestorNomina.sln
└── GestorNomina.Web/
    ├── Controllers/
    ├── Data/
    │   ├── ApplicationDbContext.cs
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

## 🗂️ Entidades Creadas (13 Tablas)

### 1. **roles**
- Gestión de roles de usuario (Admin, Supervisor, etc.)
- Campos: id, nombre, descripcion, created_at, updated_at

### 2. **usuarios**
- Usuarios del sistema con autenticación
- Campos: id, rol_id, email, password_hash, nombre, apellido, activo, created_at, updated_at

### 3. **regimenes_laborales**
- Definición de regímenes (14x14, 7x7, etc.)
- Campos: id, nombre, dias_trabajo, dias_descanso, max_horas_diarias, horas_semanales_limite, descripcion

### 4. **proyectos**
- Proyectos mineros o de alta montaña
- Campos: id, codigo, nombre, ubicacion, regimen_default_id, activo, created_at, updated_at

### 5. **empleados**
- Maestro de empleados
- Campos: id, legajo, nombre, apellido, documento_identidad, cargo, regimen_id, fecha_ingreso, activo, created_at, updated_at

### 6. **empleado_proyecto**
- Relación N:M entre Empleados y Proyectos
- Campos: id, empleado_id, proyecto_id, fecha_desde, fecha_hasta, regimen_especifico_id, created_at, updated_at

### 7. **feriados**
- Calendario de feriados
- Campos: id, fecha, descripcion, tipo, jurisdiccion

### 8. **reglas_normativas**
- Reglas de validación normativa
- Campos: id, codigo, nombre, descripcion, nivel_severidad, activa

### 9. **periodos_carga**
- Control de períodos de carga de Excel
- Campos: id, proyecto_id, mes, anio, estado, usuario_carga_id, nombre_archivo_original, hash_archivo, created_at, updated_at

### 10. **actividades**
- Catálogo de actividades/códigos de horas
- Campos: id, codigo, descripcion, es_hora_efectiva, es_licencia, es_franco

### 11. **horas_registros_raw**
- Datos crudos importados desde Excel (staging)
- Campos: id, periodo_carga_id, empleado_legajo, empleado_id, actividad_codigo, actividad_id, fecha, horas_cargadas, fila_excel_origen, created_at

### 12. **horas_registros_aprobados**
- Registros aprobados y corregidos (relación 1:1 con raw)
- Campos: id, registro_raw_id, periodo_carga_id, empleado_id, actividad_id, fecha, horas_finales, fue_modificado, motivo_modificacion, usuario_modificador_id, updated_at

### 13. **validaciones_registro**
- Observaciones y propuestas de corrección
- Campos: id, registro_raw_id, regla_id, mensaje_observacion, propuesta_sugerida, valor_sugerido_horas, actividad_sugerida_id, estado, created_at, updated_at

## 🔗 Relaciones Configuradas

- **Usuario → Rol** (N:1, Restrict)
- **Proyecto → RegimenLaboral** (N:1, SetNull)
- **Empleado → RegimenLaboral** (N:1, SetNull)
- **EmpleadoProyecto → Empleado** (N:1, Cascade)
- **EmpleadoProyecto → Proyecto** (N:1, Cascade)
- **EmpleadoProyecto → RegimenLaboral** (N:1, SetNull)
- **PeriodoCarga → Proyecto** (N:1, Restrict)
- **PeriodoCarga → Usuario** (N:1, SetNull)
- **HoraRegistroRaw → PeriodoCarga** (N:1, Cascade)
- **HoraRegistroRaw → Empleado** (N:1, SetNull)
- **HoraRegistroRaw → Actividad** (N:1, SetNull)
- **HoraRegistroAprobado → HoraRegistroRaw** (1:1, Cascade)
- **HoraRegistroAprobado → PeriodoCarga** (N:1, Restrict)
- **HoraRegistroAprobado → Empleado** (N:1, Restrict)
- **HoraRegistroAprobado → Actividad** (N:1, Restrict)
- **HoraRegistroAprobado → Usuario** (N:1, SetNull)
- **ValidacionRegistro → HoraRegistroRaw** (N:1, Cascade)
- **ValidacionRegistro → ReglaNormativa** (N:1, Restrict)
- **ValidacionRegistro → Actividad** (N:1, SetNull)

## 📊 Índices Creados

### Índices Únicos:
- `roles.nombre`
- `usuarios.email`
- `proyectos.codigo`
- `empleados.legajo`
- `actividades.codigo`
- `reglas_normativas.codigo`

### Índices Compuestos:
- `feriados(fecha, jurisdiccion)`
- `periodos_carga(proyecto_id, mes, anio)`
- `horas_registros_raw(periodo_carga_id, empleado_id, fecha)`
- `horas_registros_aprobados(periodo_carga_id, empleado_id, fecha)`

## 🔧 Configuración de PostgreSQL

### appsettings.json (Producción)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gestor_nomina;Username=postgres;Password=postgres"
  }
}
```

### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gestor_nomina_dev;Username=postgres;Password=postgres"
  }
}
```

## 📦 Paquetes NuGet Instalados

- **Npgsql.EntityFrameworkCore.PostgreSQL** (v8.0.10)
- **Microsoft.EntityFrameworkCore.Design** (v8.0.10)
- **Microsoft.EntityFrameworkCore** (v8.0.10) - Dependencia
- **Microsoft.EntityFrameworkCore.Relational** (v8.0.10) - Dependencia

## 🚀 Próximos Comandos

### Para crear la base de datos (una vez configurado PostgreSQL):

```bash
# Aplicar la migración
dotnet ef database update --project GestorNomina.Web/GestorNomina.Web.csproj --context ApplicationDbContext

# Ver el SQL que se ejecutará
dotnet ef migrations script --project GestorNomina.Web/GestorNomina.Web.csproj --context ApplicationDbContext
```

### Para compilar el proyecto:

```bash
dotnet build
```

### Para ejecutar el proyecto:

```bash
dotnet run --project GestorNomina.Web/GestorNomina.Web.csproj
```

## ⚙️ Características Implementadas

✅ Arquitectura ASP.NET Core MVC con .NET 8
✅ Entity Framework Core con Code First Approach
✅ PostgreSQL como motor de base de datos
✅ 13 entidades mapeadas según el DER
✅ Relaciones completas configuradas (1:1, 1:N, N:M)
✅ Índices para optimización de consultas
✅ Precisión decimal configurada para campos de horas
✅ Convenciones de nomenclatura snake_case para PostgreSQL
✅ Migración inicial generada
✅ Configuración de sesiones para futura autenticación

## 📝 Notas Importantes

1. **Base de Datos**: Antes de ejecutar `dotnet ef database update`, asegúrate de tener PostgreSQL instalado y configurado.

2. **Contraseñas**: Las contraseñas en los archivos de configuración son de ejemplo. En producción, utiliza secretos seguros.

3. **Migraciones**: La migración `InitialCreate` ya está generada. Solo falta aplicarla a la base de datos.

4. **Convenciones**: Las entidades utilizan Data Annotations para mapeo explícito con nomenclatura snake_case.

## 🎯 Estado del PASO 1

**✅ COMPLETADO AL 100%**

Todos los componentes del PASO 1 han sido implementados:
- ✅ Solución .NET creada
- ✅ Proyecto ASP.NET Core MVC configurado
- ✅ 13 Entidades definidas
- ✅ DbContext configurado
- ✅ Relaciones y restricciones implementadas
- ✅ Índices creados
- ✅ Migración inicial generada
- ✅ Archivos de configuración establecidos

## 🔜 Siguiente Paso

**PASO 2**: Diseño de la Plantilla Base (Layout) utilizando Tabler UI y Módulo de Autenticación/Usuarios.
