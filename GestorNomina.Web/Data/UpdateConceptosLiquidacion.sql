-- Script para actualizar la tabla conceptos_liquidacion

-- Paso 1: Eliminar el índice antiguo si existe
DROP INDEX IF EXISTS "IX_conceptos_liquidacion_codigo_tango";

-- Paso 2: Renombrar la columna codigo_tango a codigo (si existe)
DO $$ 
BEGIN
    IF EXISTS(
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name='conceptos_liquidacion' AND column_name='codigo_tango'
    ) THEN
        ALTER TABLE "conceptos_liquidacion" RENAME COLUMN "codigo_tango" TO "codigo";
    END IF;
END $$;

-- Paso 3: Renombrar la columna tipo a tipo_temp temporalmente
DO $$ 
BEGIN
    IF EXISTS(
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name='conceptos_liquidacion' AND column_name='tipo'
    ) THEN
        ALTER TABLE "conceptos_liquidacion" RENAME COLUMN "tipo" TO "tipo_temp";
    END IF;
END $$;

-- Paso 4: Agregar nueva columna tipo_concepto como INTEGER
DO $$ 
BEGIN
    IF NOT EXISTS(
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name='conceptos_liquidacion' AND column_name='tipo_concepto'
    ) THEN
        ALTER TABLE "conceptos_liquidacion" ADD COLUMN "tipo_concepto" INTEGER NOT NULL DEFAULT 0;
    END IF;
END $$;

-- Paso 5: Migrar los datos de tipo_temp a tipo_concepto
-- REMUNERATIVO = 0, NO_REMUNERATIVO = 1, DEDUCCION = 2
UPDATE "conceptos_liquidacion"
SET "tipo_concepto" = CASE 
    WHEN "tipo_temp" = 'REMUNERATIVO' THEN 0
    WHEN "tipo_temp" = 'NO_REMUNERATIVO' THEN 1
    WHEN "tipo_temp" = 'DEDUCCION' THEN 2
    ELSE 0
END
WHERE EXISTS(
    SELECT 1 
    FROM information_schema.columns 
    WHERE table_name='conceptos_liquidacion' AND column_name='tipo_temp'
);

-- Paso 6: Eliminar la columna temporal tipo_temp
DO $$ 
BEGIN
    IF EXISTS(
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name='conceptos_liquidacion' AND column_name='tipo_temp'
    ) THEN
        ALTER TABLE "conceptos_liquidacion" DROP COLUMN "tipo_temp";
    END IF;
END $$;

-- Paso 7: Crear índice único en la nueva columna codigo
CREATE UNIQUE INDEX IF NOT EXISTS "IX_conceptos_liquidacion_codigo" 
ON "conceptos_liquidacion" ("codigo");

-- Verificar resultado
SELECT * FROM "conceptos_liquidacion" ORDER BY id;
