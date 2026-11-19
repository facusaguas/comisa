# Cambios Implementados - Provincia, Ciudad y Domicilio en UI

## ✅ Resumen de Cambios

Se han agregado exitosamente los campos de **Domicilio**, **Provincia** y **Ciudad** a las vistas de empleados (Crear, Editar y Detalle), con dropdown en cascada funcional.

---

## 📋 Archivos Modificados

### 1. **ViewModels**

#### `Models/ViewModels/EmpleadoViewModel.cs`
- ✅ Agregado campo `Domicilio` (string, max 200 caracteres)
- ✅ Agregado campo `ProvinciaId` (int, nullable) - para filtro en cascada
- ✅ Agregado campo `CiudadId` (int, nullable)
- ✅ Agregadas propiedades de visualización:
  - `ProvinciaNombre`
  - `CiudadNombre`
  - `CodigoPostal`

### 2. **Controller**

#### `Controllers/EmpleadosController.cs`
- ✅ Actualizado método `LoadSelectListsAsync()` para cargar Provincias y Ciudades
- ✅ Agregado endpoint API `GetCiudadesByProvincia(int provinciaId)` que retorna JSON con ciudades filtradas por provincia

### 3. **Service**

#### `Services/EmpleadoService.cs`
- ✅ Actualizado `GetByIdAsync()` para incluir `.Include(e => e.Ciudad).ThenInclude(c => c.Provincia)`
- ✅ Actualizado `MapToViewModel()` para mapear Domicilio, CiudadId, ProvinciaNombre, CiudadNombre y CodigoPostal
- ✅ Actualizado `CreateAsync()` para incluir Domicilio y CiudadId
- ✅ Actualizado `UpdateAsync()` para incluir Domicilio y CiudadId

### 4. **Vistas**

#### `Views/Empleados/Create.cshtml`
**Cambios:**
- ✅ Movido campo `Cargo` para reordenar layout
- ✅ Agregado campo `Domicilio` (input text, full width)
- ✅ Agregado campo `Provincia` (select dropdown)
- ✅ Agregado campo `Ciudad` (select dropdown dependiente)
- ✅ Actualizado JavaScript para manejar dropdown en cascada Provincia → Ciudad
- ✅ Las ciudades muestran nombre + código postal: "Córdoba (CP: 5000)"

#### `Views/Empleados/Edit.cshtml`
**Cambios:**
- ✅ Movido campo `Cargo` para reordenar layout
- ✅ Agregado campo `Domicilio` (input text, full width)
- ✅ Agregado campo `Provincia` (select dropdown)
- ✅ Agregado campo `Ciudad` (select dropdown dependiente)
- ✅ Actualizado JavaScript para manejar dropdown en cascada Provincia → Ciudad
- ✅ Mantiene selección previa al editar (con `ciudadIdActual` y `provinciaId`)

#### `Views/Empleados/Details.cshtml`
**Cambios:**
- ✅ Agregado campo `Domicilio` en la visualización
- ✅ Agregado campo `Ciudad` con código postal: "Córdoba (CP: 5000)"
- ✅ Agregado campo `Provincia`

---

## 🎨 Estructura de la UI

### Layout en Create/Edit

```
[Legajo]  [Doc. Identidad]  [CUIL]

[Nombre]  [Apellido]

[Cargo]   [Fecha Ingreso]   [Fecha Alta]

[Domicilio - full width]

[Provincia]  [Ciudad]

[Régimen Laboral]  [Proyecto Actual]

[Estado Activo checkbox]
```

### Dropdown en Cascada

1. Usuario selecciona **Provincia**
2. JavaScript hace fetch a `/Empleados/GetCiudadesByProvincia?provinciaId={id}`
3. Endpoint retorna JSON: `[{ id: 1, nombre: "Córdoba", codigoPostal: "5000" }]`
4. JavaScript popula el dropdown **Ciudad** con las opciones
5. Se muestra: "Córdoba (CP: 5000)"

---

## 🔄 Flujo de Datos

### Crear Empleado
1. Usuario completa formulario con domicilio, provincia y ciudad
2. POST a `/Empleados/Create` con `EmpleadoViewModel`
3. `EmpleadoService.CreateAsync()` mapea a `Empleado` entity
4. Guarda en BD con `Domicilio` y `CiudadId`
5. Redirect a Details

### Editar Empleado
1. GET `/Empleados/Edit/{id}` carga empleado con ciudad y provincia
2. `MapToViewModel()` incluye `CiudadId`, `ProvinciaId`, nombres y CP
3. JavaScript detecta `provinciaId` preseleccionado
4. Carga ciudades de esa provincia automáticamente
5. Selecciona la ciudad actual con `ciudadIdActual`
6. POST actualiza con nuevos valores

