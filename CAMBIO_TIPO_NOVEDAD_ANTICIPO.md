# Agregado Tipo de Novedad: Anticipo

## Resumen
Se ha agregado un nuevo tipo de novedad llamado **"Anticipo"** al sistema de gestión de novedades de liquidación.

## Cambios Realizados

### 1. Enum TipoNovedadLiquidacion (Models/Enums/TipoNovedad.cs)
Se agregó el valor `Anticipo = 7` al enum:

```csharp
public enum TipoNovedadLiquidacion
{
    Embargo = 1,
    InicioRoster = 2,
    FinRoster = 3,
    Enfermedad = 4,
    Baja = 5,
    Vacaciones = 6,
    Anticipo = 7  // ← NUEVO
}
```

### 2. Controlador NovedadesController
Se actualizó el método `GetTipoNovedadNombre` para incluir la conversión del nuevo tipo:

```csharp
TipoNovedadLiquidacion.Anticipo => "Anticipo"
```

### 3. Vista Index de Novedades (Views/Novedades/Index.cshtml)
Se agregó el caso en el switch para mostrar el badge del tipo Anticipo con color naranja:

```razor
case TipoNovedadLiquidacion.Anticipo:
    <span class="badge bg-orange">Anticipo</span>
    break;
```

### 4. Vista Create de Novedades (Views/Novedades/Create.cshtml)
Se agregó la opción "Anticipo" en el dropdown de tipo de novedad:

```razor
<option value="@((int)TipoNovedadLiquidacion.Anticipo)">Anticipo</option>
```

### 5. Vista Edit de Novedades (Views/Novedades/Edit.cshtml)
Se agregó la opción "Anticipo" en el dropdown de tipo de novedad:

```razor
<option value="@((int)TipoNovedadLiquidacion.Anticipo)">Anticipo</option>
```

## Características del Tipo Anticipo

### Propósito
El tipo de novedad "Anticipo" permite registrar anticipos de sueldo otorgados a los empleados durante un periodo de liquidación.

### Visualización
- **Badge color**: Naranja (`bg-orange`)
- **Nombre**: "Anticipo"
- **Valor enum**: 7

### Uso
1. Al crear o editar una novedad, ahora está disponible "Anticipo" como opción en el dropdown de "Tipo de Novedad"
2. El anticipo puede incluir un monto que se descontará en la liquidación correspondiente
3. Se puede filtrar por tipo "Anticipo" en la vista principal de novedades

## Campos Relacionados
Cuando se crea una novedad de tipo Anticipo, se pueden completar los siguientes campos:

- **Empleado**: El empleado al que se le otorga el anticipo (requerido)
- **Periodo**: Mes y año de la liquidación (requerido)
- **Quincena**: Primera o segunda quincena (requerido)
- **Fecha de Novedad**: Fecha en que se otorgó el anticipo (requerido)
- **Monto**: Monto del anticipo (opcional, pero recomendado para anticipos)
- **Observaciones**: Notas adicionales sobre el anticipo (opcional)

## Ejemplos de Uso

### Registrar un Anticipo
1. Ir a Novedades → Nueva Novedad
2. Seleccionar el empleado
3. Seleccionar periodo y quincena
4. Elegir "Anticipo" como tipo de novedad
5. Ingresar la fecha y el monto del anticipo
6. Agregar observaciones si es necesario (ej: "Anticipo por razones personales")
7. Guardar

### Filtrar Anticipos
1. Ir a Novedades
2. Expandir el panel de filtros
3. Seleccionar "Anticipo" en el dropdown de "Tipo de Novedad"
4. Opcionalmente filtrar por periodo, proyecto, etc.
5. Hacer clic en "Filtrar"

### Ver Todos los Anticipos de un Periodo
```
Filtros a aplicar:
- Tipo de Novedad: Anticipo
- Periodo: 2026-11 (por ejemplo)
```

## Integración con Liquidación

### Nota Importante
El tipo "Anticipo" está registrado en el sistema de novedades, pero su integración con el cálculo de liquidación debe implementarse en:

- `Services/Liquidacion/Payroll/Steps/StepDeducciones.cs`

Para que los anticipos se descuenten automáticamente, se debe agregar lógica similar a:

```csharp
// Anticipos (XXXX) - Descontar anticipos registrados
var novedadesAnticipo = context.Novedades
    .Where(n => n.TipoNovedad == TipoNovedadLiquidacion.Anticipo)
    .ToList();

foreach (var anticipo in novedadesAnticipo)
{
    if (anticipo.MontoNovedad.HasValue && anticipo.MontoNovedad.Value > 0)
    {
        var concepto = await ObtenerConceptoPorCodigo("XXXX"); // Código de concepto para Anticipo
        if (concepto != null)
        {
            conceptos.Add(CrearConcepto(
                concepto,
                cantidad: 1,
                importe: anticipo.MontoNovedad.Value,
                valorUnitario: anticipo.MontoNovedad.Value,
                observaciones: anticipo.Observaciones
            ));
        }
    }
}
```

## Archivos Modificados

1. `Models/Enums/TipoNovedad.cs` - Agregado valor Anticipo = 7
2. `Controllers/NovedadesController.cs` - Agregado caso en GetTipoNovedadNombre
3. `Views/Novedades/Index.cshtml` - Agregado badge para Anticipo
4. `Views/Novedades/Create.cshtml` - Agregada opción en dropdown
5. `Views/Novedades/Edit.cshtml` - Agregada opción en dropdown

## Estado
✅ **Completado** - El tipo de novedad "Anticipo" está disponible en el sistema y funcionando correctamente.

⚠️ **Pendiente** - Integración con el cálculo automático de liquidación (descuento en recibo).

## Verificación
El proyecto compila correctamente sin errores después de los cambios.
