# Funcionalidad de Exportación de Empleados a Excel

## Descripción
Se agregó la capacidad de exportar el listado de empleados a un archivo Excel (.xlsx) desde la vista Index de Empleados.

## Fecha de Implementación
19 de noviembre de 2026

## Archivos Modificados

### 1. Controller: `EmpleadosController.cs`
- **Método agregado**: `ExportarExcel()`
- **Ruta**: `GET /Empleados/ExportarExcel`
- **Funcionalidad**: 
  - Exporta todos los empleados filtrados a Excel
  - Respeta los filtros activos en la vista (nombre, apellido, CUIL, proyecto, estado)
  - Genera un archivo Excel con formato profesional

### 2. Vista: `Index.cshtml`
- **Botón agregado**: "Exportar a Excel"
- **Ubicación**: Barra de acciones superior (antes de "Cargar desde PDF" y "Nuevo Empleado")
- **Color**: Verde (`btn-success`)
- **Funcionalidad**: Pasa los filtros activos como parámetros a la acción de exportación

## Características de la Exportación

### Columnas Exportadas (28 columnas)

**Datos Básicos:**
1. ID
2. Legajo
3. Apellido
4. Nombre
5. Documento
6. CUIL
7. Domicilio
8. Ciudad
9. Código Postal
10. Provincia
11. Cargo

**Fechas:**
12. Fecha Ingreso (formato: dd/MM/yyyy)
13. Fecha Alta (formato: dd/MM/yyyy)
14. Fecha Baja (formato: dd/MM/yyyy)
15. Forma Pago (Quincenal / Mensual)
16. Estado (Activo / Inactivo / Pendiente)

**Régimen y Proyecto:**
17. Régimen Laboral
18. Proyecto Actual

**Datos de Liquidación:**
19. Convenio
20. Tipo Liquidación
21. Categoría
22. Disciplina
23. Valor Hora Básico (formato: $#,##0.00)
24. Valor Convenido Diario (formato: $#,##0.00)
25. Plus Altura (Sí / No)
26. Plus Eléctrico (Sí / No)

**Auditoría:**
27. Fecha Creación (formato: dd/MM/yyyy HH:mm)
28. Fecha Actualización (formato: dd/MM/yyyy HH:mm)

### Formato del Archivo
- **Encabezados**: 
  - Negrita
  - Fondo azul claro
  - Texto centrado
  - Borde exterior
- **Datos**: 
  - Ordenados por Apellido y Nombre
  - Columnas ajustadas automáticamente al contenido
  - Formato de fechas: dd/MM/yyyy
  - Formato de fechas con hora: dd/MM/yyyy HH:mm
  - Formato monetario: $#,##0.00
  - Estado traducido a español (true/false/null → Activo/Inactivo/Pendiente)
  - Booleanos traducidos a Sí/No

### Nombre del Archivo
Formato: `Empleados_yyyyMMdd_HHmmss.xlsx`
Ejemplo: `Empleados_20261119_153045.xlsx`

## Filtros Soportados
La exportación respeta todos los filtros aplicados en la vista Index:
- ✅ Filtro por Nombre
- ✅ Filtro por Apellido
- ✅ Filtro por CUIL
- ✅ Filtro por Proyecto
- ✅ Filtro por Estado (Activo/Inactivo)

Si no hay filtros activos, exporta **todos** los empleados.

## Tecnología Utilizada
- **Librería**: ClosedXML (versión 0.104.2)
- Ya estaba instalada en el proyecto
- No se requirió instalación de paquetes adicionales

## Uso

1. Navegar a la vista de Empleados (`/Empleados/Index`)
2. Aplicar los filtros deseados (opcional)
3. Hacer clic en el botón "Exportar a Excel" (verde)
4. El archivo se descargará automáticamente al navegador

## Manejo de Errores
- En caso de error durante la exportación:
  - Se registra el error en el log
  - Se muestra un mensaje de error al usuario
  - Se redirige de vuelta a la vista Index

## Notas Técnicas
- El método usa `MemoryStream` para generar el archivo en memoria
- El tipo MIME es: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- Los datos nulos se convierten a cadena vacía en el Excel
- La forma de pago enum se traduce a texto legible

## Posibles Mejoras Futuras
- [ ] Agregar más opciones de formato (colores por estado)
- [ ] Permitir seleccionar qué columnas exportar
- [ ] Agregar gráficos o estadísticas en el Excel
- [ ] Exportar a otros formatos (CSV, PDF)
- [ ] Agregar filtro de fecha en la exportación
