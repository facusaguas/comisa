-- ============================================================
-- Script para renombrar columna aplica_plus_fenix a aplica_plus_electrico
-- Tabla: empleados
-- ============================================================

-- Renombrar la columna
ALTER TABLE empleados 
RENAME COLUMN aplica_plus_fenix TO aplica_plus_electrico;

-- Verificar el cambio
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'empleados' 
  AND column_name = 'aplica_plus_electrico';

-- Mensaje de confirmación
DO $$
BEGIN
    RAISE NOTICE 'Columna renombrada exitosamente: aplica_plus_fenix -> aplica_plus_electrico';
END $$;
