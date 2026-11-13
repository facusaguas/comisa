-- ============================================================================
-- Script Completo: Agregar Funcionalidad de Edición de Recibos
-- ============================================================================
-- Este script agrega todos los campos necesarios para permitir la edición
-- manual de recibos de liquidación sin correr migraciones de Entity Framework
-- ============================================================================

\echo '============================================================';
\echo 'Agregando funcionalidad de edición de recibos';
\echo '============================================================';
\echo '';

-- ============================================================================
-- PASO 1: Agregar campos a tabla conceptos_liquidacion
-- ============================================================================

\echo '--- PASO 1: Actualizando tabla conceptos_liquidacion ---';
\echo '';

-- Agregar campo es_obligatorio
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'conceptos_liquidacion' 
        AND column_name = 'es_obligatorio'
    ) THEN
        ALTER TABLE conceptos_liquidacion 
        ADD COLUMN es_obligatorio BOOLEAN NOT NULL DEFAULT false;
        
        \echo '✓ Campo es_obligatorio agregado';
    ELSE
        \echo '○ Campo es_obligatorio ya existe';
    END IF;
END $$;

-- Agregar campo es_editable
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'conceptos_liquidacion' 
        AND column_name = 'es_editable'
    ) THEN
        ALTER TABLE conceptos_liquidacion 
        ADD COLUMN es_editable BOOLEAN NOT NULL DEFAULT false;
        
        \echo '✓ Campo es_editable agregado';
    ELSE
        \echo '○ Campo es_editable ya existe';
    END IF;
END $$;

\echo '';

-- ============================================================================
-- PASO 2: Actualizar valores de conceptos existentes
-- ============================================================================

\echo '--- PASO 2: Configurando conceptos existentes ---';
\echo '';

-- OBLIGATORIOS Y NO EDITABLES
UPDATE conceptos_liquidacion SET es_obligatorio = true, es_editable = false WHERE codigo = '1004'; -- Horas Nocturnas
UPDATE conceptos_liquidacion SET es_obligatorio = true, es_editable = false WHERE codigo = '5001'; -- Jubilación 11%
UPDATE conceptos_liquidacion SET es_obligatorio = true, es_editable = false WHERE codigo = '5002'; -- Ley 19.032 3%
UPDATE conceptos_liquidacion SET es_obligatorio = true, es_editable = false WHERE codigo = '5003'; -- Obra Social 3%

\echo '✓ 4 conceptos configurados como obligatorios y no editables';

-- OBLIGATORIOS Y EDITABLES
UPDATE conceptos_liquidacion SET es_obligatorio = true, es_editable = true WHERE codigo = '1001'; -- Horas Normales

\echo '✓ 1 concepto configurado como obligatorio y editable';

-- OPCIONALES Y EDITABLES
UPDATE conceptos_liquidacion SET es_obligatorio = false, es_editable = true WHERE codigo IN (
    '1002',  -- Horas Extras 50%
    '1003',  -- Horas Extras 100%
    '1005',  -- Horas Nocturnas Extras 50%
    '1006',  -- Horas Nocturnas Extras 100%
    '1010',  -- Plus Altura 15%
    '1020',  -- Presentismo UOCRA 20%
    '1050',  -- Bonificación Especial
    '1090',  -- Plus Eléctrico 15%
    '5004',  -- Cuota Sindical 2.5%
    '5010',  -- Seguro de Vida
    '5011'   -- Embargo
);

\echo '✓ 11 conceptos configurados como opcionales y editables';

-- OPCIONALES Y NO EDITABLES (calculados automáticamente)
UPDATE conceptos_liquidacion SET es_obligatorio = false, es_editable = false WHERE codigo IN (
    '1025',  -- Adicional Montaje
    '1030',  -- Feriados / Licencias
    '1031',  -- Enfermedad / Licencias
    '1033',  -- Compensación Especial
    '1034'   -- Premio Final de Obra
);

\echo '✓ 5 conceptos configurados como opcionales y no editables';
\echo '';

-- ============================================================================
-- PASO 3: Agregar campos a tabla recibos
-- ============================================================================

\echo '--- PASO 3: Actualizando tabla recibos ---';
\echo '';

-- Agregar campo modo_edicion
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'recibos' 
        AND column_name = 'modo_edicion'
    ) THEN
        ALTER TABLE recibos 
        ADD COLUMN modo_edicion VARCHAR(20) NOT NULL DEFAULT 'AUTOMATICO';
        
        \echo '✓ Campo modo_edicion agregado';
    ELSE
        \echo '○ Campo modo_edicion ya existe';
    END IF;
END $$;

-- Agregar campo editado_por_usuario_id
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'recibos' 
        AND column_name = 'editado_por_usuario_id'
    ) THEN
        ALTER TABLE recibos 
        ADD COLUMN editado_por_usuario_id INTEGER NULL;
        
        -- Agregar foreign key a usuarios
        ALTER TABLE recibos
        ADD CONSTRAINT fk_recibos_editado_por_usuario
        FOREIGN KEY (editado_por_usuario_id) 
        REFERENCES usuarios(id)
        ON DELETE SET NULL;
        
        \echo '✓ Campo editado_por_usuario_id agregado con FK';
    ELSE
        \echo '○ Campo editado_por_usuario_id ya existe';
    END IF;
END $$;

