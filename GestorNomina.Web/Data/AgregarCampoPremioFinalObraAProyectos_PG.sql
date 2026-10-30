-- ============================================================================
-- Agregar campo premio_final_obra a la tabla proyectos
-- ============================================================================

\echo 'Agregando campo premio_final_obra a tabla proyectos...';

-- Verificar si la columna ya existe
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'proyectos' 
        AND column_name = 'premio_final_obra'
    ) THEN
        -- Agregar la columna
        ALTER TABLE proyectos 
        ADD COLUMN premio_final_obra DECIMAL(18, 2) NULL;
        
        \echo '✓ Campo premio_final_obra agregado exitosamente';
    ELSE
        \echo '○ Campo premio_final_obra ya existe';
    END IF;
END $$;

\echo '';
\echo 'Proceso completado';