### Ver Detalle
1. GET `/Empleados/Details/{id}` con Include de Ciudad y Provincia
2. Muestra:
   - Domicilio: "Calle Falsa 123"
   - Ciudad: "Córdoba (CP: 5000)"
   - Provincia: "Córdoba"

---

## 📊 API Endpoints

### `GET /Empleados/GetCiudadesByProvincia`

**Parámetros:**
- `provinciaId` (int, required)

**Response:**
```json
[
  {
    "id": 1,
    "nombre": "Córdoba",
    "codigoPostal": "5000"
  },
  {
    "id": 2,
    "nombre": "Villa María",
    "codigoPostal": "5900"
  }
]
```

---

## ✨ Características Implementadas

1. ✅ **Dropdown en Cascada**: Provincia → Ciudad con JavaScript vanilla
2. ✅ **Código Postal Visible**: Se muestra en el dropdown para referencia
3. ✅ **Validación**: Campos opcionales (nullable) para no romper datos existentes
4. ✅ **Persistencia**: Selección se mantiene al editar
5. ✅ **Manejo de Errores**: Mensajes claros si no hay ciudades disponibles
6. ✅ **Loading State**: Dropdown se deshabilita mientras carga datos
7. ✅ **UX Mejorada**: Mensajes claros "Seleccione primero una provincia..."

---

## 🧪 Testing Manual

### Caso 1: Crear Empleado Nuevo
1. Ir a `/Empleados/Create`
2. Completar datos básicos
3. Seleccionar Provincia (ej: "Córdoba")
4. Ver que se cargan las ciudades de Córdoba
5. Seleccionar Ciudad (ej: "Córdoba (CP: 5000)")
6. Ingresar Domicilio
7. Guardar
8. Verificar en Details que se muestren todos los datos

### Caso 2: Editar Empleado Existente
1. Ir a `/Empleados/Edit/{id}`
2. Si el empleado tiene ciudad, debe aparecer preseleccionada
3. Cambiar provincia, ver que se actualizan las ciudades
4. Cambiar ciudad
5. Guardar
6. Verificar cambios en Details

### Caso 3: Empleado Sin Ciudad (Datos Antiguos)
1. Editar un empleado viejo sin ciudad
2. Los campos deben aparecer vacíos
3. Puede asignarle ciudad opcionalmente
4. No debe dar error si se deja vacío

---

## 🚀 Pasos de Deployment

### 1. Ejecutar Script SQL
Primero ejecuta el script para crear las tablas y datos:
```sql
-- Ubicación: Data/CrearProvinciaCiudadYActualizarEmpleado.sql
```

### 2. Compilar Proyecto
```bash
dotnet build
```

### 3. Verificar Compilación
- ✅ Build exitoso (9 warnings normales del proyecto)
- ✅ No hay errores

### 4. Probar en Desarrollo
1. Ejecutar aplicación
2. Navegar a Empleados
3. Probar crear/editar/ver
4. Verificar dropdown en cascada

---

## 📝 Notas Adicionales

- Los campos son **opcionales** (nullable) para mantener compatibilidad con datos existentes
- El dropdown en cascada usa **Vanilla JavaScript** (no jQuery)
- Se usa **fetch API** para cargar ciudades dinámicamente
- El código postal se muestra como referencia pero no se guarda por separado (viene de la relación con Ciudad)
- Si no hay provincias/ciudades en BD, ejecutar primero el script SQL

---

## 🔧 Troubleshooting

### Dropdown de ciudades no carga
- Verificar que las tablas provincias/ciudades existen en BD
- Verificar que hay datos en esas tablas
- Abrir consola del navegador para ver errores de JavaScript
- Verificar que el endpoint `/Empleados/GetCiudadesByProvincia` responde

### No aparecen los campos nuevos
- Hacer clean & rebuild del proyecto
- Verificar que se modificó `EmpleadoViewModel.cs`
- Verificar que las vistas tienen los campos agregados

### Error al guardar
- Verificar que la columna `ciudad_id` existe en tabla `empleados`
- Verificar que existe la foreign key a `ciudades`
- Revisar logs de errores

---

## ✅ Checklist de Implementación

- [x] Entidades Provincia y Ciudad creadas
- [x] Migración creada (opcional, usamos script SQL)
- [x] Script SQL creado y documentado
- [x] ViewModel actualizado
- [x] Controller actualizado con endpoints
- [x] Service actualizado (CRUD completo)
- [x] Vista Create actualizada
- [x] Vista Edit actualizada
- [x] Vista Details actualizada
- [x] JavaScript para dropdown en cascada
- [x] Proyecto compila sin errores
- [x] Documentación completa

---

**Fecha:** 19 de Noviembre, 2026
**Desarrollador:** Kiro AI Assistant
**Estado:** ✅ COMPLETADO
