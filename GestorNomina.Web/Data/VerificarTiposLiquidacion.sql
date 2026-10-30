-- Script para verificar datos en tipos_liquidacion

-- Ver todos los tipos de liquidación
SELECT * FROM tipos_liquidacion ORDER BY id;

-- Contar tipos de liquidación
SELECT COUNT(*) as total_tipos FROM tipos_liquidacion;

-- Ver estructura de la tabla
SELECT 
    column_name, 
    data_type, 
    character_maximum_length,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'tipos_liquidacion'
ORDER BY ordinal_position;
