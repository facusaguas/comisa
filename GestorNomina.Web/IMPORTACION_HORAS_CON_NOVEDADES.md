# Importación de Horas con Detección Automática de Novedades

## Descripción General

El sistema de importación de horas desde archivos Excel ahora incluye la capacidad de detectar automáticamente novedades basadas en letras presentes en las celdas de horas. Cuando una celda contiene un valor numérico seguido de una letra específica, el sistema:

1. **Extrae las horas** del valor numérico
2. **Detecta la letra** asociada
3. **Crea automáticamente una novedad** para ese empleado en esa fecha

## Mapeo de Letras a Novedades

| Letra | Tipo de Novedad | Descripción |
|-------|----------------|-------------|
| **I** | Inicio de Roster | Indica el inicio de un período de roster para el empleado |
| **F** | Fin de Roster | Indica el fin de un período de roster para el empleado |
| **E** | Enfermedad | Marca un día de ausencia por enfermedad |
| **V** | Vacaciones | Indica un día de vacaciones |
| **B** | Baja | Marca una baja del empleado |

## Ejemplos de Uso

### Ejemplo 1: Horas con Inicio de Roster
```
Celda Excel: 8I
Resultado:
- Se cargan 8 horas normales para ese día
- Se crea una novedad "Inicio de Roster" para ese empleado en esa fecha
```

### Ejemplo 2: Horas con Vacaciones
```
Celda Excel: 8V
Resultado:
- Se cargan 8 horas para ese día
- Se crea una novedad "Vacaciones" para ese empleado en esa fecha
```

### Ejemplo 3: Horas con Enfermedad
```
Celda Excel: 4E
Resultado:
- Se cargan 4 horas para ese día
- Se crea una novedad "Enfermedad" para ese empleado en esa fecha
```

### Ejemplo 4: Múltiples Novedades en Distintos Días
```
Día 1: 8I  → 8 horas + Inicio de Roster
Día 2: 8   → 8 horas (sin novedad)
Día 3: 8   → 8 horas (sin novedad)
Día 4: 8F  → 8 horas + Fin de Roster
Día 5: 8V  → 8 horas + Vacaciones
```

## Características Técnicas

### Detección Automática
- El sistema detecta automáticamente si la celda contiene texto
- Verifica si el último carácter es una letra válida (I, F, E, V, B)
- La letra puede estar en mayúscula o minúscula (se normaliza a mayúscula)
- Extrae solo la parte numérica para registrar las horas

### Prevención de Duplicados
- El sistema verifica si ya existe una novedad para el mismo empleado, fecha y tipo
- Si ya existe, no crea una novedad duplicada
- Esto permite reimportar archivos sin generar registros redundantes

### Observaciones Automáticas
Cada novedad creada automáticamente incluye:
- **Observación**: "Importado desde Excel - Fila X"
- Donde X es el número de fila del Excel de origen
- Esto facilita la trazabilidad y auditoría

## Información Almacenada en las Novedades

Cada novedad creada contiene:

| Campo | Descripción | Ejemplo |
|-------|-------------|---------|
| EmpleadoId | ID del empleado | 123 |
| Periodo | Primer día del mes/año | 2026-09-01 |
| Quincena | Quincena (1 o 2) | 1 |
| TipoNovedad | Tipo según la letra | InicioRoster |
| FechaNovedad | Fecha específica del día | 2026-09-15 |
| Observaciones | Origen de la importación | "Importado desde Excel - Fila 14" |

## Integración con el Sistema

### Base de Datos
Las novedades se almacenan en la tabla `novedades` con los siguientes tipos disponibles:

```csharp
public enum TipoNovedadLiquidacion
{
    Embargo = 1,
    InicioRoster = 2,
    FinRoster = 3,
    Enfermedad = 4,
    Baja = 5,
    Vacaciones = 6
}
```

### Servicio de Importación
La lógica está implementada en:
- **Archivo**: `Services/Liquidacion/ExcelParteDiarioService.cs`
- **Método principal**: `ImportarParteDiarioAsync`
- **Método auxiliar**: `MapearLetraATipoNovedad`

## Validaciones y Reglas

### Letras Válidas
Solo se reconocen las siguientes letras: **I, F, E, V, B**

