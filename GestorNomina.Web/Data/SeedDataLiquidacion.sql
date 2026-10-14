-- ==========================================
-- SEED DATA PARA MÓDULO DE LIQUIDACIÓN
-- ==========================================

-- CONVENIOS / MODALIDADES DE PAGO
INSERT INTO convenios (codigo, nombre, activo) VALUES
('UOCRA', 'Unión Obrera de la Construcción', TRUE),
('AOMA', 'Asociación Obrera Minera Argentina', TRUE),
('MENSUAL', 'Empleados Mensualizados', TRUE)
ON CONFLICT (codigo) DO NOTHING;

-- TIPOS DE LIQUIDACIÓN
INSERT INTO tipos_liquidacion (codigo, nombre) VALUES
('DIRECTOS', 'Directos'),
('DIRECTOS_CONVERSION_ACUERDO', 'Directos con Conversión / Acuerdo'),
('INDIRECTO', 'Indirecto')
ON CONFLICT (codigo) DO NOTHING;

-- CATEGORÍAS PROFESIONALES (UOCRA)
INSERT INTO categorias (codigo, nombre, convenio_id) VALUES
('AYUDANTE', 'Ayudante', (SELECT id FROM convenios WHERE codigo = 'UOCRA')),
('MEDIO_OFICIAL', 'Medio Oficial', (SELECT id FROM convenios WHERE codigo = 'UOCRA')),
('OFICIAL', 'Oficial', (SELECT id FROM convenios WHERE codigo = 'UOCRA')),
('OFICIAL_ESP', 'Oficial Especializado', (SELECT id FROM convenios WHERE codigo = 'UOCRA')),
('CAPATAZ', 'Capataz', (SELECT id FROM convenios WHERE codigo = 'UOCRA')),
('SUPERVISOR', 'Supervisor', (SELECT id FROM convenios WHERE codigo = 'UOCRA'))
ON CONFLICT (codigo) DO NOTHING;

-- DISCIPLINAS / ESPECIALIDADES
INSERT INTO disciplinas (codigo, nombre) VALUES
('OPERADOR', 'Operador'),
('MECANICO', 'Mecánico'),
('MONTADOR', 'Montador'),
('PREVENCIONISTA', 'Prevencionista'),
('ELECTRICISTA', 'Electricista'),
('SOLDADOR', 'Soldador'),
('CARPINTERO', 'Carpintero'),
('PINTOR', 'Pintor'),
('ALBAÑIL', 'Albañil')
ON CONFLICT (codigo) DO NOTHING;

-- NOTA: Los proyectos ya existen en la tabla 'proyectos'
-- No es necesario insertar obras adicionales

-- TIPOS DE NOVEDAD DEL PARTE DIARIO
INSERT INTO tipos_novedad (codigo, nombre, requiere_horas, color_hex_default) VALUES
('NORMAL', 'Normal', TRUE, NULL),
('ENFERMEDAD', 'Enfermedad', FALSE, '#FFFF00'),
('FALTA_INJUSTIFICADA', 'Falta Injustificada', FALSE, '#FF0000'),
('VACACIONES', 'Vacaciones', FALSE, NULL),
('FERIADO', 'Feriado', TRUE, NULL),
('ACCIDENTE', 'Accidente', FALSE, NULL),
('ROSTER', 'Roster / Franco', FALSE, '#00FF00')
ON CONFLICT (codigo) DO NOTHING;

-- CONCEPTOS DE LIQUIDACIÓN (TANGO)
INSERT INTO conceptos_liquidacion (codigo_tango, nombre, tipo) VALUES
-- CONCEPTOS REMUNERATIVOS
('1001', 'Horas Normales', 'REMUNERATIVO'),
('1002', 'Horas Extras 50%', 'REMUNERATIVO'),
('1003', 'Horas Extras 100%', 'REMUNERATIVO'),
('1004', 'Horas Nocturnas', 'REMUNERATIVO'),
('1005', 'Horas Nocturnas Extras 50%', 'REMUNERATIVO'),
('1006', 'Horas Nocturnas Extras 100%', 'REMUNERATIVO'),
('1010', 'Plus Altura 15%', 'REMUNERATIVO'),
('1020', 'Presentismo UOCRA 20%', 'REMUNERATIVO'),
('1025', 'Adicional Montaje / Fénix 25%', 'REMUNERATIVO'),
('1030', 'Feriados / Licencias', 'REMUNERATIVO'),
('1050', 'Bonificación Especial - Ajuste Neto Convenido', 'REMUNERATIVO'),

