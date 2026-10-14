# Ejemplos de Uso - Módulo de Liquidación

## Ejemplo Completo: Proceso de Liquidación Quincenal

### Paso 1: Preparar los Datos Maestros

```sql
-- 1. Verificar que existan los catálogos necesarios
SELECT * FROM convenios WHERE codigo = 'UOCRA';
SELECT * FROM tipos_liquidacion WHERE codigo = 'DIRECTOS_CONVERSION_ACUERDO';
SELECT * FROM categorias WHERE codigo = 'OFICIAL_ESP';
SELECT * FROM obras WHERE codigo = 'RCI';

-- 2. Crear empleado de liquidación
INSERT INTO empleados_liquidacion (
    empleado_id, 
    legajo, 
    apellido, 
    nombre, 
    cuil, 
    fecha_alta,
    convenio_id,
    tipo_liquidacion_id,
    categoria_id,
    disciplina_id,
    obra_id,
    valor_hora_basico,
    valor_convenido_diario,
    aplica_plus_altura,
    aplica_plus_fenix,
    activo
) VALUES (
    (SELECT id FROM empleados WHERE legajo = '1207'),
    1207,
    'GOMEZ',
    'CARLOS',
    '20-30123456-7',
    '2024-01-15',
    (SELECT id FROM convenios WHERE codigo = 'UOCRA'),
    (SELECT id FROM tipos_liquidacion WHERE codigo = 'DIRECTOS_CONVERSION_ACUERDO'),
    (SELECT id FROM categorias WHERE codigo = 'OFICIAL_ESP'),
    (SELECT id FROM disciplinas WHERE codigo = 'MONTADOR'),
    (SELECT id FROM obras WHERE codigo = 'RCI'),
    7561.00,          -- Valor hora básico
    150000.00,        -- Neto convenido diario
    TRUE,             -- Aplica plus altura
    TRUE,             -- Aplica plus Fénix
    TRUE              -- Activo
);
```

### Paso 2: Importar Parte Diario desde Excel

```csharp
using GestorNomina.Web.Services.Liquidacion;

public class EjemploImportacionController : Controller
{
    private readonly IExcelParteDiarioService _excelService;
    private readonly ICatalogResolver _catalogResolver;

    public async Task<IActionResult> ImportarTarja(IFormFile archivo)
    {
        // Cargar catálogos en memoria
        if (!_catalogResolver.IsLoaded)
            await _catalogResolver.LoadCatalogsAsync();

        // Importar el archivo Excel
        using var stream = archivo.OpenReadStream();
        var registrosImportados = await _excelService.ImportarParteDiarioAsync(
            stream, 
            periodo: "2026-09", 
            quincena: 1  // Primera quincena
        );

        return Ok(new { 
            mensaje = $"Se importaron {registrosImportados} registros",
            periodo = "2026-09",
            quincena = 1
        });
    }
}
```

### Paso 3: Procesar Liquidación Individual

```csharp
using GestorNomina.Web.Services.Liquidacion.Payroll;

public class EjemploLiquidacionController : Controller
{
    private readonly IPayrollEngine _payrollEngine;

    public async Task<IActionResult> ProcesarEmpleado(int empleadoId)
    {
        try
        {
            // Procesar la liquidación del empleado
            var recibo = await _payrollEngine.ProcesarLiquidacionAsync(
                empleadoId: empleadoId,
                periodo: "2026-09",
                quincena: 1
            );

            // Mostrar resumen del recibo
            return Ok(new
            {
                legajo = recibo.Empleado.Legajo,
                apellido = recibo.Empleado.Apellido,
                nombre = recibo.Empleado.Nombre,
                diasTrabajados = recibo.DiasTrabajados,
                totalBruto = recibo.TotalBruto,
                totalDeducciones = recibo.TotalDeducciones,
                totalNeto = recibo.TotalNeto,
                netoRedondeado = recibo.NetoRedondeado,
                netoConvenidoObjetivo = recibo.NetoConvenidoObjetivo,
                diferenciaControl = recibo.DiferenciaControl,
                conceptos = recibo.Detalles.Select(d => new
                {
                    codigo = d.Concepto.CodigoTango,
                    nombre = d.Concepto.Nombre,
                    unidades = d.Unidades,
                    valorUnitario = d.ValorUnitario,
                    subtotal = d.Subtotal
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

### Paso 4: Procesar Liquidación Masiva

```csharp
public class EjemploLiquidacionMasivaController : Controller
{
    private readonly IPayrollEngine _payrollEngine;

