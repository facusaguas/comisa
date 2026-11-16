-- Script para optimizar la búsqueda de novedades duplicadas
-- Este índice mejora el rendimiento de la verificación durante la importación de Excel

-- Crear índice compuesto en la tabla novedades
-- para optimizar la búsqueda por empleado_id + fecha_novedad + tipo_novedad
CREATE INDEX IF NOT EXISTS idx_novedades_duplicados 
ON novedades (empleado_id, fecha_novedad, tipo_novedad);

-- Verificar que el índice se creó correctamente
SELECT 
    schemaname,
    tablename,
    indexname,
    indexdef
FROM pg_indexes 
WHERE tablename = 'novedades' 
  AND indexname = 'idx_novedades_duplicados';

-- Comentario: Este índice es usado por la query de verificación de duplicados
-- en ExcelParteDiarioService.cs cuando se importan novedades desde Excel
