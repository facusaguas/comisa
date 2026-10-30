-- Script para agregar el campo premio_final_obra a la tabla proyectos
-- Alternativa a la migración de Entity Framework

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'proyectos' 
    AND COLUMN_NAME = 'premio_final_obra'
)
BEGIN
    -- Agregar la columna premio_final_obra
    ALTER TABLE proyectos
    ADD premio_final_obra DECIMAL(12,2) NULL;
    
    PRINT 'Campo premio_final_obra agregado exitosamente a la tabla proyectos';
END
ELSE
BEGIN
    PRINT 'El campo premio_final_obra ya existe en la tabla proyectos';
END

-- Opcional: Establecer valores iniciales para proyectos existentes
-- Descomentar las siguientes líneas si deseas establecer un valor por defecto

-- UPDATE proyectos
-- SET premio_final_obra = 0
-- WHERE premio_final_obra IS NULL;

-- PRINT 'Valores iniciales establecidos';
