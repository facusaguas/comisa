-- Script para agregar nuevos conceptos de liquidación
-- Ejecutar después de aplicar las migraciones

-- Verificar si los conceptos ya existen antes de insertar
IF NOT EXISTS (SELECT 1 FROM conceptos_liquidacion WHERE codigo = '1031')
BEGIN
    INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
    VALUES ('1031', 'Enfermedad / Licencias', 1); -- TipoConcepto.Remunerativo = 1
    PRINT 'Concepto 1031 - Enfermedad / Licencias agregado';
END
ELSE
BEGIN
    PRINT 'Concepto 1031 ya existe';
END

IF NOT EXISTS (SELECT 1 FROM conceptos_liquidacion WHERE codigo = '1033')
BEGIN
    INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
    VALUES ('1033', 'Compensación Especial', 1); -- TipoConcepto.Remunerativo = 1
    PRINT 'Concepto 1033 - Compensación Especial agregado';
END
ELSE
BEGIN
    PRINT 'Concepto 1033 ya existe';
END

IF NOT EXISTS (SELECT 1 FROM conceptos_liquidacion WHERE codigo = '1034')
BEGIN
    INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
    VALUES ('1034', 'Premio Final de Obra', 3); -- TipoConcepto.NoRemunerativo = 3
    PRINT 'Concepto 1034 - Premio Final de Obra agregado';
END
ELSE
BEGIN
    PRINT 'Concepto 1034 ya existe';
END

IF NOT EXISTS (SELECT 1 FROM conceptos_liquidacion WHERE codigo = '1090')
BEGIN
    INSERT INTO conceptos_liquidacion (codigo, nombre, tipo_concepto)
    VALUES ('1090', 'Plus Eléctrico 15%', 1); -- TipoConcepto.Remunerativo = 1
    PRINT 'Concepto 1090 - Plus Eléctrico 15% agregado';
END
ELSE
BEGIN
    PRINT 'Concepto 1090 ya existe';
END

PRINT 'Proceso completado';
