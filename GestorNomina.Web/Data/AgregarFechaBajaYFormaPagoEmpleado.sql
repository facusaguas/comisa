-- =============================================
-- Script para agregar campos:
-- - fecha_baja
-- - forma_pago (enum: 1=Quincenal, 2=Mensual)
-- - Modificar activo a nullable
-- PostgreSQL
-- =============================================

-- =============================================
-- 1. AGREGAR COLUMNA fecha_baja
-- =============================================

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'empleados' 
        AND column_name = 'fecha_baja'
    ) THEN
        ALTER TABLE empleados ADD COLUMN fecha_baja TIMESTAMP;
        RAISE NOTICE 'Columna fecha_baja agregada a empleados';
    ELSE
        RAISE NOTICE 'La columna fecha_baja ya existe en empleados';
    END IF;
END $$;

-- =============================================
-- 2. AGREGAR COLUMNA forma_pago (INTEGER)
-- =============================================

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'empleados' 
        AND column_name = 'forma_pago'
    ) THEN
        -- Agregar columna con valor por defecto 1 (Quincenal)
        ALTER TABLE empleados ADD COLUMN forma_pago INTEGER NOT NULL DEFAULT 1;
        RAISE NOTICE 'Columna forma_pago agregada a empleados con valor por defecto 1 (Quincenal)';
    ELSE
        RAISE NOTICE 'La columna forma_pago ya existe en empleados';
    END IF;
END $$;

-- =============================================
-- 3. MODIFICAR COLUMNA activo A NULLABLE
-- =============================================

-- Verificar tipo actual de la columna activo
DO $$
DECLARE
    is_nullable text;
BEGIN
    SELECT is_nullable INTO is_nullable
    FROM information_schema.columns
    WHERE table_name = 'empleados' 
    AND column_name = 'activo';
    
    IF is_nullable = 'NO' THEN
        -- La columna no es nullable, la modificamos
        ALTER TABLE empleados ALTER COLUMN activo DROP NOT NULL;
        RAISE NOTICE 'Columna activo modificada a nullable';
    ELSE
        RAISE NOTICE 'La columna activo ya es nullable';
    END IF;
END $$;

-- =============================================
-- 4. ACTUALIZAR VALORES EXISTENTES
-- =============================================

-- Asegurar que todos los empleados existentes tengan forma_pago = 1 (Quincenal)
UPDATE empleados 
SET forma_pago = 1 
WHERE forma_pago IS NULL;

-- Los empleados existentes mantienen su estado activo actual (true/false)
-- No modificamos el campo activo para datos existentes

RAISE NOTICE 'Valores existentes actualizados';

-- =============================================
-- 5. AGREGAR COMENTARIOS A LAS COLUMNAS
-- =============================================

COMMENT ON COLUMN empleados.fecha_baja IS 'Fecha en que el empleado causó baja';
COMMENT ON COLUMN empleados.forma_pago IS 'Forma de pago: 1=Quincenal (default), 2=Mensual';
COMMENT ON COLUMN empleados.activo IS 'Estado del empleado: true=Activo, false=Inactivo, null=No definido';

-- =============================================
-- 6. CREAR CONSTRAINT PARA VALIDAR forma_pago
-- =============================================

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM pg_constraint 
        WHERE conname = 'chk_empleados_forma_pago'
    ) THEN
        ALTER TABLE empleados 
        ADD CONSTRAINT chk_empleados_forma_pago 
        CHECK (forma_pago IN (1, 2));
        RAISE NOTICE 'Constraint chk_empleados_forma_pago creado (valores permitidos: 1 o 2)';
    ELSE
        RAISE NOTICE 'Constraint chk_empleados_forma_pago ya existe';
    END IF;
END $$;

-- =============================================
-- 7. VERIFICAR ESTRUCTURA ACTUALIZADA
-- =============================================

-- Mostrar información de las columnas modificadas/agregadas
SELECT 
    column_name,
    data_type,
    is_nullable,
    column_default,
    character_maximum_length
FROM information_schema.columns
WHERE table_name = 'empleados'
AND column_name IN ('fecha_baja', 'forma_pago', 'activo')
ORDER BY ordinal_position;

-- =============================================
-- 8. EJEMPLOS DE USO
-- =============================================

-- Ejemplo: Dar de baja a un empleado
-- UPDATE empleados 
-- SET fecha_baja = NOW(), activo = false 
-- WHERE id = 1;

-- Ejemplo: Cambiar forma de pago a Mensual
-- UPDATE empleados 
-- SET forma_pago = 2 
-- WHERE id = 1;

-- Ejemplo: Consultar empleados activos con pago quincenal
-- SELECT 
--     legajo,
--     nombre,
--     apellido,
--     CASE forma_pago 
--         WHEN 1 THEN 'Quincenal'
--         WHEN 2 THEN 'Mensual'
--     END as forma_pago_texto,
--     CASE 
--         WHEN activo = true THEN 'Activo'
--         WHEN activo = false THEN 'Inactivo'
--         ELSE 'No definido'
--     END as estado
-- FROM empleados
-- WHERE activo = true AND forma_pago = 1;

-- =============================================
-- 9. MENSAJE FINAL
-- =============================================

DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Script ejecutado exitosamente';
    RAISE NOTICE 'Columnas agregadas/modificadas:';
    RAISE NOTICE '  - fecha_baja (TIMESTAMP, nullable)';
    RAISE NOTICE '  - forma_pago (INTEGER, NOT NULL, default 1)';
    RAISE NOTICE '  - activo (BOOLEAN, nullable)';
    RAISE NOTICE '';
    RAISE NOTICE 'Valores de forma_pago:';
    RAISE NOTICE '  1 = Quincenal (por defecto)';
    RAISE NOTICE '  2 = Mensual';
    RAISE NOTICE '========================================';
END $$;
