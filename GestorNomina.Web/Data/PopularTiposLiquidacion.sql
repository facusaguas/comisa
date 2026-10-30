-- ============================================================
-- Script para popular Tipos de Liquidación
-- ============================================================

-- Verificar si ya existen datos
DO $$
DECLARE
    total_registros INT;
BEGIN
    SELECT COUNT(*) INTO total_registros FROM tipos_liquidacion;
    RAISE NOTICE 'Registros actuales en tipos_liquidacion: %', total_registros;
END $$;

-- Insertar o actualizar tipos de liquidación
INSERT INTO tipos_liquidacion (codigo, nombre) VALUES
    ('DIRECTOS', 'Directos'),
    ('DIRECTOS_CONVERSION_ACUERDO', 'Directos con Conversión / Acuerdo'),
    ('INDIRECTO', 'Indirecto')
ON CONFLICT (codigo) DO UPDATE 
SET nombre = EXCLUDED.nombre;

-- Verificar resultado
SELECT 
    id, 
    codigo, 
    nombre,
    'INSERTADO/ACTUALIZADO' as estado
FROM tipos_liquidacion
ORDER BY id;

-- Mostrar resumen
DO $$
DECLARE
    total_registros INT;
BEGIN
    SELECT COUNT(*) INTO total_registros FROM tipos_liquidacion;
    RAISE NOTICE '==============================================';
    RAISE NOTICE 'Total de Tipos de Liquidación: %', total_registros;
    RAISE NOTICE '==============================================';
END $$;
