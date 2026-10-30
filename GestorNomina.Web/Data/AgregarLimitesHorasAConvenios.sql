-- Script para agregar campos de límites de horas a la tabla convenios
-- Estos límites se usan para ajustar horas en el proceso de neto convenido

-- 1. Agregar campo hs_normales_max
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'convenios' 
    AND COLUMN_NAME = 'hs_normales_max'
)
BEGIN
    ALTER TABLE convenios
    ADD hs_normales_max DECIMAL(10,2) NULL;
    PRINT 'Campo hs_normales_max agregado a la tabla convenios';
END
ELSE
BEGIN
    PRINT 'El campo hs_normales_max ya existe en convenios';
END

-- 2. Agregar campo hs_50_max
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'convenios' 
    AND COLUMN_NAME = 'hs_50_max'
)
BEGIN
    ALTER TABLE convenios
    ADD hs_50_max DECIMAL(10,2) NULL;
    PRINT 'Campo hs_50_max agregado a la tabla convenios';
END
ELSE
BEGIN
    PRINT 'El campo hs_50_max ya existe en convenios';
END

-- 3. Agregar campo hs_100_max
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'convenios' 
    AND COLUMN_NAME = 'hs_100_max'
)
BEGIN
    ALTER TABLE convenios
    ADD hs_100_max DECIMAL(10,2) NULL;
    PRINT 'Campo hs_100_max agregado a la tabla convenios';
END
ELSE
BEGIN
    PRINT 'El campo hs_100_max ya existe en convenios';
END

PRINT '';
PRINT '========================================';
PRINT 'Configuración de límites de horas por convenio';
PRINT '========================================';
PRINT '';
PRINT 'Ejemplo para configurar límites UOCRA (por quincena):';
PRINT '';
PRINT 'UPDATE convenios';
PRINT 'SET hs_normales_max = 88,      -- Máximo de horas normales en una quincena';
PRINT '    hs_50_max = 24,            -- Máximo de horas extras 50% en una quincena';
PRINT '    hs_100_max = 12            -- Máximo de horas extras 100% en una quincena';
PRINT 'WHERE codigo = ''UOCRA'';';
PRINT '';
PRINT '========================================';

-- Opcional: Configurar valores por defecto para UOCRA
-- Descomentar las siguientes líneas para establecer valores iniciales

/*
UPDATE convenios
SET hs_normales_max = 88,    -- 11 días * 8 horas
    hs_50_max = 24,          -- Máximo de extras 50%
    hs_100_max = 12          -- Máximo de extras 100%
WHERE codigo = 'UOCRA';

PRINT 'Límites configurados para convenio UOCRA';
*/