    public async Task<IActionResult> ProcesarQuincena(string periodo, int quincena)
    {
        try
        {
            // Procesar todos los empleados del periodo
            var recibos = await _payrollEngine.ProcesarLiquidacionMasivaAsync(periodo, quincena);

            // Generar resumen
            var resumen = new
            {
                periodo = periodo,
                quincena = quincena,
                totalEmpleados = recibos.Count,
                totalBruto = recibos.Sum(r => r.TotalBruto),
                totalDeducciones = recibos.Sum(r => r.TotalDeducciones),
                totalNeto = recibos.Sum(r => r.TotalNeto),
                empleados = recibos.Select(r => new
                {
                    legajo = r.Empleado.Legajo,
                    apellidoNombre = $"{r.Empleado.Apellido}, {r.Empleado.Nombre}",
                    dias = r.DiasTrabajados,
                    neto = r.NetoRedondeado,
                    diferencia = r.DiferenciaControl
                })
            };

            return Ok(resumen);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

### Paso 5: Exportar a Tango Gestión

```csharp
using GestorNomina.Web.Services.Liquidacion;

public class EjemploExportacionController : Controller
{
    private readonly ITangoExportService _tangoExportService;

    public async Task<IActionResult> ExportarATango(string periodo, int quincena)
    {
        try
        {
            // Generar el archivo de exportación
            var contenido = await _tangoExportService.ExportarPeriodoAsync(periodo, quincena);

            // Generar nombre del archivo
            var nombreArchivo = $"Liquidacion_{periodo}_Q{quincena}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            // Descargar como archivo
            return File(
                System.Text.Encoding.UTF8.GetBytes(contenido),
                "text/plain",
                nombreArchivo
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IActionResult> GuardarExportacion(string periodo, int quincena, string rutaDestino)
    {
        try
        {
            var contenido = await _tangoExportService.ExportarPeriodoAsync(periodo, quincena);
            
            var rutaCompleta = Path.Combine(rutaDestino, $"Liquidacion_{periodo}_Q{quincena}.txt");
            await _tangoExportService.GuardarArchivoAsync(contenido, rutaCompleta);

            return Ok(new { mensaje = $"Archivo guardado en: {rutaCompleta}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

## Ejemplo: Consultas de Análisis

### Consultar Registros del Parte Diario

```sql
-- Ver todos los registros de un empleado en una quincena
SELECT 
    e.legajo,
    e.apellido,
    e.nombre,
    pdr.fecha,
    pdr.horas_diurnas,
    pdr.horas_nocturnas,
    tn.nombre as tipo_novedad
FROM parte_diario_registros pdr
JOIN empleados_liquidacion e ON e.id = pdr.empleado_id
JOIN tipos_novedad tn ON tn.id = pdr.tipo_novedad_id
WHERE e.legajo = 1207
    AND pdr.periodo = '2026-09'
    AND pdr.quincena = 1
ORDER BY pdr.fecha;

-- Totalizar horas por empleado
SELECT 
    e.legajo,
    e.apellido || ', ' || e.nombre as empleado,
    SUM(pdr.horas_diurnas) as total_diurnas,
    SUM(pdr.horas_nocturnas) as total_nocturnas,
    SUM(pdr.horas_diurnas + pdr.horas_nocturnas) as total_horas
FROM parte_diario_registros pdr
JOIN empleados_liquidacion e ON e.id = pdr.empleado_id
WHERE pdr.periodo = '2026-09'
    AND pdr.quincena = 1
GROUP BY e.legajo, e.apellido, e.nombre
ORDER BY e.legajo;
```

### Analizar Recibos Generados

```sql
-- Ver resumen de recibos de una quincena
SELECT 
    r.id as recibo_id,
    e.legajo,
    e.apellido || ', ' || e.nombre as empleado,
    tl.nombre as tipo_liquidacion,
    r.dias_trabajados,
    r.total_bruto,
    r.total_deducciones,
    r.total_neto,
    r.neto_redondeado,
    r.neto_convenido_objetivo,
    r.diferencia_control,
    r.fecha_calculo
FROM recibos r
JOIN empleados_liquidacion e ON e.id = r.empleado_id
JOIN tipos_liquidacion tl ON tl.id = e.tipo_liquidacion_id
WHERE r.periodo = '2026-09'
    AND r.quincena = 1
ORDER BY e.legajo;

-- Ver detalle completo de un recibo
SELECT 
    e.legajo,
    e.apellido || ', ' || e.nombre as empleado,
    cl.codigo_tango,
    cl.nombre as concepto,
    cl.tipo as tipo_concepto,
    rd.unidades,
    rd.valor_unitario,
    rd.subtotal
FROM recibo_detalles rd
JOIN recibos r ON r.id = rd.recibo_id
JOIN empleados_liquidacion e ON e.id = r.empleado_id
JOIN conceptos_liquidacion cl ON cl.id = rd.concepto_id
WHERE e.legajo = 1207
    AND r.periodo = '2026-09'
    AND r.quincena = 1
ORDER BY cl.tipo, cl.codigo_tango;

-- Totalizar por tipo de concepto
SELECT 
    cl.tipo as tipo_concepto,
    COUNT(*) as cantidad_conceptos,
    SUM(rd.subtotal) as total
FROM recibo_detalles rd
JOIN recibos r ON r.id = rd.recibo_id
JOIN conceptos_liquidacion cl ON cl.id = rd.concepto_id
WHERE r.periodo = '2026-09'
    AND r.quincena = 1
GROUP BY cl.tipo
ORDER BY cl.tipo;
```

## Ejemplo: Testing Manual

### Test de Importación

```csharp
[Fact]
public async Task Importar_ParteDiario_DebeCrearRegistrosCorrectamente()
{
    // Arrange
    var rutaArchivo = "TestFiles/Tarja_2026-09_Q1.xlsx";
    var periodo = "2026-09";
    var quincena = 1;

    // Act
    var registrosImportados = await _excelService.ImportarParteDiarioAsync(
        rutaArchivo, periodo, quincena);

    // Assert
    Assert.True(registrosImportados > 0);
    
    var registros = await _context.ParteDiarioRegistros
        .Where(pdr => pdr.Periodo == periodo && pdr.Quincena == quincena)
        .ToListAsync();
    
    Assert.Equal(registrosImportados, registros.Count);
}
```

### Test de Liquidación con Neto Convenido

```csharp
[Fact]
public async Task Liquidar_EmpleadoConNetoConvenido_DebeAjustarCorrectamente()
{
    // Arrange
    var empleadoId = 1; // Empleado con tipo DIRECTOS_CONVERSION_ACUERDO
    var periodo = "2026-09";
    var quincena = 1;

    // Act
    var recibo = await _payrollEngine.ProcesarLiquidacionAsync(
        empleadoId, periodo, quincena);

    // Assert
    Assert.NotNull(recibo.NetoConvenidoObjetivo);
    Assert.True(Math.Abs(recibo.DiferenciaControl ?? 0) < 1); // Diferencia menor a $1
    
    // Verificar que existe la bonificación especial (1050)
    var bonificacion = recibo.Detalles
        .FirstOrDefault(d => d.Concepto.CodigoTango == "1050");
    
    if (recibo.NetoConvenidoObjetivo > 0)
        Assert.NotNull(bonificacion);
}
```

## Ejemplo: Script de Migración de Empleados

```sql
-- Script para migrar empleados existentes a empleados_liquidacion
INSERT INTO empleados_liquidacion (
    empleado_id,
    legajo,
    apellido,
    nombre,
    cuil,
    fecha_alta,
    convenio_id,
    tipo_liquidacion_id,
    categoria_id,
    disciplina_id,
    obra_id,
    valor_hora_basico,
    valor_convenido_diario,
    aplica_plus_altura,
    aplica_plus_fenix,
    activo
)
SELECT 
    e.id,
    e.legajo::integer,
    e.apellido,
    e.nombre,
    COALESCE(e.documento_identidad, '00-00000000-0'),
    COALESCE(e.fecha_ingreso, CURRENT_DATE),
    (SELECT id FROM convenios WHERE codigo = 'UOCRA' LIMIT 1),
    (SELECT id FROM tipos_liquidacion WHERE codigo = 'DIRECTOS' LIMIT 1),
    (SELECT id FROM categorias WHERE codigo = 'OFICIAL' LIMIT 1),
    NULL,
    (SELECT id FROM obras WHERE codigo = 'RCI' LIMIT 1),
    7000.00,  -- Valor hora por defecto, ajustar manualmente
    NULL,     -- Sin neto convenido por defecto
    FALSE,    -- Sin plus altura por defecto
    FALSE,    -- Sin plus Fénix por defecto
    e.activo
FROM empleados e
WHERE e.activo = TRUE
    AND NOT EXISTS (
        SELECT 1 FROM empleados_liquidacion el 
        WHERE el.empleado_id = e.id
    );

-- Actualizar valores específicos manualmente después de la migración
UPDATE empleados_liquidacion
SET valor_hora_basico = 7561.00,
    valor_convenido_diario = 150000.00,
    tipo_liquidacion_id = (SELECT id FROM tipos_liquidacion WHERE codigo = 'DIRECTOS_CONVERSION_ACUERDO'),
    aplica_plus_altura = TRUE,
    aplica_plus_fenix = TRUE
WHERE legajo = 1207;
```

## Ejemplo: Dashboard de Liquidación

```csharp
public class DashboardLiquidacionViewModel
{
    public string Periodo { get; set; }
    public int Quincena { get; set; }
    public int TotalEmpleados { get; set; }
    public int EmpleadosLiquidados { get; set; }
    public int EmpleadosPendientes { get; set; }
    public decimal TotalBruto { get; set; }
    public decimal TotalDeducciones { get; set; }
    public decimal TotalNeto { get; set; }
    public List<EmpleadoLiquidadoDto> Empleados { get; set; }
}

public async Task<DashboardLiquidacionViewModel> ObtenerDashboard(string periodo, int quincena)
{
    var recibos = await _context.Recibos
        .Include(r => r.Empleado)
        .Where(r => r.Periodo == periodo && r.Quincena == quincena)
        .ToListAsync();

    var empleadosConRegistros = await _context.ParteDiarioRegistros
        .Where(pdr => pdr.Periodo == periodo && pdr.Quincena == quincena)
        .Select(pdr => pdr.EmpleadoId)
        .Distinct()
        .CountAsync();

    return new DashboardLiquidacionViewModel
    {
        Periodo = periodo,
        Quincena = quincena,
        TotalEmpleados = empleadosConRegistros,
        EmpleadosLiquidados = recibos.Count,
        EmpleadosPendientes = empleadosConRegistros - recibos.Count,
        TotalBruto = recibos.Sum(r => r.TotalBruto),
        TotalDeducciones = recibos.Sum(r => r.TotalDeducciones),
        TotalNeto = recibos.Sum(r => r.TotalNeto),
        Empleados = recibos.Select(r => new EmpleadoLiquidadoDto
        {
            Legajo = r.Empleado.Legajo,
            ApellidoNombre = $"{r.Empleado.Apellido}, {r.Empleado.Nombre}",
            DiasTrabajados = r.DiasTrabajados,
            NetoRedondeado = r.NetoRedondeado,
            DiferenciaControl = r.DiferenciaControl
        }).ToList()
    };
}
```

Este conjunto de ejemplos cubre los casos de uso más comunes del módulo de liquidación.
