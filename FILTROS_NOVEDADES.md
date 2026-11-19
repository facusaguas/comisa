# Filtros en Vista de Novedades

## Resumen de Cambios

Se han agregado filtros avanzados a la vista de novedades para facilitar la búsqueda y visualización de información. Los usuarios ahora pueden filtrar novedades por múltiples criterios simultáneamente.

## Filtros Implementados

Los siguientes filtros están disponibles en la vista de novedades:

1. **Legajo**: Búsqueda por número de legajo del empleado (búsqueda parcial)
2. **CUIL**: Búsqueda por CUIL del empleado (búsqueda parcial)
3. **Proyecto**: Filtro por proyecto asignado al empleado (dropdown con todos los proyectos)
4. **Periodo**: Filtro por mes y año (selector de mes/año)
5. **Quincena**: Filtro por primera o segunda quincena
6. **Tipo de Novedad**: Filtro por tipo específico de novedad (Embargo, Inicio Roster, Fin Roster, Enfermedad, Baja, Vacaciones, Anticipo)

## Características

### Panel de Filtros Colapsable
- El panel de filtros es colapsable para ahorrar espacio en pantalla
- Por defecto se muestra expandido
- Los filtros se organizan en dos filas para mejor visualización

### Filtrado Flexible
- Los filtros se pueden combinar
- No es necesario completar todos los campos
- La búsqueda por legajo y CUIL es de tipo "contiene" (no requiere coincidencia exacta)

### Visualización de Filtros Activos
- Cuando hay filtros aplicados, se muestra un resumen en la parte superior de la página
- El resumen muestra todos los filtros activos en formato legible

### Botones de Acción
- **Filtrar**: Aplica los filtros seleccionados
- **Limpiar**: Elimina todos los filtros y muestra todas las novedades

### Tabla Mejorada
La tabla de novedades ahora incluye columnas adicionales:
- Legajo (separado)
- Empleado (Apellido, Nombre)
- CUIL
- Proyecto asignado
- Periodo/Quincena
- Tipo de novedad (con badge de color)
- Fecha de novedad
- Monto
- Acciones (Editar/Eliminar)

## Archivos Modificados

### 1. Services/INovedadService.cs
- Se agregó el método `GetFilteredAsync` para soportar filtrado múltiple

### 2. Services/NovedadService.cs
- Implementación del método `GetFilteredAsync` con consultas LINQ
- Incluye la relación con Proyecto para mostrar la información correctamente
- Los filtros son opcionales y se aplican solo si tienen valor

### 3. Controllers/NovedadesController.cs
- Se inyectó `IProyectoService` para cargar los proyectos
- Se actualizó el método `Index` para recibir y procesar múltiples parámetros de filtro
- Se agregó el método `CargarFiltrosDropdown` para poblar los dropdowns del formulario
- Se agregó el método `GetTipoNovedadNombre` para convertir el enum a texto legible
- Los valores de los filtros se mantienen en ViewBag para preservar el estado del formulario

### 4. Views/Novedades/Index.cshtml
- Se agregó un panel de filtros colapsable con todos los campos de búsqueda
- Se actualizó la tabla para mostrar las columnas adicionales
- Se mejoró la visualización de los filtros activos
- Se agregó el caso de "Vacaciones" en el switch de tipos de novedad

## Uso

### Para filtrar novedades:

1. Acceder a la vista de Novedades
2. Completar uno o más campos de filtro según sea necesario
3. Hacer clic en "Filtrar"
4. Los resultados mostrarán solo las novedades que cumplan con todos los criterios seleccionados

### Para limpiar filtros:

- Hacer clic en el botón "Limpiar" para volver a la vista sin filtros

## Ejemplos de Uso

### Ejemplo 1: Buscar novedades de un legajo específico
- Ingresar el legajo en el campo "Legajo"
- Hacer clic en "Filtrar"

### Ejemplo 2: Ver todas las novedades de un proyecto en un periodo
- Seleccionar el proyecto del dropdown
- Seleccionar el mes/año en "Periodo"
- Hacer clic en "Filtrar"

### Ejemplo 3: Ver embargos del último mes
- Seleccionar "Embargo" en "Tipo de Novedad"
- Seleccionar el mes actual en "Periodo"
- Hacer clic en "Filtrar"

### Ejemplo 4: Ver novedades de la segunda quincena
- Seleccionar "Segunda (2)" en "Quincena"
- Opcionalmente seleccionar también el periodo
- Hacer clic en "Filtrar"

## Notas Técnicas

- Los filtros se aplican mediante query strings en la URL
- La búsqueda por legajo y CUIL usa el método `Contains` para búsqueda parcial
- Los filtros de proyecto, quincena y tipo de novedad requieren coincidencia exacta
- El filtro de periodo solo compara mes y año (ignora el día)
- Los datos se ordenan por: Periodo DESC, Quincena DESC, Apellido, Nombre

## Mejoras Futuras Sugeridas

1. Agregar paginación para manejar grandes volúmenes de datos
2. Agregar exportación a Excel de resultados filtrados
3. Guardar filtros favoritos del usuario
4. Agregar filtro por rango de fechas
5. Agregar filtro por estado (procesado/pendiente)