Cualquier otra letra será ignorada y la celda se procesará como texto normal.

### Formato de Celda
El formato esperado es: `[número][letra]`

Ejemplos válidos:
- `8I`
- `8.5F`
- `10E`
- `4V`
- `8B`

Ejemplos que no generan novedades:
- `8X` (letra no reconocida)
- `I8` (letra al inicio, no al final)
- `8 I` (espacio entre número y letra - depende del comportamiento del Excel)

### Horas con Valor Cero
Si después de extraer la parte numérica el valor es 0 o negativo, el registro completo se omite (ni horas ni novedad).

## Casos Especiales

### Misma Novedad en Múltiples Días
Si un empleado tiene la misma letra en varios días (por ejemplo, varios días de vacaciones), se crea una novedad independiente por cada día:

```
Día 10: 8V → Novedad de Vacaciones para el 10/09/2026
Día 11: 8V → Novedad de Vacaciones para el 11/09/2026
Día 12: 8V → Novedad de Vacaciones para el 12/09/2026
```

### Reimportación de Archivos
Si se reimporta el mismo período:
- Las horas se pueden duplicar (depende de la lógica de períodos de carga)
- Las novedades NO se duplican gracias a la validación de existencia

## Visualización y Gestión

Las novedades creadas automáticamente pueden:
- Visualizarse en el módulo de Novedades
- Editarse si es necesario
- Eliminarse manualmente si fueron creadas por error
- Filtrarse por tipo, empleado, período, etc.

## Auditoría

El campo `Observaciones` permite rastrear:
- Que la novedad fue creada automáticamente desde Excel
- La fila exacta del archivo de origen
- Facilita la corrección de errores de importación

## Ejemplo Completo de Escenario Real

### Archivo Excel - Septiembre 2026, Primera Quincena

| Legajo | Nombre | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 |
|--------|--------|---|---|---|---|---|---|---|---|---|----|----|----|----|----|----|
| 1001 | Juan Pérez | 8 | 8I | 8 | 8 | 8F | 0 | 0 | 8 | 8 | 8 | 8 | 8 | 0 | 0 | 8V |

### Resultado de la Importación

**Horas cargadas**: 15 registros de horas (excluyendo los días con 0 horas)

**Novedades creadas**: 3 novedades
1. Día 2 (02/09/2026): Inicio de Roster
2. Día 5 (05/09/2026): Fin de Roster
3. Día 15 (15/09/2026): Vacaciones

### Base de Datos - Tabla `novedades`

| id | empleado_id | periodo | quincena | tipo_novedad | fecha_novedad | observaciones |
|----|-------------|---------|----------|--------------|---------------|---------------|
| 1 | 123 | 2026-09-01 | 1 | 2 (InicioRoster) | 2026-09-02 | Importado desde Excel - Fila 14 |
| 2 | 123 | 2026-09-01 | 1 | 3 (FinRoster) | 2026-09-05 | Importado desde Excel - Fila 14 |
| 3 | 123 | 2026-09-01 | 1 | 6 (Vacaciones) | 2026-09-15 | Importado desde Excel - Fila 14 |

## Beneficios

1. **Automatización**: Reduce el trabajo manual de crear novedades una por una
2. **Trazabilidad**: Cada novedad queda registrada con su origen
3. **Flexibilidad**: Permite indicar múltiples tipos de novedades en un mismo archivo
4. **Integridad**: Previene duplicados automáticamente
5. **Auditoría**: Facilita el seguimiento y corrección de errores

## Consideraciones Futuras

### Posibles Mejoras
- Agregar más tipos de novedades según las necesidades del negocio
- Implementar validaciones cruzadas (ej: no permitir horas normales el mismo día de una baja)
- Generar reportes de novedades importadas por período
- Permitir configurar el mapeo de letras desde la UI
- Validar que las novedades sean consistentes con las reglas de negocio (ej: días máximos de vacaciones)

### Compatibilidad
Esta funcionalidad es **retrocompatible**:
- Si no hay letras en las celdas, funciona como antes (solo carga horas)
- No requiere cambios en los archivos Excel existentes
- Es opcional: solo se activa cuando detecta letras válidas
