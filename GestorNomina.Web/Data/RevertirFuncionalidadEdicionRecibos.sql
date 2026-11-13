-- ============================================================================
-- Script de Rollback: Revertir Funcionalidad de Edición de Recibos
-- ============================================================================
-- Este script revierte todos los cambios realizados por el script
-- AgregarFuncionalidadEdicionRecibos.sql
-- ============================================================================
-- ⚠️ ADVERTENCIA: Este script eliminará los campos de edición.
-- Si ya tienes datos en estos campos, se perderán.
-- ============================================================================

\echo '============================================================';
\echo '⚠️  REVERTIR funcionalidad de edición de recibos';
\echo '============================================================';
\echo '';
\echo '¿Está seguro de continuar? Presione Ctrl+C para cancelar';
\echo '';

-- Esperar 5 segundos para dar tiempo a cancelar
SELECT pg_sleep(5);

\echo 'Iniciando rollback...';
\echo '';

-- ============================================================================
-- PASO 1: Eliminar índices
-- ============================================================================

\echo '--- PASO 1: Eliminando índices ---';
\echo '';

DROP INDEX IF EXISTS idx_conceptos_liquidacion_editable;
\echo '✓ Índice idx_conceptos_liquidacion_editable eliminado';

DROP INDEX IF EXISTS idx_recibos_modo_edicion;
\echo '✓ Índice idx_recibos_modo_edicion eliminado';

DROP INDEX IF EXISTS idx_recibo_detalles_editado;
\echo '✓ Índice idx_recibo_detalles_editado eliminado';

\echo '';

-- ============================================================================
-- PASO 2: Eliminar campos de tabla recibo_detalles
-- ============================================================================

\echo '--- PASO 2: Revertiendo tabla recibo_detalles ---';
\echo '';

ALTER TABLE recibo_detalles DROP COLUMN IF EXISTS valor_original;
\echo '✓ Campo valor_original eliminado';

ALTER TABLE recibo_detalles DROP COLUMN IF EXISTS editado_manualmente;
\echo '✓ Campo editado_manualmente eliminado';

\echo '';

-- ============================================================================
-- PASO 3: Eliminar campos de tabla recibos
-- ============================================================================

\echo '--- PASO 3: Revertiendo tabla recibos ---';
\echo '';

-- Primero eliminar el FK antes de eliminar la columna
ALTER TABLE recibos DROP CONSTRAINT IF EXISTS fk_recibos_editado_por_usuario;
\echo '✓ Foreign key fk_recibos_editado_por_usuario eliminado';

ALTER TABLE recibos DROP COLUMN IF EXISTS observaciones_edicion;
\echo '✓ Campo observaciones_edicion eliminado';

ALTER TABLE recibos DROP COLUMN IF EXISTS fecha_ultima_edicion;
\echo '✓ Campo fecha_ultima_edicion eliminado';

ALTER TABLE recibos DROP COLUMN IF EXISTS editado_por_usuario_id;
\echo '✓ Campo editado_por_usuario_id eliminado';

ALTER TABLE recibos DROP COLUMN IF EXISTS modo_edicion;
\echo '✓ Campo modo_edicion eliminado';

\echo '';

-- ============================================================================
-- PASO 4: Eliminar campos de tabla conceptos_liquidacion
-- ============================================================================

\echo '--- PASO 4: Revertiendo tabla conceptos_liquidacion ---';
\echo '';

ALTER TABLE conceptos_liquidacion DROP COLUMN IF EXISTS es_editable;
\echo '✓ Campo es_editable eliminado';

ALTER TABLE conceptos_liquidacion DROP COLUMN IF EXISTS es_obligatorio;
\echo '✓ Campo es_obligatorio eliminado';

\echo '';

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================

\echo '============================================================';
\echo 'Verificación de rollback';
\echo '============================================================';
\echo '';

\echo '--- Campos eliminados de conceptos_liquidacion ---';
SELECT COUNT(*) as campos_restantes
FROM information_schema.columns
WHERE table_name = 'conceptos_liquidacion'
AND column_name IN ('es_obligatorio', 'es_editable');

\echo '';
\echo '--- Campos eliminados de recibos ---';
SELECT COUNT(*) as campos_restantes
FROM information_schema.columns
WHERE table_name = 'recibos'
AND column_name IN ('modo_edicion', 'editado_por_usuario_id', 'fecha_ultima_edicion', 'observaciones_edicion');

\echo '';
\echo '--- Campos eliminados de recibo_detalles ---';
SELECT COUNT(*) as campos_restantes
FROM information_schema.columns
WHERE table_name = 'recibo_detalles'
AND column_name IN ('editado_manualmente', 'valor_original');

\echo '';
\echo '============================================================';
\echo 'Rollback completado';
\echo '============================================================';
\echo '';
\echo 'Si COUNT = 0 en las verificaciones anteriores, el rollback fue exitoso.';
\echo '';
