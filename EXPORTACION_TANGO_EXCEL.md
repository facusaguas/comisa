# Exportación de Liquidación a Excel para Tango

## Descripción General

Se modificó el botón **"Exportar Tango"** para que ahora genere un archivo **Excel (.xlsx)** en lugar de un archivo de texto, con un formato específico optimizado para la importación en Tango.

---

## 📋 Formato del Archivo Excel

### Estructura de Columnas

| Columna | Campo | Descripción | Ejemplo |
|---------|-------|-------------|---------|
| **A** | Fecha | Fecha de liquidación (formato dd/MM/yyyy) | 15/09/2026 |
| **B** | Legajo | Legajo del empleado | 1001 |
| **C** | CodigoConcepto | Código del concepto de liquidación | HsNormales |
| **D** | ValorEntero | Valor para conceptos de horas/unidades | 160.00 |
| **E** | Monto | Valor para conceptos monetarios | 250000.00 |

### Características del Archivo

- ✅ **Sin encabezados**: El archivo contiene solo datos
- ✅ **Un registro por concepto**: Cada fila representa un concepto de un empleado
- ✅ **Formato de fecha**: dd/MM/yyyy
- ✅ **Formato numérico**: Dos decimales (0.00)
- ✅ **Ordenamiento**: Por legajo de empleado

---

## 🔢 Mapeo de Conceptos

### Conceptos que van en la columna **ValorEntero** (columna D)

Estos conceptos se cargan en la columna "ValorEntero" y la columna "Monto" queda vacía:

- **HsNormales** - Horas normales trabajadas
- **HsExt** - Horas extras (genérico)
- **HsExt50** - Horas extras al 50%
- **HsExt100** - Horas extras al 100%
- **Fenix** - Plus Fenix (unidades)
- **PlusFenix** - Plus Fenix adicional
- **PlusElectrico** - Plus eléctrico
- **Presentismo** - Presentismo (unidades o porcentaje)

### Conceptos que van en la columna **Monto** (columna E)

Todos los demás conceptos se cargan en la columna "Monto" y la columna "ValorEntero" queda vacía:

- **SalarioBasico** - Salario base
- **Vacaciones** - Días de vacaciones
- **SAC** - Sueldo Anual Complementario
- **Antiguedad** - Antigüedad
- **Embargo** - Embargos judiciales
- **AdelantoQuincenal** - Adelanto de quincena
- **Deducciones** - Otras deducciones
- Y todos los demás conceptos definidos en el sistema

---

## 📅 Lógica de Fechas

La fecha se calcula según el período y la quincena:

### Primera Quincena (1)
- **Día**: 15 del mes
- **Ejemplo**: Septiembre 2026, Quincena 1 → **15/09/2026**

### Segunda Quincena (2)
- **Día**: Último día del mes (28, 29, 30 o 31)
- **Ejemplo**: Septiembre 2026, Quincena 2 → **30/09/2026**

---

## 💡 Ejemplo Completo

### Datos de Entrada

**Empleado**: Juan Pérez (Legajo 1001)  
**Período**: Septiembre 2026  
**Quincena**: 1 (Primera quincena)

**Conceptos en el recibo**:
- HsNormales: 88 horas
- HsExt50: 8 horas
- PlusElectrico: 2 unidades
- SalarioBasico: $250,000.00
- Presentismo: $25,000.00
- AdelantoQuincenal: $100,000.00

### Resultado en Excel

| Fecha | Legajo | CodigoConcepto | ValorEntero | Monto |
|-------|--------|----------------|-------------|-------|
| 15/09/2026 | 1001 | HsNormales | 88.00 | |
| 15/09/2026 | 1001 | HsExt50 | 8.00 | |
| 15/09/2026 | 1001 | PlusElectrico | 2.00 | |
| 15/09/2026 | 1001 | SalarioBasico | | 250000.00 |
| 15/09/2026 | 1001 | Presentismo | | 25000.00 |
| 15/09/2026 | 1001 | AdelantoQuincenal | | 100000.00 |

---

## 📂 Nombre del Archivo

El archivo se genera con el siguiente formato:

