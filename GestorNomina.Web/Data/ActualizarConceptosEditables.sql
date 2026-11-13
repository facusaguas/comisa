-- ============================================================================
-- Actualizar conceptos de liquidación con flags es_obligatorio y es_editable
-- ============================================================================

\echo '============================================================';
\echo 'Actualizando flags de conceptos de liquidación';
\echo '============================================================';
\echo '';

-- ============================================================================
-- OBLIGATORIOS Y NO EDITABLES
-- ============================================================================

\echo '--- Conceptos Obligatorios y NO Editables ---';

-- Horas Nocturnas (calculado automáticamente)
UPDATE conceptos_liquidacion
SET es_obligatorio = true, es_editable = false
WHERE codigo = '1004';

-- Jubilación 11%
UPDATE conceptos_liquidacion
SET es_obligatorio = true, es_editable = false
WHERE codigo = '5001';

-- Ley 19.032 3%
UPDATE conceptos_liquidacion
SET es_obligatorio = true, es_editable = false
WHERE codigo = '5002';

-- Obra Social 3%
UPDATE conceptos_liquidacion
SET es_obligatorio = true, es_editable = false
WHERE codigo = '5003';

\echo '✓ 4 conceptos marcados como obligatorios y no editables';
\echo '';

-- ============================================================================
-- OBLIGATORIOS Y EDITABLES
-- ============================================================================

\echo '--- Conceptos Obligatorios y Editables ---';

-- Horas Normales (puede editarse manualmente)
UPDATE conceptos_liquidacion
SET es_obligatorio = true, es_editable = true
WHERE codigo = '1001';

\echo '✓ 1 concepto marcado como obligatorio y editable';
\echo '';

-- ============================================================================
-- OPCIONALES Y EDITABLES
-- ============================================================================

\echo '--- Conceptos Opcionales y Editables ---';

-- Horas Extras 50%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1002';

-- Horas Extras 100%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1003';

-- Horas Nocturnas Extras 50%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1005';

-- Horas Nocturnas Extras 100%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1006';

-- Plus Altura 15%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1010';

-- Presentismo UOCRA 20%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1020';

-- Bonificación Especial - Ajuste Neto Convenido
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1050';

-- Plus Eléctrico 15%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '1090';

-- Cuota Sindical 2.5%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '5004';

-- Seguro de Vida Obligatorio
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '5010';

-- Embargo
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = true
WHERE codigo = '5011';

\echo '✓ 11 conceptos marcados como opcionales y editables';
\echo '';

-- ============================================================================
-- OPCIONALES Y NO EDITABLES (calculados automáticamente)
-- ============================================================================

\echo '--- Conceptos Opcionales y NO Editables (calculados) ---';

-- Adicional Montaje UOCRA (Fenix) 25%
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = false
WHERE codigo = '1025';

-- Feriados / Licencias
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = false
WHERE codigo = '1030';

-- Enfermedad / Licencias
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = false
WHERE codigo = '1031';

-- Compensación Especial
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = false
WHERE codigo = '1033';

-- Premio Final de Obra (No Remunerativo)
UPDATE conceptos_liquidacion
SET es_obligatorio = false, es_editable = false
WHERE codigo = '1034';

\echo '✓ 5 conceptos marcados como opcionales y no editables';
\echo '';

-- ============================================================================
-- RESUMEN
-- ============================================================================

\echo '============================================================';
\echo 'Resumen de configuración';
\echo '============================================================';
\echo '';

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

\echo '';
\echo 'Detalle por concepto:';

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
WHERE codigo IN (
    '1001', '1002', '1003', '1004', '1005', '1006', '1010', '1020', '1025', '1030',
    '1031', '1033', '1034', '1050', '1090',
    '5001', '5002', '5003', '5004', '5010', '5011'
)
ORDER BY codigo::integer;

\echo '';
\echo '============================================================';
\echo 'Actualización completada exitosamente';
\echo '============================================================';
