# Módulo de Liquidación de Haberes

## Descripción General

El módulo de liquidación de haberes implementa el proceso completo de importación, consolidación diaria y motor de liquidación basado en la lógica de la planilla "Tarja de Control de Horas / Tango".

## Características Principales

- **Importación de Parte Diario desde Excel**: Parseo automático de planillas quincenales con ClosedXML
- **Motor de Liquidación con Pipeline**: Cálculo automático de conceptos remunerativos, deducciones y ajuste por Neto Convenido (Grossing-Up)
- **Exportación a Tango Gestión**: Generación de archivos de texto para importación directa

## Arquitectura del Sistema

### 1. Modelo de Datos

#### Catálogos Maestros (Lookup Tables)
- `convenios`: Convenios colectivos (UOCRA, AOMA, MENSUAL)
- `tipos_liquidacion`: Tipos de liquidación (DIRECTOS, DIRECTOS_CONVERSION_ACUERDO, INDIRECTO)
- `categorias`: Categorías profesionales (OFICIAL ESPECIALIZADO, SUPERVISOR, etc.)
- `disciplinas`: Especialidades (OPERADOR, MECANICO, MONTADOR, etc.)
- `obras`: Catálogo de obras/proyectos
- `tipos_novedad`: Tipos de novedades del parte diario (NORMAL, FERIADO, ENFERMEDAD, etc.)
- `conceptos_liquidacion`: Conceptos de liquidación para Tango

#### Entidades Transaccionales
- `empleados_liquidacion`: Datos de empleados con información de liquidación
- `parte_diario_registros`: Registros diarios de horas y novedades
- `recibos`: Recibos de liquidación generados
- `recibo_detalles`: Detalle de conceptos de cada recibo

### 2. Servicios Principales

#### ICatalogResolver / CatalogResolver
Servicio que mantiene en memoria los catálogos para optimizar el rendimiento durante el cálculo masivo.

```csharp
await _catalogResolver.LoadCatalogsAsync();
var tipoNovedadId = _catalogResolver.GetTipoNovedadId(CodigoTipoNovedad.ENFERMEDAD);
```

#### IExcelParteDiarioService / ExcelParteDiarioService
Servicio para importar el parte diario desde archivos Excel.

**Estructura esperada del Excel:**
- **Fila 13**: Encabezados
- **Columna D**: Legajo
- **Columna F**: CUIL
- **Columnas P-AE**: Horas diurnas (días 1-15 o 16-31)
- **Columnas AG-AT**: Horas nocturnas

**Mapeo de Novedades:**
- Valores numéricos → Horas trabajadas
- Texto: "E"/"ENF" → ENFERMEDAD, "V"/"VAC" → VACACIONES, "FER" → FERIADO
- Colores: Rojo → FALTA_INJUSTIFICADA, Amarillo → ENFERMEDAD, Verde/Azul → ROSTER

```csharp
var registrosImportados = await _excelService.ImportarParteDiarioAsync(stream, "2026-09", 1);
```

#### IPayrollEngine / PayrollEngine
Motor de liquidación implementado con patrón Pipeline.

**Pipeline de Pasos:**

1. **StepAgregadorHoras**: Analiza registros y calcula totalizadores
   - HSNOR: Horas normales (lunes-viernes hasta 8hs, sábados hasta 4hs)
   - HSEXT: Horas extras 50%
   - HS100: Horas extras 100% (domingos, feriados, exceso de sábados)
   - HSNOCT, HSNOCT50, HSNOCT100: Horas nocturnas equivalentes
   - DIAS: Días trabajados (total horas / 11)

2. **StepConceptosRemunerativos**: Calcula conceptos remunerativos
   - Sueldo Básico (1001): HSNOR × ValorHora
   - Plus Altura 15% (1010): SueldoBasico × 0.15 (si aplica)
   - Horas Extras 50% (1002): HSEXT × ValorHora × 1.5 × 1.45
   - Horas Extras 100% (1003): HS100 × ValorHora × 2.0 × 1.45
   - Horas Nocturnas (1004): HSNOCT × ValorHora × 1.13
   - Presentismo 20% (1020): (SueldoBasico + Nocturnas) × 0.20 (si no hay faltas)
   - Adicional Fénix 25% (1025): (SueldoBasico + Nocturnas) × 0.25 (si aplica)
   - Feriados/Licencias (1030): DiasFeriado × (ValorHora × 8)

