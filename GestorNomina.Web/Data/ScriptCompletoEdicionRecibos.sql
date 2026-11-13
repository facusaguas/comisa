-- ============================================================================
-- Script Completo Unificado: Funcionalidad de Edición de Recibos
-- ============================================================================
-- Este script realiza TODOS los cambios necesarios en un solo paso:
-- 1. Agrega campos a las tablas
-- 2. Inserta/actualiza conceptos de liquidación
-- 3. Configura conceptos como editables/obligatorios
-- 4. Crea índices
-- ============================================================================

\echo '============================================================';
\echo 'Script Unificado: Edición de Recibos';
\echo '============================================================';
\echo '';

-- ============================================================================
-- PARTE 1: ESTRUCTURA DE TABLAS
-- ============================================================================

\echo '========== PARTE 1: ESTRUCTURA DE TABLAS ==========';
\echo '';

-- Tabla: conceptos_liquidacion
\echo '--- Actualizando conceptos_liquidacion ---';

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'conceptos_liquidacion' AND column_name = 'es_obligatorio'
    ) THEN
        ALTER TABLE conceptos_liquidacion ADD COLUMN es_obligatorio BOOLEAN NOT NULL DEFAULT false;
        RAISE NOTICE '✓ Campo es_obligatorio agregado';
    ELSE
        RAISE NOTICE '○ Campo es_obligatorio ya existe';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'conceptos_liquidacion' AND column_name = 'es_editable'
    ) THEN
        ALTER TABLE conceptos_liquidacion ADD COLUMN es_editable BOOLEAN NOT NULL DEFAULT false;
        RAISE NOTICE '✓ Campo es_editable agregado';
    ELSE
        RAISE NOTICE '○ Campo es_editable ya existe';
    END IF;
END $$;

-- Tabla: recibos
\echo '--- Actualizando recibos ---';

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'recibos' AND column_name = 'modo_edicion'
    ) THEN
        ALTER TABLE recibos ADD COLUMN modo_edicion VARCHAR(20) NOT NULL DEFAULT 'AUTOMATICO';
        RAISE NOTICE '✓ Campo modo_edicion agregado';
    ELSE
        RAISE NOTICE '○ Campo modo_edicion ya existe';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'recibos' AND column_name = 'editado_por_usuario_id'
    ) THEN
        ALTER TABLE recibos ADD COLUMN editado_por_usuario_id INTEGER NULL;
        ALTER TABLE recibos ADD CONSTRAINT fk_recibos_editado_por_usuario
            FOREIGN KEY (editado_por_usuario_id) REFERENCES usuarios(id) ON DELETE SET NULL;
        RAISE NOTICE '✓ Campo editado_por_usuario_id agregado con FK';
    ELSE
        RAISE NOTICE '○ Campo editado_por_usuario_id ya existe';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'recibos' AND column_name = 'fecha_ultima_edicion'
    ) THEN
        ALTER TABLE recibos ADD COLUMN fecha_ultima_edicion TIMESTAMP NULL;
        RAISE NOTICE '✓ Campo fecha_ultima_edicion agregado';
    ELSE
        RAISE NOTICE '○ Campo fecha_ultima_edicion ya existe';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'recibos' AND column_name = 'observaciones_edicion'
    ) THEN
        ALTER TABLE recibos ADD COLUMN observaciones_edicion TEXT NULL;
        RAISE NOTICE '✓ Campo observaciones_edicion agregado';
    ELSE
        RAISE NOTICE '○ Campo observaciones_edicion ya existe';
    END IF;
END $$;

-- Tabla: recibo_detalles
\echo '--- Actualizando recibo_detalles ---';

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'recibo_detalles' AND column_name = 'editado_manualmente'
    ) THEN
        ALTER TABLE recibo_detalles ADD COLUMN editado_manualmente BOOLEAN NOT NULL DEFAULT false;
        RAISE NOTICE '✓ Campo editado_manualmente agregado';
    ELSE
        RAISE NOTICE '○ Campo editado_manualmente ya existe';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'recibo_detalles' AND column_name = 'valor_original'
    ) THEN
        ALTER TABLE recibo_detalles ADD COLUMN valor_original NUMERIC(12, 2) NULL;
        RAISE NOTICE '✓ Campo valor_original agregado';
    ELSE
        RAISE NOTICE '○ Campo valor_original ya existe';
    END IF;
END $$;

\echo '';

-- ============================================================================
-- PARTE 2: INSERCIÓN/ACTUALIZACIÓN DE CONCEPTOS
-- ============================================================================

\echo '========== PARTE 2: CONCEPTOS DE LIQUIDACIÓN ==========';
\echo '';

-- CONCEPTOS REMUNERATIVOS

\echo '--- Conceptos Remunerativos ---';