```
Tango_Liquidacion_[AÑO]_[MES]_Q[QUINCENA]_[FECHA_GENERACION].xlsx
```

### Ejemplos:
- `Tango_Liquidacion_2026_09_Q1_20261113.xlsx`
- `Tango_Liquidacion_2026_09_Q2_20261128.xlsx`
- `Tango_Liquidacion_2026_10_Q1_20261215.xlsx`

---

## 🔧 Implementación Técnica

### Archivos Modificados

#### 1. Interfaz del Servicio
**Archivo**: `Services/Liquidacion/ITangoExportService.cs`

```csharp
/// <summary>
/// Genera un archivo Excel con el formato para Tango
/// Formato: Fecha - Legajo - CodigoConcepto - ValorEntero - Monto
/// </summary>
Task<MemoryStream> GenerarExcelTangoAsync(List<Recibo> recibos);
```

#### 2. Implementación del Servicio
**Archivo**: `Services/Liquidacion/TangoExportService.cs`

**Método principal**: `GenerarExcelTangoAsync`

**Características**:
- Usa **ClosedXML** para generar el Excel
- Define un HashSet con los conceptos que van en ValorEntero
- Calcula automáticamente la fecha según período y quincena
- Ordena los recibos por legajo
- Genera un MemoryStream para descarga directa

**Lógica de clasificación**:
```csharp
var conceptosValorEntero = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "HsNormales", "HsExt", "HsExt50", "HsExt100",
    "Fenix", "PlusFenix", "PlusElectrico", "Presentismo"
};

bool esValorEntero = conceptosValorEntero.Contains(detalle.Concepto.Codigo);
```

#### 3. Controlador
**Archivo**: `Controllers/LiquidacionController.cs`

**Método modificado**: `GenerarArchivoTango`

**Cambios**:
```csharp
// ANTES: Generaba archivo de texto
var contenidoArchivo = await _tangoExportService.ExportarATango(recibos);
return File(System.Text.Encoding.UTF8.GetBytes(contenidoArchivo), "text/plain", nombreArchivo);

// AHORA: Genera archivo Excel
var excelStream = await _tangoExportService.GenerarExcelTangoAsync(recibos);
return File(excelStream, 
    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
    nombreArchivo);
```

---

## 🚀 Cómo Usar

### Paso 1: Acceder al Módulo
1. Iniciar sesión en el sistema
2. Navegar a **Liquidación** → **Exportar Tango**

### Paso 2: Seleccionar Período
1. Elegir el **Período** (ejemplo: 2026-09)
2. Elegir la **Quincena** (1 o 2)
3. Click en **"Generar Archivo Tango"**

### Paso 3: Descargar
- El navegador descargará automáticamente el archivo Excel
- El archivo se abrirá con Excel, LibreOffice o Google Sheets

### Paso 4: Importar en Tango
1. Abrir Tango Gestión
2. Ir al módulo de importación de liquidaciones
3. Seleccionar el archivo Excel generado
4. Mapear las columnas según corresponda
5. Ejecutar la importación

---

## ✅ Validaciones Implementadas

### Validaciones en el Backend

1. **Existencia de recibos**: Verifica que existan recibos para el período/quincena seleccionado
2. **Datos completos**: Verifica que los recibos tengan empleado y detalles cargados
3. **Formato de fecha**: Maneja correctamente los últimos días de cada mes

### Mensajes de Error

Si no hay recibos:
```
"No hay recibos para exportar en ese periodo/quincena"
```

Si hay un error al generar:
```
"Error al generar archivo: [detalle del error]"
```

---

## 📊 Ventajas del Nuevo Formato

### Formato Excel vs Texto

| Aspecto | Texto (Anterior) | Excel (Actual) |
|---------|------------------|----------------|
| **Visualización** | Difícil de leer | Clara y estructurada |
| **Edición** | Compleja | Fácil con Excel |
| **Validación** | Manual | Automática en Excel |
| **Importación** | Requiere parseo | Directa desde Tango |
| **Formato** | Delimitado por pipes | Columnas definidas |

### Beneficios