3. **StepNetoConvenido**: Motor de Grossing-Up
   - Aplica solo para tipo "DIRECTOS_CONVERSION_ACUERDO"
   - NetoObjetivo = ValorConvenidoDiario × DIAS
   - NetoProvisorio = BrutoParcial × (1 - 0.195)
   - AjusteBruto = (NetoObjetivo - NetoProvisorio) × 1.24223572358584
   - Agrega bonificación especial (1050) si es necesario

4. **StepDeducciones**: Retenciones de ley
   - Jubilación 11% (5001)
   - Ley 19.032 3% (5002)
   - Obra Social 3% (5003) con tope mínimo UOCRA
   - Cuota Sindical 2.5% (5004)
   - Seguro de Vida (5010): $19,989.90 fijo

5. **StepCierreRecibo**: Cierre y control
   - TotalBruto = Remunerativos + NoRemunerativos
   - TotalNeto = TotalBruto - Deducciones
   - DiferenciaControl = NetoObjetivo - TotalNeto (debe ser ≈ 0)
   - NetoRedondeado = Math.Ceiling(TotalNeto)

```csharp
// Procesar un empleado
var recibo = await _payrollEngine.ProcesarLiquidacionAsync(empleadoId, "2026-09", 1);

// Procesar todos los empleados del periodo
var recibos = await _payrollEngine.ProcesarLiquidacionMasivaAsync("2026-09", 1);
```

#### ITangoExportService / TangoExportService
Servicio para exportar recibos al formato de Tango Gestión.

**Formato de salida:**
```
[Legajo];[CodigoConcepto];[Unidades];[ImporteCalculado]
```

**Ejemplo:**
```
1207;1001;24.00;181464.00
1207;1020;1.00;36292.80
1207;5001;11.00;90272.65
```

```csharp
// Exportar un periodo completo
var contenido = await _tangoExportService.ExportarPeriodoAsync("2026-09", 1);
await _tangoExportService.GuardarArchivoAsync(contenido, "Liquidacion_2026-09_Q1.txt");
```

## Instalación y Configuración

### 1. Instalar Paquetes NuGet

El paquete ClosedXML ya está agregado en el .csproj:

```xml
<PackageReference Include="ClosedXML" Version="0.104.2" />
```

### 2. Aplicar Migraciones

```bash
cd GestorNomina.Web
dotnet ef database update
```

### 3. Cargar Datos Iniciales (Seed Data)

Ejecutar el script SQL de seed data:

```bash
# Desde PostgreSQL
psql -U usuario -d gestornomina -f Data/SeedDataLiquidacion.sql
```

O ejecutar manualmente el contenido del archivo `SeedDataLiquidacion.sql`.

### 4. Configurar Empleados de Liquidación

Crear registros en la tabla `empleados_liquidacion` para cada empleado que participará en el proceso de liquidación:

```sql
INSERT INTO empleados_liquidacion (
    empleado_id, legajo, apellido, nombre, cuil, fecha_alta,
    convenio_id, tipo_liquidacion_id, categoria_id, disciplina_id, obra_id,
    valor_hora_basico, valor_convenido_diario,
    aplica_plus_altura, aplica_plus_fenix, activo
) VALUES (
    1, 1207, 'PEREZ', 'JUAN', '20-12345678-9', '2024-01-01',
    (SELECT id FROM convenios WHERE codigo = 'UOCRA'),
    (SELECT id FROM tipos_liquidacion WHERE codigo = 'DIRECTOS_CONVERSION_ACUERDO'),
    (SELECT id FROM categorias WHERE codigo = 'OFICIAL_ESP'),
    (SELECT id FROM disciplinas WHERE codigo = 'MONTADOR'),
    (SELECT id FROM obras WHERE codigo = 'RCI'),
    7561.00, 150000.00, TRUE, TRUE, TRUE
);
```

## Uso del Sistema

### Flujo Completo de Liquidación

#### 1. Importar Parte Diario

```csharp
// Desde el controlador o servicio
await _excelService.ImportarParteDiarioAsync(archivoStream, "2026-09", 1);
```

O usando la API REST:

```http
POST /Liquidacion/ImportarParteDiario
Content-Type: multipart/form-data

archivo: [archivo.xlsx]
periodo: 2026-09
quincena: 1
```

