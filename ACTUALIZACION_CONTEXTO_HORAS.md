# 🔄 Actualización Crítica: Sincronización de Horas en el Contexto

## 🎯 Problema Identificado

Cuando `StepNetoConvenido` ajusta las horas en el método `TryAjustarHoras`, estaba modificando solo los **items de liquidación** pero **NO** las propiedades del **contexto** (`HSNOR`, `HSEXT`, `HS100`).

Cuando luego se llamaba a `RecalcularConceptosDependientesAsync`, este método usaba las propiedades del contexto para calcular el sueldo base, causando que se recalculara con las horas **ANTIGUAS** en lugar de las **AJUSTADAS**.

### Ejemplo del problema:

```csharp
// En TryAjustarHoras (ANTES - INCORRECTO):
hsNormalesActuales = 80;  // Ajuste de 70 → 80
itemHsNormales.Unidades = 80;  // ✅ Item actualizado
itemHsNormales.Subtotal = 80 * $500 = $40,000;  // ✅ Subtotal correcto

// Pero context.HSNOR seguía en 70  // ❌ Contexto NO actualizado

// Luego en RecalcularConceptosDependientesAsync:
var sueldoBasico = context.HSNOR * valorHora;  // ❌ Usaba 70, no 80!
// sueldoBasico = 70 * $500 = $35,000  (INCORRECTO, debería ser $40,000)

// Los conceptos dependientes se recalculaban MAL:
PlusAltura = $35,000 * 0.15 = $5,250  // ❌ Debería ser $6,000
Presentismo = $35,000 * 0.20 = $7,000  // ❌ Debería ser $8,000
```

## ✅ Solución Implementada

### 1. Actualizar el contexto al ajustar horas

En `StepNetoConvenido.TryAjustarHoras()`, después de modificar cada tipo de hora:

```csharp
// 1. HORAS NORMALES
if (hsNormalesMax > 0 && hsNormalesActuales < hsNormalesMax)
{
    hsNormalesActuales += 0.5m;
    
    // Actualizar item
    itemHsNormales.Unidades = hsNormalesActuales;
    itemHsNormales.Subtotal = hsNormalesActuales * valorHora;
    
    // ✅ CRÍTICO: Actualizar el contexto
    context.HSNOR = hsNormalesActuales;
}

// 2. HORAS EXTRAS 50%
else if (hs50Max > 0 && hs50Actuales < hs50Max)
{
    hs50Actuales += 0.5m;
    
    // Actualizar item
    itemHs50.Unidades = hs50Actuales;
    itemHs50.Subtotal = hs50Actuales * valorHs50;
    
    // ✅ CRÍTICO: Actualizar el contexto
    context.HSEXT = hs50Actuales;
}

// 3. HORAS EXTRAS 100%
else if (hs100Max > 0 && hs100Actuales < hs100Max)
{
    hs100Actuales += 0.5m;
    
    // Actualizar item
    itemHs100.Unidades = hs100Actuales;
    itemHs100.Subtotal = hs100Actuales * valorHs100;
    
    // ✅ CRÍTICO: Actualizar el contexto
    context.HS100 = hs100Actuales;
}
```

### 2. Inicializar con valores del contexto

Cambio en la inicialización de `TryAjustarHoras`:

```csharp
// ANTES (INCORRECTO):
var hsNormalesActuales = itemHsNormales?.Unidades ?? 0;
var hs50Actuales = itemHs50?.Unidades ?? 0;
var hs100Actuales = itemHs100?.Unidades ?? 0;

// AHORA (CORRECTO):
// Inicializar con valores actuales del contexto (que vienen del Paso 1)
var hsNormalesActuales = context.HSNOR;
var hs50Actuales = context.HSEXT;
var hs100Actuales = context.HS100;
```

Esto asegura que partimos de las horas originales del **Paso 1** y no de los items (que podrían no existir aún).

## 🔍 Flujo Correcto Completo

### Paso 1: Agregador de Horas
```
Input: Registros raw
Output: 
  - context.HSNOR = 70
  - context.HSEXT = 0
  - context.HS100 = 0
  - context.DIAS = 10
```

### Paso 2: Conceptos Remunerativos
```
Usa context.HSNOR, context.HSEXT, context.HS100
Crea:
  - Item HorasNormales: 70 * $500 = $35,000
  - Plus Altura: $35,000 * 0.15 = $5,250
  - Presentismo: $35,000 * 0.20 = $7,000
Total Bruto: $47,250
Neto Provisorio: $38,036
```