-- DEDUCCIONES
('5001', 'Jubilación 11%', 'DEDUCCION'),
('5002', 'Ley 19.032 3%', 'DEDUCCION'),
('5003', 'Obra Social 3%', 'DEDUCCION'),
('5004', 'Cuota Sindical 2.5%', 'DEDUCCION'),
('5010', 'Seguro de Vida Obligatorio', 'DEDUCCION')
ON CONFLICT (codigo_tango) DO NOTHING;

-- ==========================================
-- EMPLEADOS DE EJEMPLO PARA LIQUIDACIÓN
-- ==========================================

-- Nota: Estos son ejemplos. Ajustar según los empleados reales.
-- Los campos de liquidación ahora están directamente en la tabla 'empleados'

-- Ejemplo de actualización de empleado existente con datos de liquidación
-- Descomentar y ajustar según sea necesario:

/*
UPDATE empleados SET
    cuil = '20-12345678-9',
    fecha_alta = '2024-01-01',
    convenio_id = (SELECT id FROM convenios WHERE codigo = 'UOCRA'),
    tipo_liquidacion_id = (SELECT id FROM tipos_liquidacion WHERE codigo = 'DIRECTOS_CONVERSION_ACUERDO'),
    categoria_id = (SELECT id FROM categorias WHERE codigo = 'OFICIAL_ESP'),
    disciplina_id = (SELECT id FROM disciplinas WHERE codigo = 'MONTADOR'),
    proyecto_id = (SELECT id FROM proyectos WHERE codigo = 'TU_CODIGO_PROYECTO'),
    valor_hora_basico = 7561.00,
    valor_convenido_diario = 150000.00,
    aplica_plus_altura = TRUE,
    aplica_plus_fenix = TRUE
WHERE legajo = '1207';
*/

-- Ejemplo de inserción de nuevo empleado con datos de liquidación
/*
INSERT INTO empleados (
    legajo, apellido, nombre, documento_identidad, cuil, cargo,
    regimen_id, fecha_ingreso, fecha_alta, activo,
    convenio_id, tipo_liquidacion_id, categoria_id, disciplina_id, proyecto_id,
    valor_hora_basico, valor_convenido_diario,
    aplica_plus_altura, aplica_plus_fenix
) VALUES (
    '1207', 'PEREZ', 'JUAN', '12345678', '20-12345678-9', 'Montador',
    NULL, '2024-01-01', '2024-01-01', TRUE,
    (SELECT id FROM convenios WHERE codigo = 'UOCRA'),
    (SELECT id FROM tipos_liquidacion WHERE codigo = 'DIRECTOS_CONVERSION_ACUERDO'),
    (SELECT id FROM categorias WHERE codigo = 'OFICIAL_ESP'),
    (SELECT id FROM disciplinas WHERE codigo = 'MONTADOR'),
    (SELECT id FROM proyectos WHERE codigo = 'TU_CODIGO_PROYECTO'),
    7561.00, 150000.00, TRUE, TRUE
)
ON CONFLICT (legajo) DO NOTHING;
*/

-- ==========================================
-- VERIFICACIÓN DE DATOS INSERTADOS
-- ==========================================

SELECT 'Convenios insertados: ' || COUNT(*) FROM convenios;
SELECT 'Tipos de Liquidación insertados: ' || COUNT(*) FROM tipos_liquidacion;
SELECT 'Categorías insertadas: ' || COUNT(*) FROM categorias;
SELECT 'Disciplinas insertadas: ' || COUNT(*) FROM disciplinas;
SELECT 'Tipos de Novedad insertados: ' || COUNT(*) FROM tipos_novedad;
SELECT 'Conceptos de Liquidación insertados: ' || COUNT(*) FROM conceptos_liquidacion;
SELECT 'Proyectos existentes: ' || COUNT(*) FROM proyectos;
