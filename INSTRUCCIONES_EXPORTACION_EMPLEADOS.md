# Instrucciones para Exportar Empleados a Excel

## Funcionalidad Implementada

Se ha agregado una nueva funcionalidad para exportar a Excel la lista de empleados de un proyecto organizados por quincena.

## Cómo usar

### 1. Acceder a la funcionalidad

1. Navega a la sección **Proyectos** desde el menú principal
2. Haz clic en **Detalles** del proyecto deseado
3. En la vista de detalles, verás un botón verde **"Exportar Empleados"** en la parte superior derecha

### 2. Configurar la exportación

Al hacer clic en el botón se abrirá un modal con:

- **Información del Proyecto**: Muestra el nombre, código y estadísticas de empleados
- **Estadísticas**: Cantidad total de empleados y distribución por tipo de contratación
- **Selección de Período**:
  - **Mes**: Selecciona el mes deseado (Enero - Diciembre)
  - **Año**: Selecciona el año (por defecto viene el año actual)
  - **Quincena**: 
    - Primera quincena (días 1-15)
    - Segunda quincena (días 16-fin de mes)

### 3. Generar el archivo

1. Completa los tres campos requeridos (Mes, Año, Quincena)
2. El botón **"Generar Excel"** se habilitará automáticamente
3. Haz clic en **"Generar Excel"**
4. El archivo se descargará automáticamente con el nombre:
   - Formato: `Empleados_[CODIGO_PROYECTO]_[MES]_[AÑO]_Q[QUINCENA].xlsx`
   - Ejemplo: `Empleados_PRY001_Octubre_2026_Q1.xlsx`

## Estructura del Excel Generado

### Encabezados

El Excel tiene 3 filas de encabezado:

**Fila 1**: Título de la quincena
- Ejemplo: "1Q OCTUBRE" o "2Q NOVIEMBRE"

**Fila 2**: Letras de los días de la semana
- Solo se muestran en las columnas de días (a partir de la columna J)
- Muestra la inicial: L (Lunes), M (Martes), X (Miércoles), J (Jueves), V (Viernes), S (Sábado), D (Domingo)

**Fila 3**: Nombres de columnas y números de días

### Columnas Fijas (A-I)

| Columna | Encabezado | Descripción |
|---------|------------|-------------|
| A | LEGAJO | Número de legajo del empleado |
| B | APELLIDO | Apellido en mayúsculas |
| C | NOMBRE | Nombre en mayúsculas |
| D | TIPO CONTRATACION | Tipo de liquidación del empleado |
| E | INGRESO | Fecha de ingreso (formato dd/MM/yyyy) |
| F | CUIL | CUIL del empleado |
| G | AFILIADO | Campo vacío para completar |
| H | DISCIPLINA | Disciplina del empleado (si está asignada) |
| I | CATEGORIA | Categoría del empleado (si está asignada) |

### Columnas de Días (J en adelante)

- **Ubicación**: Comienzan siempre en la columna J
- **Cantidad**: Depende de la quincena seleccionada:
  - **Primera quincena**: Días 1 al 15 (15 columnas)
  - **Segunda quincena**: Días 16 al 31 (15 o 16 columnas según el mes)
- **Formato de encabezado**:
  - Fila 2: Inicial del día de la semana (L, M, X, J, V, S, D)
  - Fila 3: Número del día
- Las celdas están vacías para que el usuario las complete con las horas trabajadas

### Columna Final - TOTAL

- **Ubicación**: Última columna después de los días
- **Encabezado**: "TOTAL"
- **Contenido**: Campo vacío para completar con el total de horas del empleado

## Características del Excel

### Formato y Estilos

- **Fila 1**: Título con la quincena y mes ("1Q OCTUBRE" o "2Q NOVIEMBRE")
- **Fila 2**: Letras de los días de la semana (solo en columnas de días)
- **Fila 3**: Encabezados de columnas fijas y números de días
- **Encabezados**: Con fondo gris, texto en negrita y bordes
- **Datos**: Organizados con bordes en todas las celdas para facilitar la lectura
- **Paneles congelados**: Las primeras 3 columnas (Legajo, Apellido, Nombre) y las 3 filas de encabezado están congeladas para facilitar el desplazamiento

### Anchos de Columna

- Columnas A-F (datos básicos): Anchos ajustados al contenido (10-20 caracteres)
- Columnas G-I (Afiliado, Disciplina, Categoría): Ancho medio (10-15 caracteres)
- Columnas J en adelante (días): Angostas (5 caracteres) para visualizar más días
- Columna TOTAL: Ancho medio (8 caracteres)

### Ordenamiento

Los empleados se muestran ordenados alfabéticamente por:
1. Apellido
2. Nombre

### Filtros

Solo se incluyen empleados:
- Asignados al proyecto seleccionado
- Con estado **Activo**

## Notas Técnicas

### Archivos Creados/Modificados

1. **Servicios**:
   - `Services/IExcelExportService.cs` - Interfaz del servicio
   - `Services/ExcelExportService.cs` - Implementación del servicio de exportación

2. **ViewModels**:
   - `Models/ViewModels/EstadisticasProyectoViewModel.cs` - ViewModel para estadísticas

3. **Controladores**:
   - `Controllers/ProyectosController.cs` - Métodos agregados:
     - `GetEstadisticasProyecto()` - Obtiene estadísticas del proyecto
     - `DescargarExcelEmpleados()` - Genera y descarga el Excel

4. **Servicios de Proyecto**:
   - `Services/IProyectoService.cs` - Método agregado: `GetEstadisticasProyectoAsync()`
   - `Services/ProyectoService.cs` - Implementación de estadísticas

5. **Vistas**:
   - `Views/Proyectos/Details.cshtml` - Modal y funcionalidad JavaScript

6. **Program.cs**:
   - Registro del servicio `IExcelExportService`

### Dependencias

El proyecto ya incluía la librería **ClosedXML** (versión 0.104.2) necesaria para generar archivos Excel.

### Permisos

La funcionalidad está disponible para usuarios con rol:
- **Administrador**
- **Supervisor**

## Ejemplo de Uso

1. Vas a "Proyectos" → "Detalles" del proyecto "Construcción Central"
2. Haces clic en "Exportar Empleados"
3. Seleccionas:
   - Mes: Octubre
   - Año: 2026
   - Quincena: Primera (1-15)
4. Haces clic en "Generar Excel"
5. Se descarga: `Empleados_CONST001_Octubre_2026_Q1.xlsx`

El archivo Excel tendrá:

### Estructura visual:
```
Fila 1: "1Q OCTUBRE" (título centrado y en negrita)

Fila 2:          [vacío hasta columna J]   L  M  X  J  V  S  D  L  M  X  J  V  S  D  L

Fila 3: LEGAJO | APELLIDO | NOMBRE | TIPO CONTRATACION | INGRESO | CUIL | AFILIADO | DISCIPLINA | CATEGORIA | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | TOTAL

Fila 4: 1001   | PEREZ    | JUAN   | MENSUAL          | 01/05/2024 | 20-12345678-9 |  | Eléctrica | Oficial | [campos vacíos para completar] | 
```

- Título: "1Q OCTUBRE"
- Columnas A-I con datos del empleado
- Columnas J-X con los días 1 al 15 (vacías para completar horas)
- Última columna TOTAL (vacía para completar el total de horas)

## Soporte

Para reportar problemas o sugerencias, contacta al equipo de desarrollo.
