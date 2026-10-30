-- Script para agregar el concepto de Embargo (5011) a conceptos_liquidacion

-- Verificar si el concepto ya existe antes de insertar
IF NOT EXISTS (SELECT 1 FROM conceptos_liquidacion WHERE codigo = '5011')
BEGIN
    INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
    VALUES ('5011', 'Embargo', 2); -- TipoConcepto.Deduccion = 2
    
    PRINT 'Concepto 5011 - Embargo agregado exitosamente';
END
ELSE
BEGIN
    PRINT 'El concepto 5011 - Embargo ya existe';
END

PRINT 'Proceso completado';