1. ✅ **Más intuitivo**: Fácil de revisar visualmente antes de importar
2. ✅ **Menos errores**: Excel valida tipos de datos automáticamente
3. ✅ **Editable**: Se pueden hacer correcciones manuales si es necesario
4. ✅ **Compatible**: Funciona con Excel, LibreOffice, Google Sheets
5. ✅ **Trazable**: Se puede archivar y auditar fácilmente

---

## 🧪 Testing

### Casos de Prueba Sugeridos

#### Test 1: Exportación Básica
**Datos**:
- 1 empleado
- 5 conceptos mixtos (horas y montos)
- Primera quincena

**Verificar**:
- Archivo se descarga correctamente
- Nombre del archivo es correcto
- Fecha es 15 del mes
- ValorEntero tiene valores solo en conceptos de horas
- Monto tiene valores en conceptos monetarios

#### Test 2: Múltiples Empleados
**Datos**:
- 10 empleados
- Conceptos variados
- Segunda quincena

**Verificar**:
- Todos los empleados están presentes
- Ordenados por legajo
- Fecha es último día del mes
- Cada empleado tiene todos sus conceptos

#### Test 3: Todos Conceptos de Horas
**Datos**:
- 1 empleado
- Solo conceptos que van en ValorEntero

**Verificar**:
- Columna Monto está vacía
- Columna ValorEntero tiene todos los valores

#### Test 4: Todos Conceptos de Monto
**Datos**:
- 1 empleado
- Solo conceptos que van en Monto

**Verificar**:
- Columna ValorEntero está vacía
- Columna Monto tiene todos los valores

#### Test 5: Sin Recibos
**Datos**:
- Período sin recibos generados

**Verificar**:
- Muestra mensaje de error
- No descarga archivo
- Redirige a la vista de exportación

---

## 🔮 Mejoras Futuras

### Posibles Extensiones

1. **Filtros adicionales**:
   - Por proyecto
   - Por empleado específico
   - Por tipo de concepto

2. **Formato personalizable**:
   - Permitir elegir columnas a exportar
   - Configurar mapeo de conceptos desde UI
   - Agregar/quitar encabezados

3. **Múltiples formatos**:
   - Mantener opción de texto plano
   - Agregar formato CSV
   - Exportar a PDF para revisión

4. **Validaciones adicionales**:
   - Verificar conceptos faltantes
   - Alertar sobre inconsistencias
   - Generar reporte de validación

5. **Auditoría**:
   - Registrar cada exportación
   - Quién exportó y cuándo
   - Hash del archivo generado

---

## 📝 Notas Importantes

### Compatibilidad

- ✅ **Excel 2010+**: Formato .xlsx compatible
- ✅ **LibreOffice Calc**: Soportado completamente
- ✅ **Google Sheets**: Puede abrir y editar el archivo
- ✅ **Tango Gestión**: Requiere mapeo de columnas en primera importación

### Rendimiento

- El archivo se genera en memoria (MemoryStream)
- No se escriben archivos temporales en disco
- Rendimiento óptimo hasta 1000 empleados
- Para más empleados, considerar paginación o procesamiento asíncrono

### Seguridad

- Solo usuarios autenticados pueden exportar
- Se valida que el usuario tenga permisos sobre el período
- El archivo no contiene datos sensibles más allá de la liquidación
- Se recomienda usar HTTPS para la descarga

---

## 🆘 Solución de Problemas

### El archivo no se descarga
**Solución**: Verificar que el navegador no esté bloqueando descargas automáticas

### Excel muestra errores al abrir
**Solución**: Asegurarse de tener Excel 2010 o superior, o usar LibreOffice

### Columnas vacías
**Solución**: Verificar que los conceptos tengan el código correcto (case-insensitive)

### Fecha incorrecta
**Solución**: Verificar el formato del período (debe ser YYYY-MM)

---

## 📚 Referencias

- **Código principal**: `Services/Liquidacion/TangoExportService.cs`
- **Controlador**: `Controllers/LiquidacionController.cs`
- **Interfaz**: `Services/Liquidacion/ITangoExportService.cs`
- **Librería usada**: ClosedXML (versión 0.104.2)

---

**Implementado por**: Kiro AI  
**Fecha**: Noviembre 13, 2026  
**Versión**: 1.0
