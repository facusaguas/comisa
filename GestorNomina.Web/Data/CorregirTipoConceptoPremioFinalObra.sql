-- Script para corregir el tipo del concepto Premio Final de Obra
-- De Remunerativo (1) a No Remunerativo (3)

-- Verificar si el concepto existe y corregir su tipo
IF EXISTS (SELECT 1 FROM conceptos_liquidacion WHERE codigo = '1034')
BEGIN
    UPDATE conceptos_liquidacion
    SET tipo_concepto = 3  -- TipoConcepto.NoRemunerativo = 3
    WHERE codigo = '1034';
    
    PRINT 'Concepto 1034 - Premio Final de Obra actualizado a No Remunerativo';
END
ELSE
BEGIN
    -- Si no existe, insertarlo directamente como No Remunerativo
    INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
    VALUES ('1034', 'Premio Final de Obra', 3);
    
    PRINT 'Concepto 1034 - Premio Final de Obra agregado como No Remunerativo';
END

PRINT 'Proceso completado';
