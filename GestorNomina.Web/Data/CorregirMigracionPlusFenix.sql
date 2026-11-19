-- Script de corrección para la migración problemática
-- Este script verifica y corrige el problema de la columna aplica_plus_fenix

-- Verificar si existe la columna aplica_plus_fenix
DO $$
BEGIN
    -- Si existe aplica_plus_fenix, renombrarla a aplica_plus_electrico
    IF EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'empleados' 
        AND column_name = 'aplica_plus_fenix'
    ) THEN
        ALTER TABLE empleados RENAME COLUMN aplica_plus_fenix TO aplica_plus_electrico;
        RAISE NOTICE 'Columna aplica_plus_fenix renombrada a aplica_plus_electrico';
    ELSE
        RAISE NOTICE 'La columna aplica_plus_fenix no existe, no se requiere acción';
    END IF;

    -- Verificar si ya existe aplica_plus_electrico
    IF EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'empleados' 
        AND column_name = 'aplica_plus_electrico'
    ) THEN
        RAISE NOTICE 'La columna aplica_plus_electrico ya existe correctamente';
    ELSE
        RAISE NOTICE 'ADVERTENCIA: No existe ni aplica_plus_fenix ni aplica_plus_electrico';
    END IF;
END $$;