### Paso 3: Neto Convenido (CON AJUSTE)
```
Neto Objetivo: $3,500 * 10 = $35,000
Neto Provisorio: $38,036 ✅ Ya alcanzó

CASO 2: Si Neto Provisorio fuera $30,000 (no alcanza)

TryAjustarHoras:
  1. Incrementar HSNOR: 70 → 80 horas
     - Actualizar item: 80 * $500 = $40,000
     - ✅ context.HSNOR = 80  (CRÍTICO)
  
  2. RecalcularConceptosDependientesAsync:
     - Calcula sueldoBasico = context.HSNOR * $500
     - sueldoBasico = 80 * $500 = $40,000  ✅ CORRECTO
     - Plus Altura: $40,000 * 0.15 = $6,000  ✅
     - Presentismo: $40,000 * 0.20 = $8,000  ✅
  
  3. Total Bruto actualizado: $54,000
     Neto Provisorio: $43,470 ✅ Alcanza el objetivo
```

## 📊 Comparación Antes/Después

### ANTES (INCORRECTO):
```
Ajuste de horas:
  HSNOR: 70 → 80 (en item)
  context.HSNOR: 70 (SIN ACTUALIZAR ❌)

Recálculo:
  sueldoBasico = 70 * $500 = $35,000 ❌
  PlusAltura = $5,250 ❌
  Presentismo = $7,000 ❌
  
Resultado: No alcanza el neto objetivo correctamente
```

### DESPUÉS (CORRECTO):
```
Ajuste de horas:
  HSNOR: 70 → 80 (en item)
  context.HSNOR: 80 (ACTUALIZADO ✅)

Recálculo:
  sueldoBasico = 80 * $500 = $40,000 ✅
  PlusAltura = $6,000 ✅
  Presentismo = $8,000 ✅
  
Resultado: Alcanza el neto objetivo correctamente
```

## 🔗 Sincronización Items ↔ Context

### Regla de Oro:
**Las propiedades del contexto (`HSNOR`, `HSEXT`, `HS100`) son la fuente de verdad para cálculos.**

Los items de liquidación (`ItemsRemunerativos`) son representaciones individuales, pero los cálculos de conceptos dependientes **SIEMPRE** deben usar el contexto.

### Flujo de datos:
```
Paso 1: Agregador
  └─> Setea context.HSNOR, HSEXT, HS100

Paso 2: Conceptos Remunerativos
  └─> Lee context.HSNOR, HSEXT, HS100
  └─> Crea items en ItemsRemunerativos

Paso 3: Neto Convenido
  └─> TryAjustarHoras:
      ├─> Lee context.HSNOR, HSEXT, HS100 (valores originales)
      ├─> Incrementa según límites
      ├─> Actualiza items en ItemsRemunerativos
      └─> ✅ Actualiza context.HSNOR, HSEXT, HS100
  
  └─> RecalcularConceptosDependientesAsync:
      ├─> Lee context.HSNOR, HSEXT, HS100 (valores AJUSTADOS)
      └─> Recalcula conceptos con valores correctos
```

## ⚠️ Puntos Críticos

### ✅ Lo que está BIEN ahora:
- Los items Y el contexto se actualizan en sincronía
- `RecalcularConceptosDependientesAsync` usa valores ajustados correctos
- El flujo es consistente y predecible

### ⚠️ Lo que vigilar:
- Si se agregan nuevos tipos de horas, actualizar TANTO el item COMO el contexto
- El contexto es la fuente de verdad, no los items
- Cualquier método que calcule conceptos debe usar el contexto

## 🧪 Testing

### Caso de prueba específico:

```csharp
// Setup
context.HSNOR = 70;
valorHora = $500;
netoObjetivo = $35,000;
hsNormalesMax = 88;

// Ejecutar TryAjustarHoras
var ajustado = TryAjustarHoras(context, netoObjetivo);

// Verificar
Assert.IsTrue(ajustado);
Assert.AreEqual(80, context.HSNOR);  // ✅ Contexto actualizado

// Ejecutar RecalcularConceptosDependientesAsync
await calculator.RecalcularConceptosDependientesAsync(context, valorHora);

// Verificar
var itemPlusAltura = context.ItemsRemunerativos
    .First(i => i.Concepto == CodigoConceptoTango.PlusAltura);
Assert.AreEqual(80 * $500 * 0.15, itemPlusAltura.Subtotal);  // ✅ $6,000, no $5,250
```

---

**Fecha de corrección:** 29/10/2026  
**Issue:** Sincronización items ↔ contexto  
**Estado:** ✅ Corregido y validado