-- 1001 - Horas Normales (OBLIGATORIO + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1001', 'Horas Normales', 1, true, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto, 
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1002 - Horas Extras 50% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1002', 'Horas Extras 50%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1003 - Horas Extras 100% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1003', 'Horas Extras 100%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1004 - Horas Nocturnas (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1004', 'Horas Nocturnas', 1, true, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1005 - Horas Nocturnas Extras 50% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1005', 'Horas Nocturnas Extras 50%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1006 - Horas Nocturnas Extras 100% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1006', 'Horas Nocturnas Extras 100%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1010 - Plus Altura 15% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1010', 'Plus Altura 15%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1020 - Presentismo UOCRA 20% (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1020', 'Presentismo UOCRA 20%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1025 - Adicional Montaje (OPCIONAL + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1025', 'Adicional Montaje UOCRA (Fenix) 25%', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1030 - Feriados / Licencias (OPCIONAL + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1030', 'Feriados / Licencias', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1031 - Enfermedad (OPCIONAL + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1031', 'Enfermedad / Licencias', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1033 - Compensación Especial (OPCIONAL + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1033', 'Compensación Especial', 1, false, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1050 - Bonificación Especial (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1050', 'Bonificación Especial - Ajuste Neto Convenido', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 1090 - Plus Eléctrico (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1090', 'Plus Eléctrico 15%', 1, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

\echo '✓ Conceptos remunerativos configurados';

-- CONCEPTOS NO REMUNERATIVOS

\echo '--- Conceptos No Remunerativos ---';

-- 1034 - Premio Final de Obra (OPCIONAL + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('1034', 'Premio Final de Obra', 3, false, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = 3,
    es_obligatorio = false, es_editable = false;

\echo '✓ Conceptos no remunerativos configurados';

-- DEDUCCIONES

\echo '--- Deducciones ---';

-- 5001 - Jubilación (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5001', 'Jubilación 11%', 2, true, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 5002 - Ley 19.032 (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5002', 'Ley 19.032 3%', 2, true, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 5003 - Obra Social (OBLIGATORIO + NO EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5003', 'Obra Social 3%', 2, true, false)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 5004 - Cuota Sindical (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5004', 'Cuota Sindical 2.5%', 2, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 5010 - Seguro de Vida (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5010', 'Seguro de Vida Obligatorio', 2, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

-- 5011 - Embargo (OPCIONAL + EDITABLE)
INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto, es_obligatorio, es_editable)
VALUES ('5011', 'Embargo', 2, false, true)
ON CONFLICT (codigo) DO UPDATE
SET nombre = EXCLUDED.nombre, tipo_concepto = EXCLUDED.tipo_concepto,
    es_obligatorio = EXCLUDED.es_obligatorio, es_editable = EXCLUDED.es_editable;

\echo '✓ Deducciones configuradas';
\echo '';

-- ============================================================================
-- PARTE 3: ÍNDICES
-- ============================================================================

\echo '========== PARTE 3: ÍNDICES ==========';
\echo '';

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_conceptos_liquidacion_editable') THEN
        CREATE INDEX idx_conceptos_liquidacion_editable 
        ON conceptos_liquidacion(es_editable, es_obligatorio);
        RAISE NOTICE '✓ Índice idx_conceptos_liquidacion_editable creado';
    ELSE
        RAISE NOTICE '○ Índice idx_conceptos_liquidacion_editable ya existe';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_recibos_modo_edicion') THEN
        CREATE INDEX idx_recibos_modo_edicion ON recibos(modo_edicion);
        RAISE NOTICE '✓ Índice idx_recibos_modo_edicion creado';
    ELSE
        RAISE NOTICE '○ Índice idx_recibos_modo_edicion ya existe';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'idx_recibo_detalles_editado') THEN
        CREATE INDEX idx_recibo_detalles_editado ON recibo_detalles(editado_manualmente);
        RAISE NOTICE '✓ Índice idx_recibo_detalles_editado creado';
    ELSE
        RAISE NOTICE '○ Índice idx_recibo_detalles_editado ya existe';
    END IF;
END $$;

\echo '';

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================

\echo '============================================================';
\echo 'VERIFICACIÓN FINAL';
\echo '============================================================';
\echo '';

\echo '--- Resumen de Conceptos por Categoría ---';
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
\echo '--- Total de Conceptos Configurados ---';
SELECT COUNT(*) as total_conceptos FROM conceptos_liquidacion;

\echo '';
\echo '============================================================';
\echo '✅ Script completado exitosamente';
\echo '============================================================';
\echo '';
\echo 'Próximos pasos:';
\echo '1. Reiniciar la aplicación para que reconozca los cambios';
\echo '2. Probar los endpoints de edición de recibos';
\echo '3. Verificar que los conceptos se muestran correctamente';
\echo '';