#### 2. Procesar Liquidación

```csharp
// Procesar todos los empleados
var recibos = await _payrollEngine.ProcesarLiquidacionMasivaAsync("2026-09", 1);
```

O usando la API REST:

```http
POST /Liquidacion/ProcesarLiquidacion
Content-Type: application/x-www-form-urlencoded

periodo=2026-09&quincena=1
```

#### 3. Exportar a Tango

```csharp
var contenido = await _tangoExportService.ExportarPeriodoAsync("2026-09", 1);
await _tangoExportService.GuardarArchivoAsync(contenido, "Liquidacion_2026-09_Q1.txt");
```

O usando la API REST:

```http
GET /Liquidacion/ExportarTango?periodo=2026-09&quincena=1
```

## Enums Fuertemente Tipados

El sistema utiliza enums para eliminar strings mágicos:

```csharp
// CodigoTipoNovedad
NORMAL, ENFERMEDAD, FALTA_INJUSTIFICADA, VACACIONES, FERIADO, ACCIDENTE, ROSTER

// CodigoTipoLiquidacion
DIRECTOS, DIRECTOS_CONVERSION_ACUERDO, INDIRECTO

// CodigoConceptoTango
HorasNormales = 1001
HorasExtras50 = 1002
HorasExtras100 = 1003
// ... etc
```

## Extensibilidad

### Agregar un Nuevo Paso al Pipeline

1. Crear una clase que implemente `IPayrollStep`:

```csharp
public class StepCustom : IPayrollStep
{
    public string StepName => "Mi Paso Personalizado";
    
    public Task ExecuteAsync(PayrollContext context)
    {
        // Lógica personalizada
        return Task.CompletedTask;
    }
}
```

2. Agregarlo al pipeline en `PayrollEngine`:

```csharp
_steps = new List<IPayrollStep>
{
    new StepAgregadorHoras(catalogResolver),
    new StepConceptosRemunerativos(),
    new StepCustom(), // <-- Nuevo paso
    new StepNetoConvenido(catalogResolver),
    new StepDeducciones(),
    new StepCierreRecibo()
};
```

### Agregar Nuevos Conceptos de Liquidación

1. Agregar el código al enum `CodigoConceptoTango`
2. Insertar el registro en la tabla `conceptos_liquidacion`
3. Agregar la lógica de cálculo en el paso correspondiente del pipeline

## Consideraciones de Rendimiento

- **ICatalogResolver**: Carga los catálogos en memoria una vez por request (Scoped)
- **Liquidación Masiva**: Procesa empleados secuencialmente con manejo de errores individual
- **Transacciones**: Cada recibo se guarda en una transacción independiente

## Seguridad

- Todas las operaciones de liquidación requieren rol de **Administrador** (`[AdminOnly]`)
- Los datos sensibles (CUIL, salarios) están protegidos por autenticación
- Las importaciones validan la estructura del archivo antes de procesar

## Testing

### Ejemplo de Test Unitario

```csharp
[Fact]
public async Task StepAgregadorHoras_DebeCalcularHorasCorrectamente()
{
    // Arrange
    var context = new PayrollContext
    {
        Registros = new List<ParteDiarioRegistro>
        {
            new() { Fecha = new DateTime(2026, 9, 1), HorasDiurnas = 8, HorasNocturnas = 0 },
            new() { Fecha = new DateTime(2026, 9, 2), HorasDiurnas = 10, HorasNocturnas = 0 }
        }
    };
    
    var step = new StepAgregadorHoras(catalogResolver);
    
    // Act
    await step.ExecuteAsync(context);
    
    // Assert
    Assert.Equal(16, context.HSNOR);
    Assert.Equal(2, context.HSEXT);
}
```

## Troubleshooting

### Error: "Los catálogos no han sido cargados"
**Solución**: Llamar a `await _catalogResolver.LoadCatalogsAsync()` antes de usar el resolver.

### Error: "No hay registros de parte diario"
**Solución**: Verificar que se haya importado el parte diario para el periodo y quincena especificados.

### La diferencia de control es muy alta
**Solución**: Verificar que el coeficiente de grossing-up (1.24223572358584) y las tasas de retención (19.5%) sean correctas para la normativa vigente.

## Contacto y Soporte

Para consultas sobre el módulo de liquidación, contactar al equipo de desarrollo.
