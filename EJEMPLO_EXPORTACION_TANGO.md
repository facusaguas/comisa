# Ejemplo Visual de Exportación Tango

## Escenario de Prueba

**Proyecto**: Construcción Central  
**Período**: Septiembre 2026  
**Quincena**: 1 (Primera quincena - del 1 al 15)  
**Empleados**: 3  

---

## Datos de Entrada

### Empleado 1: Juan Pérez (Legajo 1001)
| Concepto | Tipo | Valor |
|----------|------|-------|
| HsNormales | Horas | 88 horas |
| HsExt50 | Horas | 8 horas |
| SalarioBasico | Monto | $250,000.00 |
| Presentismo | Monto | $25,000.00 |
| AdelantoQuincenal | Monto | $100,000.00 |

### Empleado 2: María González (Legajo 1002)
| Concepto | Tipo | Valor |
|----------|------|-------|
| HsNormales | Horas | 88 horas |
| HsExt100 | Horas | 4 horas |
| PlusElectrico | Unidades | 2 |
| SalarioBasico | Monto | $280,000.00 |
| Antiguedad | Monto | $28,000.00 |
| Presentismo | Monto | $28,000.00 |

### Empleado 3: Carlos Rodríguez (Legajo 1003)
| Concepto | Tipo | Valor |
|----------|------|-------|
| HsNormales | Horas | 88 horas |
| SalarioBasico | Monto | $300,000.00 |
| Embargo | Monto | $50,000.00 |

---

## Resultado en Excel

### Vista del Archivo Excel Generado

```
┌─────────────┬────────┬──────────────────┬─────────────┬────────────┐
│   Fecha     │ Legajo │ CodigoConcepto   │ ValorEntero │   Monto    │
├─────────────┼────────┼──────────────────┼─────────────┼────────────┤
│ 15/09/2026  │  1001  │ HsNormales       │    88.00    │            │
│ 15/09/2026  │  1001  │ HsExt50          │     8.00    │            │
│ 15/09/2026  │  1001  │ SalarioBasico    │             │ 250000.00  │
│ 15/09/2026  │  1001  │ Presentismo      │             │  25000.00  │
│ 15/09/2026  │  1001  │ AdelantoQuinc... │             │ 100000.00  │
├─────────────┼────────┼──────────────────┼─────────────┼────────────┤
│ 15/09/2026  │  1002  │ HsNormales       │    88.00    │            │
│ 15/09/2026  │  1002  │ HsExt100         │     4.00    │            │
│ 15/09/2026  │  1002  │ PlusElectrico    │     2.00    │            │
│ 15/09/2026  │  1002  │ SalarioBasico    │             │ 280000.00  │
│ 15/09/2026  │  1002  │ Antiguedad       │             │  28000.00  │
│ 15/09/2026  │  1002  │ Presentismo      │             │  28000.00  │
├─────────────┼────────┼──────────────────┼─────────────┼────────────┤
│ 15/09/2026  │  1003  │ HsNormales       │    88.00    │            │
│ 15/09/2026  │  1003  │ SalarioBasico    │             │ 300000.00  │
│ 15/09/2026  │  1003  │ Embargo          │             │  50000.00  │
└─────────────┴────────┴──────────────────┴─────────────┴────────────┘
```

---

## Características del Archivo

### Metadata del Archivo
- **Nombre**: `Tango_Liquidacion_2026_09_Q1_20261113.xlsx`
- **Formato**: Excel (.xlsx)
- **Filas**: 15 (una por cada concepto de cada empleado)
- **Columnas**: 5 (A, B, C, D, E)
- **Encabezado**: NO (solo datos)

### Formato de Celdas
- **Columna A (Fecha)**: Formato de fecha dd/MM/yyyy
- **Columna B (Legajo)**: Texto
- **Columna C (CodigoConcepto)**: Texto
- **Columna D (ValorEntero)**: Número con 2 decimales (0.00)
- **Columna E (Monto)**: Número con 2 decimales (0.00)

---

## Análisis por Empleado

### Resumen Empleado 1 (Juan Pérez - 1001)

**Total de registros**: 5

| Tipo de Concepto | Cantidad | Detalle |
|------------------|----------|---------|
| ValorEntero | 2 | HsNormales (88), HsExt50 (8) |
| Monto | 3 | SalarioBasico, Presentismo, AdelantoQuincenal |

**Totales**:
- Horas trabajadas: 96 horas (88 normales + 8 extras al 50%)
- Haberes: $275,000.00
- Deducciones: $100,000.00

---

### Resumen Empleado 2 (María González - 1002)

**Total de registros**: 6

| Tipo de Concepto | Cantidad | Detalle |
|------------------|----------|---------|
| ValorEntero | 3 | HsNormales (88), HsExt100 (4), PlusElectrico (2) |
| Monto | 3 | SalarioBasico, Antiguedad, Presentismo |

**Totales**:
- Horas trabajadas: 92 horas (88 normales + 4 extras al 100%)
- Plus: 2 unidades
- Haberes: $336,000.00
- Deducciones: $0.00

---

### Resumen Empleado 3 (Carlos Rodríguez - 1003)

**Total de registros**: 3

