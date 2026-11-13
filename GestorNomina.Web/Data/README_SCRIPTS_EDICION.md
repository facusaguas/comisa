# 📝 Scripts SQL para Funcionalidad de Edición de Recibos

## 📋 Índice

1. [Scripts Disponibles](#scripts-disponibles)
2. [Ejecución Recomendada](#ejecución-recomendada)
3. [Detalles de Cada Script](#detalles-de-cada-script)
4. [Verificación Post-Ejecución](#verificación-post-ejecución)
5. [Rollback](#rollback)
6. [Troubleshooting](#troubleshooting)

---

## 📄 Scripts Disponibles

### 1. **ScriptCompletoEdicionRecibos.sql** ⭐ RECOMENDADO
**Descripción:** Script TODO-EN-UNO que realiza todos los cambios necesarios.

**Incluye:**
- ✅ Agrega campos a tablas (conceptos_liquidacion, recibos, recibo_detalles)
- ✅ Inserta/actualiza todos los 21 conceptos de liquidación
- ✅ Configura conceptos como editables/obligatorios
- ✅ Crea índices para optimizar performance
- ✅ Verifica que todo se aplicó correctamente

**Cuándo usar:** Primera vez instalando la funcionalidad o actualizando desde cero.

---

### 2. **AgregarFuncionalidadEdicionRecibos.sql**
**Descripción:** Solo agrega la estructura (campos e índices) sin tocar conceptos.

**Incluye:**
- ✅ Agrega campos a tablas
- ✅ Actualiza conceptos existentes con flags
- ✅ Crea índices
- ❌ NO inserta conceptos nuevos

**Cuándo usar:** Si ya tienes conceptos en la BD y solo quieres agregar los campos de edición.

---

### 3. **InsertarTodosConceptosLiquidacion.sql**
**Descripción:** Inserta/actualiza todos los conceptos con la configuración de editabilidad.

**Incluye:**
- ✅ Inserta 21 conceptos (si no existen)
- ✅ Actualiza conceptos existentes con flags
- ❌ NO modifica estructura de tablas

**Cuándo usar:** Si ya tienes los campos en las tablas pero necesitas actualizar conceptos.

---

### 4. **ActualizarConceptosEditables.sql**
**Descripción:** Solo actualiza los flags de conceptos existentes.

**Incluye:**
- ✅ Actualiza es_obligatorio y es_editable de conceptos existentes
- ❌ NO inserta conceptos nuevos
- ❌ NO modifica estructura de tablas

**Cuándo usar:** Si ya tienes conceptos y campos, solo necesitas actualizar la configuración.

---

### 5. **RevertirFuncionalidadEdicionRecibos.sql** ⚠️
**Descripción:** Rollback completo - revierte todos los cambios.

**⚠️ ADVERTENCIA:** Elimina campos y datos de edición. Úsalo con precaución.

---

## 🚀 Ejecución Recomendada

### Opción 1: Primera Instalación (RECOMENDADA)

```bash
# Ejecutar script completo
psql -U postgres -d gestor_nomina -f ScriptCompletoEdicionRecibos.sql
```

Este script es **idempotente** - puedes ejecutarlo múltiples veces sin problemas.

---

### Opción 2: Actualización Parcial

**Si ya tienes campos pero no conceptos:**
```bash
psql -U postgres -d gestor_nomina -f InsertarTodosConceptosLiquidacion.sql
```

**Si ya tienes conceptos pero no campos:**
```bash
psql -U postgres -d gestor_nomina -f AgregarFuncionalidadEdicionRecibos.sql
```

**Si solo necesitas actualizar flags:**
```bash
psql -U postgres -d gestor_nomina -f ActualizarConceptosEditables.sql
```

---

## 📊 Detalles de Cada Script

### ScriptCompletoEdicionRecibos.sql

#### Cambios en Base de Datos:

**Tabla `conceptos_liquidacion`:**
```sql
+ es_obligatorio  BOOLEAN NOT NULL DEFAULT false
+ es_editable     BOOLEAN NOT NULL DEFAULT false
```

**Tabla `recibos`:**
```sql
+ modo_edicion              VARCHAR(20) NOT NULL DEFAULT 'AUTOMATICO'
+ editado_por_usuario_id    INTEGER NULL (FK → usuarios.id)
+ fecha_ultima_edicion      TIMESTAMP NULL
+ observaciones_edicion     TEXT NULL
```

**Tabla `recibo_detalles`:**
```sql
+ editado_manualmente  BOOLEAN NOT NULL DEFAULT false
+ valor_original       NUMERIC(12, 2) NULL
```

**Índices creados:**
```sql
idx_conceptos_liquidacion_editable  ON conceptos_liquidacion(es_editable, es_obligatorio)
idx_recibos_modo_edicion            ON recibos(modo_edicion)
idx_recibo_detalles_editado         ON recibo_detalles(editado_manualmente)
```

#### Configuración de Conceptos:

| Categoría | Cantidad | Conceptos |
|-----------|----------|-----------|
| **Obligatorio + Editable** | 1 | 1001 (Horas Normales) |
| **Obligatorio + No Editable** | 4 | 1004, 5001, 5002, 5003 |
| **Opcional + Editable** | 11 | 1002, 1003, 1005, 1006, 1010, 1020, 1050, 1090, 5004, 5010, 5011 |
| **Opcional + No Editable** | 5 | 1025, 1030, 1031, 1033, 1034 |
| **TOTAL** | **21** | |

---

## ✅ Verificación Post-Ejecución

### 1. Verificar campos agregados:

```sql
-- Verificar conceptos_liquidacion
SELECT column_name, data_type 
FROM information_schema.columns
WHERE table_name = 'conceptos_liquidacion'
AND column_name IN ('es_obligatorio', 'es_editable');

-- Verificar recibos
SELECT column_name, data_type 
FROM information_schema.columns
WHERE table_name = 'recibos'
AND column_name IN ('modo_edicion', 'editado_por_usuario_id', 'fecha_ultima_edicion', 'observaciones_edicion');

-- Verificar recibo_detalles
SELECT column_name, data_type 
FROM information_schema.columns
WHERE table_name = 'recibo_detalles'
AND column_name IN ('editado_manualmente', 'valor_original');
```

**Resultado esperado:** Todas las consultas deben retornar filas.

---

### 2. Verificar configuración de conceptos:

```sql
-- Ver resumen por categoría
SELECT 
    CASE 
        WHEN es_obligatorio AND es_editable THEN 'OBLIGATORIO + EDITABLE'
        WHEN es_obligatorio AND NOT es_editable THEN 'OBLIGATORIO + NO EDITABLE'
        WHEN NOT es_obligatorio AND es_editable THEN 'OPCIONAL + EDITABLE'
        WHEN NOT es_obligatorio AND NOT es_editable THEN 'OPCIONAL + NO EDITABLE'
    END AS categoria,
    COUNT(*) as cantidad
FROM conceptos_liquidacion
GROUP BY es_obligatorio, es_editable
ORDER BY es_obligatorio DESC, es_editable DESC;
```

**Resultado esperado:**
```
categoria                       | cantidad
--------------------------------|----------
OBLIGATORIO + EDITABLE          |        1
OBLIGATORIO + NO EDITABLE       |        4
OPCIONAL + EDITABLE             |       11
OPCIONAL + NO EDITABLE          |        5
```

---

### 3. Verificar conceptos específicos:

```sql
SELECT 
    codigo,
    nombre,
    CASE 
        WHEN tipo_concepto = 1 THEN 'REMUNERATIVO'
        WHEN tipo_concepto = 2 THEN 'DEDUCCIÓN'
        WHEN tipo_concepto = 3 THEN 'NO REMUNERATIVO'
    END AS tipo,
    CASE WHEN es_obligatorio THEN 'Sí' ELSE 'No' END AS obligatorio,
    CASE WHEN es_editable THEN 'Sí' ELSE 'No' END AS editable
FROM conceptos_liquidacion
WHERE codigo IN ('1001', '1004', '5001', '5002', '5003')
ORDER BY codigo;
```

**Resultado esperado:**
```
codigo | nombre              | tipo          | obligatorio | editable
-------|---------------------|---------------|-------------|----------
1001   | Horas Normales      | REMUNERATIVO  | Sí          | Sí
1004   | Horas Nocturnas     | REMUNERATIVO  | Sí          | No
5001   | Jubilación 11%      | DEDUCCIÓN     | Sí          | No
5002   | Ley 19.032 3%       | DEDUCCIÓN     | Sí          | No
5003   | Obra Social 3%      | DEDUCCIÓN     | Sí          | No
```

---

### 4. Verificar índices:

```sql
SELECT indexname, tablename
FROM pg_indexes
WHERE tablename IN ('conceptos_liquidacion', 'recibos', 'recibo_detalles')
AND (indexname LIKE '%edita%' OR indexname LIKE '%modo%')
ORDER BY tablename, indexname;
```

**Resultado esperado:** Debe mostrar 3 índices.

---

## ⏪ Rollback

### Revertir todos los cambios:

```bash
psql -U postgres -d gestor_nomina -f RevertirFuncionalidadEdicionRecibos.sql
```

**⚠️ ADVERTENCIA:** 
- Esto eliminará todos los campos agregados
- Se perderán los datos de edición manual si existen
- Los recibos volverán a modo solo-lectura

**Confirmación:** El script espera 5 segundos antes de ejecutar. Presiona `Ctrl+C` para cancelar.

---

## 🔧 Troubleshooting

### Error: "permission denied for table conceptos_liquidacion"

**Solución:** Ejecutar como superusuario de PostgreSQL:
```bash
psql -U postgres -d gestor_nomina -f ScriptCompletoEdicionRecibos.sql
```

---

### Error: "column already exists"

**Causa:** El campo ya fue agregado anteriormente.

**Solución:** No es un error crítico. El script usa `IF NOT EXISTS` para evitar duplicados. Puedes ignorarlo.

---

### Error: "relation does not exist"

**Causa:** El script espera que las tablas base ya existan.

**Solución:** Asegúrate de que la aplicación ya haya creado las tablas base mediante migraciones anteriores.

---

### Error: "foreign key constraint violation"

**Causa:** Intentas eliminar un usuario que tiene recibos editados.

**Solución:** El FK usa `ON DELETE SET NULL`, por lo que el campo `editado_por_usuario_id` se pondrá en NULL automáticamente.

---

### Verificar si el script ya fue ejecutado:

```sql
-- Si retorna filas, el script ya fue ejecutado
SELECT 1 
FROM information_schema.columns 
WHERE table_name = 'conceptos_liquidacion' 
AND column_name = 'es_editable';
```

---

### Ver historial de cambios en recibos:

```sql
-- Ver recibos editados manualmente
SELECT 
    r.id,
    e.legajo,
    e.nombre_completo,
    r.periodo,
    r.quincena,
    r.modo_edicion,
    u.nombre_completo as editado_por,
    r.fecha_ultima_edicion
FROM recibos r
JOIN empleados e ON r.empleado_id = e.id
LEFT JOIN usuarios u ON r.editado_por_usuario_id = u.id
WHERE r.modo_edicion = 'MANUAL'
ORDER BY r.fecha_ultima_edicion DESC;
```

---

## 📞 Soporte

Si encuentras problemas durante la ejecución:

1. **Verifica la versión de PostgreSQL:** `SELECT version();`
2. **Verifica permisos de usuario:** Debe tener permisos DDL (ALTER, CREATE)
3. **Revisa el log de errores:** Los errores se mostrarán en la consola
4. **Ejecuta las verificaciones:** Usa las queries de verificación arriba

---

## 📅 Changelog

- **2024-10-29:** Versión inicial
  - Script completo unificado
  - Scripts modulares separados
  - Script de rollback
  - Documentación completa

---

**✅ IMPORTANTE:** Después de ejecutar el script, **reinicia la aplicación** para que Entity Framework reconozca los nuevos campos.