-- Agregar campo fecha_ultima_edicion
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'recibos' 
        AND column_name = 'fecha_ultima_edicion'
    ) THEN
        ALTER TABLE recibos 
        ADD COLUMN fecha_ultima_edicion TIMESTAMP NULL;
        
        \echo '✓ Campo fecha_ultima_edicion agregado';
    ELSE
        \echo '○ Campo fecha_ultima_edicion ya existe';
    END IF;
END $$;

-- Agregar campo observaciones_edicion
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'recibos' 
        AND column_name = 'observaciones_edicion'
    ) THEN
        ALTER TABLE recibos 
        ADD COLUMN observaciones_edicion TEXT NULL;
        
        \echo '✓ Campo observaciones_edicion agregado';
    ELSE
        \echo '○ Campo observaciones_edicion ya existe';
    END IF;
END $$;

\echo '';

-- ============================================================================
-- PASO 4: Agregar campos a tabla recibo_detalles
-- ============================================================================

\echo '--- PASO 4: Actualizando tabla recibo_detalles ---';
\echo '';

-- Agregar campo editado_manualmente
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'recibo_detalles' 
        AND column_name = 'editado_manualmente'
    ) THEN
        ALTER TABLE recibo_detalles 
        ADD COLUMN editado_manualmente BOOLEAN NOT NULL DEFAULT false;
        
        \echo '✓ Campo editado_manualmente agregado';
    ELSE
        \echo '○ Campo editado_manualmente ya existe';
    END IF;
END $$;

-- Agregar campo valor_original
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'recibo_detalles' 
        AND column_name = 'valor_original'
    ) THEN
        ALTER TABLE recibo_detalles 
        ADD COLUMN valor_original NUMERIC(12, 2) NULL;
        
        \echo '✓ Campo valor_original agregado';
    ELSE
        \echo '○ Campo valor_original ya existe';
    END IF;
END $$;

\echo '';

-- ============================================================================
-- PASO 5: Crear índices para mejorar performance
-- ============================================================================

\echo '--- PASO 5: Creando índices ---';
\echo '';

-- Índice en conceptos_liquidacion para búsquedas por editabilidad
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM pg_indexes 
        WHERE tablename = 'conceptos_liquidacion' 
        AND indexname = 'idx_conceptos_liquidacion_editable'
    ) THEN
        CREATE INDEX idx_conceptos_liquidacion_editable 
        ON conceptos_liquidacion(es_editable, es_obligatorio);
        
        \echo '✓ Índice idx_conceptos_liquidacion_editable creado';
    ELSE
        \echo '○ Índice idx_conceptos_liquidacion_editable ya existe';
    END IF;
END $$;

-- Índice en recibos para búsquedas por modo de edición
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM pg_indexes 
        WHERE tablename = 'recibos' 
        AND indexname = 'idx_recibos_modo_edicion'
    ) THEN
        CREATE INDEX idx_recibos_modo_edicion 
        ON recibos(modo_edicion);
        
        \echo '✓ Índice idx_recibos_modo_edicion creado';
    ELSE
        \echo '○ Índice idx_recibos_modo_edicion ya existe';
    END IF;
END $$;

-- Índice en recibo_detalles para búsquedas por edición manual
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM pg_indexes 
        WHERE tablename = 'recibo_detalles' 
        AND indexname = 'idx_recibo_detalles_editado'
    ) THEN
        CREATE INDEX idx_recibo_detalles_editado 
        ON recibo_detalles(editado_manualmente);
        
        \echo '✓ Índice idx_recibo_detalles_editado creado';
    ELSE
        \echo '○ Índice idx_recibo_detalles_editado ya existe';
    END IF;
END $$;

\echo '';

-- ============================================================================
-- PASO 6: Verificación final
-- ============================================================================

\echo '============================================================';
\echo 'Verificación de cambios realizados';
\echo '============================================================';
\echo '';

\echo '--- Tabla conceptos_liquidacion ---';
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'conceptos_liquidacion'
AND column_name IN ('es_obligatorio', 'es_editable')
ORDER BY column_name;

\echo '';
\echo '--- Resumen de configuración de conceptos ---';
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
\echo '--- Tabla recibos ---';
SELECT 
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'recibos'
AND column_name IN ('modo_edicion', 'editado_por_usuario_id', 'fecha_ultima_edicion', 'observaciones_edicion')
ORDER BY column_name;

\echo '';
\echo '--- Tabla recibo_detalles ---';
SELECT 
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'recibo_detalles'
AND column_name IN ('editado_manualmente', 'valor_original')
ORDER BY column_name;

\echo '';
\echo '--- Índices creados ---';
SELECT 
    indexname,
    tablename
FROM pg_indexes
WHERE tablename IN ('conceptos_liquidacion', 'recibos', 'recibo_detalles')
AND indexname LIKE 'idx_%edita%' OR indexname LIKE 'idx_%modo%'
ORDER BY tablename, indexname;

\echo '';
\echo '============================================================';
\echo 'Script completado exitosamente';
\echo '============================================================';
\echo '';
\echo 'Próximos pasos:';
\echo '1. Verificar que todos los campos fueron agregados correctamente';
\echo '2. Verificar la configuración de conceptos editables';
\echo '3. Reiniciar la aplicación para que EF reconozca los nuevos campos';
\echo '';
\echo 'NOTA: Los campos agregados son compatibles con el modelo de Entity Framework';
\echo 'actualizado en la aplicación. No es necesario ejecutar migraciones.';