| Tipo de Concepto | Cantidad | Detalle |
|------------------|----------|---------|
| ValorEntero | 1 | HsNormales (88) |
| Monto | 2 | SalarioBasico, Embargo |

**Totales**:
- Horas trabajadas: 88 horas normales
- Haberes: $300,000.00
- Deducciones: $50,000.00 (embargo)

---

## Validaciones en Excel

### Celdas con ValorEntero
```
= CONTARA(D:D) = 6  ← 6 conceptos tienen ValorEntero
```

### Celdas con Monto
```
= CONTARA(E:E) = 8  ← 8 conceptos tienen Monto
```

### Total de Registros
```
= CONTARA(A:A) = 15  ← 15 registros en total
```

### Suma de Montos
```
= SUMA(E:E) = $861,000.00  ← Total de haberes y deducciones
```

---

## Comparación: Antes vs Ahora

### ANTES (Texto Plano)
```
H|2026-09|1|3|861000.00|150000.00|711000.00
E|1001|20301001001|Juan|Pérez|275000.00|100000.00|175000.00|QUINCENAL
D|1001|HsNormales|88.00||...
D|1001|HsExt50|8.00||...
...
T|3|861000.00|150000.00|711000.00
```

**Problemas**:
- ❌ Difícil de leer
- ❌ Necesita parseo complejo
- ❌ No se puede editar fácilmente
- ❌ Propenso a errores

### AHORA (Excel)
```
15/09/2026 | 1001 | HsNormales    | 88.00 |
15/09/2026 | 1001 | HsExt50       | 8.00  |
15/09/2026 | 1001 | SalarioBasico |       | 250000.00
...
```

**Ventajas**:
- ✅ Fácil de leer y entender
- ✅ Se abre directo en Excel
- ✅ Editable si es necesario
- ✅ Validación automática de tipos
- ✅ Se puede filtrar y ordenar

---

## Uso del Archivo en Tango

### Paso 1: Importar en Tango
1. Abrir Tango Gestión
2. Módulo: **Liquidación de Sueldos**
3. Opción: **Importar desde Excel**
4. Seleccionar archivo: `Tango_Liquidacion_2026_09_Q1_20261113.xlsx`

### Paso 2: Mapear Columnas (primera vez)
```
Columna A → Campo: FechaLiquidacion
Columna B → Campo: Legajo
Columna C → Campo: CodigoConcepto
Columna D → Campo: Unidades
Columna E → Campo: Importe
```

### Paso 3: Validar Importación
Tango mostrará un resumen:
```
✓ 15 registros procesados
✓ 3 empleados actualizados
✓ 0 errores encontrados
✓ Importación exitosa
```

### Paso 4: Revisar y Confirmar
- Verificar que los importes sean correctos
- Revisar las horas cargadas
- Confirmar la importación

---

## Casos Especiales

### Ejemplo 1: Solo Horas (Sin montos)
```
15/09/2026 | 2001 | HsNormales    | 176.00 |
15/09/2026 | 2001 | HsExt50       |  10.00 |
```
← Columna Monto vacía

### Ejemplo 2: Solo Montos (Sin horas)
```
15/09/2026 | 3001 | Vacaciones    |        | 150000.00
15/09/2026 | 3001 | SAC           |        |  75000.00
```
← Columna ValorEntero vacía

### Ejemplo 3: Quincena 2
```
30/09/2026 | 1001 | HsNormales    | 88.00  |
```
← Fecha es el último día del mes (30 en septiembre)

### Ejemplo 4: Mes con 31 días
```
31/10/2026 | 1001 | HsNormales    | 88.00  |
```
← Octubre tiene 31 días

---

## Tips de Uso

### ✅ Buenas Prácticas

1. **Revisar antes de importar**
   - Abrir el Excel
   - Verificar visualmente los datos
   - Ordenar por legajo si es necesario

2. **Validar totales**
   - Usar fórmulas de Excel para sumar
   - Comparar con reportes del sistema
   - Verificar cantidad de empleados

3. **Guardar copia**
   - Archivar el Excel generado
   - Usar para auditorías futuras
   - Mantener histórico de liquidaciones

4. **Documentar cambios**
   - Si se edita manualmente el Excel
   - Agregar una hoja "Notas"
   - Indicar qué se modificó y por qué

### ❌ Errores Comunes

1. **No abrir el archivo antes de importar**
   → Siempre revisar visualmente

2. **Modificar la estructura**
   → No agregar/eliminar columnas

3. **Cambiar formatos de celda**
   → Mantener los formatos originales

4. **Mezclar quincenas**
   → Generar un archivo por quincena

---

## Resumen de Beneficios

| Aspecto | Mejora |
|---------|--------|
| **Visualización** | ⭐⭐⭐⭐⭐ Excelente |
| **Edición** | ⭐⭐⭐⭐⭐ Muy fácil |
| **Validación** | ⭐⭐⭐⭐⭐ Automática |
| **Importación** | ⭐⭐⭐⭐⭐ Directa |
| **Auditoría** | ⭐⭐⭐⭐⭐ Completa |

---

**Nota**: Este ejemplo muestra el formato esperado. Los valores reales dependerán de la liquidación calculada por el sistema.
