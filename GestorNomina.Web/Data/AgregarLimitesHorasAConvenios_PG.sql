-- ============================================================================
-- Agregar campos de límites de horas a la tabla convenios
-- ============================================================================

\echo 'Agregando campos de límites de horas a tabla convenios...';

-- Agregar hs_normales_max
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'convenios' 
        AND column_name = 'hs_normales_max'
    ) THEN
        ALTER TABLE convenios 
        ADD COLUMN hs_normales_max DECIMAL(10, 2) NULL;
        
        \echo '✓ Campo hs_normales_max agregado';
    ELSE
        \echo '○ Campo hs_normales_max ya existe';
    END IF;
END $$;

-- Agregar hs_50_max
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'convenios' 
        AND column_name = 'hs_50_max'
    ) THEN
        ALTER TABLE convenios 
        ADD COLUMN hs_50_max DECIMAL(10, 2) NULL;
        
        \echo '✓ Campo hs_50_max agregado';
    ELSE
        \echo '○ Campo hs_50_max ya existe';
    END IF;
END $$;

-- Agregar hs_100_max
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'convenios' 
        AND column_name = 'hs_100_max'
    ) THEN
        ALTER TABLE convenios 
        ADD COLUMN hs_100_max DECIMAL(10, 2) NULL;
        
        \echo '✓ Campo hs_100_max agregado';
    ELSE
        \echo '○ Campo hs_100_max ya existe';
    END IF;
END $$;

\echo '';
\echo 'Proceso completado';
\echo '';
\echo '--- Ejemplo de configuración para UOCRA ---';
\echo 'UPDATE convenios';
\echo 'SET hs_normales_max = 88,';
\echo '    hs_50_max = 24,';
\echo '    hs_100_max = 12';
\echo 'WHERE nombre = ''UOCRA'';';
