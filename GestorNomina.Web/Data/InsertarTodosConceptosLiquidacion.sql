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

-- 1001 - Horas Normales
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1001', 'Horas Normales', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1002 - Horas Extras 50%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1002', 'Horas Extras 50%', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1003 - Horas Extras 100%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1003', 'Horas Extras 100%', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1004 - Horas Nocturnas
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1004', 'Horas Nocturnas', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1005 - Horas Nocturnas Extras 50%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1005', 'Horas Nocturnas Extras 50%', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1006 - Horas Nocturnas Extras 100%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1006', 'Horas Nocturnas Extras 100%', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1010 - Plus Altura 15%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1010', 'Plus Altura 15%', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1020 - Presentismo UOCRA 20%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1020', 'Presentismo UOCRA 20%', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1025 - Adicional Montaje UOCRA (Fenix) 25%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1025', 'Adicional Montaje UOCRA (Fenix) 25%', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1030 - Feriados / Licencias
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1030', 'Feriados / Licencias', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1031 - Enfermedad / Licencias
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1031', 'Enfermedad / Licencias', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1033 - Compensación Especial
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1033', 'Compensación Especial', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1050 - Bonificación Especial - Ajuste Neto Convenido
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1050', 'Bonificación Especial - Ajuste Neto Convenido', 1)
ON CONFLICT (codigo) DO NOTHING;

-- 1090 - Plus Eléctrico 15%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1090', 'Plus Eléctrico 15%', 1)
ON CONFLICT (codigo) DO NOTHING;

\echo '✓ Conceptos remunerativos insertados';
\echo '';

-- ============================================================================
-- CONCEPTOS NO REMUNERATIVOS (tipo_concepto = 3)
-- ============================================================================

\echo '--- CONCEPTOS NO REMUNERATIVOS ---';
\echo '';

-- 1034 - Premio Final de Obra (NO REMUNERATIVO)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('1034', 'Premio Final de Obra', 3)
ON CONFLICT (codigo) DO UPDATE
SET tipo_concepto = 3
WHERE conceptos_liquidacion.tipo_concepto != 3;

\echo '✓ Premio Final de Obra (No Remunerativo)';
\echo '';

-- ============================================================================
-- DEDUCCIONES (tipo_concepto = 2)
-- ============================================================================

\echo '--- DEDUCCIONES ---';
\echo '';

-- 5001 - Jubilación 11%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('5001', 'Jubilación 11%', 2)
ON CONFLICT (codigo) DO NOTHING;

-- 5002 - Ley 19.032 3%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('5002', 'Ley 19.032 3%', 2)
ON CONFLICT (codigo) DO NOTHING;

-- 5003 - Obra Social 3%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('5003', 'Obra Social 3%', 2)
ON CONFLICT (codigo) DO NOTHING;

-- 5004 - Cuota Sindical 2.5%
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('5004', 'Cuota Sindical 2.5%', 2)
ON CONFLICT (codigo) DO NOTHING;

-- 5010 - Seguro de Vida Obligatorio
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('5010', 'Seguro de Vida Obligatorio', 2)
ON CONFLICT (codigo) DO NOTHING;

-- 5011 - Embargo
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
VALUES ('5011', 'Embargo', 2)
ON CONFLICT (codigo) DO NOTHING;

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
