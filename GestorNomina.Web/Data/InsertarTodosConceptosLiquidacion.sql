-- ============================================================================
-- Script Completo: Insertar TODOS los Conceptos de Liquidación (PostgreSQL)
-- ============================================================================
-- Este script inserta todos los conceptos necesarios para el sistema de liquidación
-- Evita errores de FK verificando existencia antes de insertar
-- ============================================================================

\echo '============================================================';
\echo 'Iniciando inserción de conceptos de liquidación';
\echo '============================================================';
\echo '';

-- ============================================================================
-- CONCEPTOS REMUNERATIVOS (tipo_concepto = 1)
-- ============================================================================

\echo '--- CONCEPTOS REMUNERATIVOS ---';
\echo '';

-- 1001 - Horas Normales (OBLIGATORIO + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1001', 'Horas Normales', 1, true, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = true, es_editable = true;

-- 1002 - Horas Extras 50% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1002', 'Horas Extras 50%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 1003 - Horas Extras 100% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1003', 'Horas Extras 100%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 1004 - Horas Nocturnas (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1004', 'Horas Nocturnas', 1, true, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = true, es_editable = false;

-- 1005 - Horas Nocturnas Extras 50% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1005', 'Horas Nocturnas Extras 50%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 1006 - Horas Nocturnas Extras 100% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1006', 'Horas Nocturnas Extras 100%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 1010 - Plus Altura 15% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1010', 'Plus Altura 15%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 1020 - Presentismo UOCRA 20% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1020', 'Presentismo UOCRA 20%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 1025 - Adicional Montaje UOCRA (Fenix) 25% (OPCIONAL + NO EDITABLE - calculado)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1025', 'Adicional Montaje UOCRA (Fenix) 25%', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = false;

-- 1030 - Feriados / Licencias (OPCIONAL + NO EDITABLE - calculado)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1030', 'Feriados / Licencias', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = false;

-- 1031 - Enfermedad / Licencias (OPCIONAL + NO EDITABLE - calculado)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1031', 'Enfermedad / Licencias', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = false;

-- 1033 - Compensación Especial (OPCIONAL + NO EDITABLE - calculado)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1033', 'Compensación Especial', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = false;

-- 1050 - Bonificación Especial - Ajuste Neto Convenido (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1050', 'Bonificación Especial - Ajuste Neto Convenido', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 1090 - Plus Eléctrico 15% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1090', 'Plus Eléctrico 15%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

\echo '✓ Conceptos remunerativos insertados';
\echo '';

-- ============================================================================
-- CONCEPTOS NO REMUNERATIVOS (tipo_concepto = 3)
-- ============================================================================

\echo '--- CONCEPTOS NO REMUNERATIVOS ---';
\echo '';

-- 1034 - Premio Final de Obra (NO REMUNERATIVO - OPCIONAL + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1034', 'Premio Final de Obra', 3, false, false)
ON CONFLICT (codigo) DO UPDATE
SET tipo_concepto = 3, es_obligatorio = false, es_editable = false
WHERE conceptos_liquidacion.tipo_concepto != 3;

\echo '✓ Premio Final de Obra (No Remunerativo)';
\echo '';

-- ============================================================================
-- DEDUCCIONES (tipo_concepto = 2)
-- ============================================================================

\echo '--- DEDUCCIONES ---';
\echo '';

-- 5001 - Jubilación 11% (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5001', 'Jubilación 11%', 2, true, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = true, es_editable = false;

-- 5002 - Ley 19.032 3% (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5002', 'Ley 19.032 3%', 2, true, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = true, es_editable = false;

-- 5003 - Obra Social 3% (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5003', 'Obra Social 3%', 2, true, false)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = true, es_editable = false;

-- 5004 - Cuota Sindical 2.5% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5004', 'Cuota Sindical 2.5%', 2, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 5010 - Seguro de Vida Obligatorio (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5010', 'Seguro de Vida Obligatorio', 2, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

-- 5011 - Embargo (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5011', 'Embargo', 2, false, true)
ON CONFLICT (codigo) DO UPDATE
SET es_obligatorio = false, es_editable = true;

\echo '✓ Deducciones insertadas';
\echo '';

\echo '============================================================';
\echo 'Proceso completado exitosamente';
\echo '============================================================';
\echo '';

-- ============================================================================
-- RESUMEN
-- ============================================================================

\echo '--- RESUMEN DE CONCEPTOS ---';
\echo '';

SELECT 
    CASE 
        WHEN tipo_concepto = 1 THEN 'REMUNERATIVO'
        WHEN tipo_concepto = 2 THEN 'DEDUCCIÓN'
        WHEN tipo_concepto = 3 THEN 'NO REMUNERATIVO'
        ELSE 'DESCONOCIDO'
    END AS tipo,
    codigo,
    nombre
FROM conceptos_liquidacion
WHERE codigo IN (
    '1001', '1002', '1003', '1004', '1005', '1006', '1010', '1020', '1025', '1030', 
    '1031', '1033', '1034', '1050', '1090',
    '5001', '5002', '5003', '5004', '5010', '5011'
)
ORDER BY tipo_concepto, codigo::integer;

\echo '';
\echo 'Total de conceptos verificados: 21';
