-- Script de Datos Iniciales para Gestor de Nómina
-- Ejecutar después de aplicar las migraciones

-- ==============================================
-- 1. ROLES
-- ==============================================
INSERT INTO roles (nombre, descripcion, created_at, updated_at) VALUES
('Administrador', 'Acceso completo al sistema', NOW(), NOW()),
('Supervisor', 'Gestión de proyectos y validaciones', NOW(), NOW()),
('Operador', 'Carga de planillas Excel', NOW(), NOW()),
('Consulta', 'Solo lectura de reportes', NOW(), NOW());

-- ==============================================
-- 2. REGÍMENES LABORALES
-- ==============================================
INSERT INTO regimenes_laborales (nombre, dias_trabajo, dias_descanso, max_horas_diarias, horas_semanales_limite, descripcion) VALUES
('14x14', 14, 14, 12.0, 84.0, 'Régimen minero: 14 días de trabajo y 14 de descanso'),
('7x7', 7, 7, 12.0, 48.0, 'Régimen minero: 7 días de trabajo y 7 de descanso'),
('20x10', 20, 10, 10.0, 60.0, 'Régimen especial: 20 días de trabajo y 10 de descanso'),
('6x1', 6, 1, 8.0, 48.0, 'Régimen semanal estándar con un día de descanso'),
('5x2', 5, 2, 8.0, 40.0, 'Régimen estándar de oficina');

-- ==============================================
-- 3. ACTIVIDADES
-- ==============================================
INSERT INTO actividades (codigo, descripcion, es_hora_efectiva, es_licencia, es_franco) VALUES
-- Horas efectivas
('HN', 'Hora Normal', true, false, false),
('HE50', 'Hora Extra 50%', true, false, false),
('HE100', 'Hora Extra 100%', true, false, false),
('HN_FERIADO', 'Hora Normal en Feriado', true, false, false),

-- Licencias
('LIC_MEDICA', 'Licencia Médica', false, true, false),
('LIC_ANUAL', 'Licencia Anual', false, true, false),
('LIC_ESTUDIO', 'Licencia por Estudio', false, true, false),
('LIC_MATERNIDAD', 'Licencia por Maternidad', false, true, false),

-- Francos y ausencias
('FRANCO', 'Franco Compensatorio', false, false, true),
('AUSENTE', 'Ausente sin aviso', false, false, false),
('SUSPENSION', 'Suspensión', false, false, false),

-- Capacitación
('CAPACITACION', 'Capacitación', true, false, false);

-- ==============================================
-- 4. REGLAS NORMATIVAS
-- ==============================================
INSERT INTO reglas_normativas (codigo, nombre, descripcion, nivel_severidad, activa) VALUES
('RN001', 'Límite de horas diarias', 'Verifica que no se excedan las horas máximas permitidas por día según el régimen', 'CRITICO', true),
('RN002', 'Descanso interjornada', 'Valida que exista un descanso mínimo de 12 horas entre jornadas', 'CRITICO', true),
('RN003', 'Límite semanal', 'Controla que no se superen las horas semanales establecidas', 'ALTO', true),
('RN004', 'Días consecutivos', 'Verifica que no se trabajen más días consecutivos que los permitidos por el régimen', 'ALTO', true),
('RN005', 'Horas en feriado', 'Controla que las horas en feriados estén correctamente codificadas', 'MEDIO', true),
('RN006', 'Francos compensatorios', 'Valida que los francos compensatorios coincidan con los días trabajados', 'MEDIO', true),
('RN007', 'Licencias médicas', 'Verifica que las licencias médicas no se superpongan con horas trabajadas', 'ALTO', true),
('RN008', 'Horas negativas', 'Detecta registros con valores negativos o cero en horas', 'CRITICO', true),
('RN009', 'Duplicados', 'Identifica registros duplicados para el mismo empleado en la misma fecha', 'CRITICO', true),
('RN010', 'Empleado inactivo', 'Alerta si hay horas cargadas para empleados dados de baja', 'ALTO', true);

-- ==============================================
-- 5. FERIADOS (Ejemplo Argentina 2024)
-- ==============================================
INSERT INTO feriados (fecha, descripcion, tipo, jurisdiccion) VALUES
('2024-01-01', 'Año Nuevo', 'Nacional', 'Argentina'),
('2024-02-12', 'Carnaval', 'Nacional', 'Argentina'),
('2024-02-13', 'Carnaval', 'Nacional', 'Argentina'),
('2024-03-24', 'Día Nacional de la Memoria', 'Nacional', 'Argentina'),
('2024-03-29', 'Viernes Santo', 'Nacional', 'Argentina'),
('2024-04-02', 'Día del Veterano', 'Nacional', 'Argentina'),
('2024-05-01', 'Día del Trabajador', 'Nacional', 'Argentina'),
('2024-05-25', 'Revolución de Mayo', 'Nacional', 'Argentina'),
('2024-06-20', 'Día de la Bandera', 'Nacional', 'Argentina'),
('2024-07-09', 'Día de la Independencia', 'Nacional', 'Argentina'),
('2024-08-17', 'Gral. San Martín', 'Nacional', 'Argentina'),
('2024-10-12', 'Día del Respeto', 'Nacional', 'Argentina'),
('2024-11-18', 'Día de la Soberanía', 'Nacional', 'Argentina'),
('2024-12-08', 'Inmaculada Concepción', 'Nacional', 'Argentina'),
('2024-12-25', 'Navidad', 'Nacional', 'Argentina');

-- ==============================================
-- 6. USUARIO ADMINISTRADOR INICIAL
-- ==============================================
-- Email: admin@gestornomina.com
-- Contraseña: Admin123!
-- Hash BCrypt (work factor 11)
INSERT INTO usuarios (rol_id, email, password_hash, nombre, apellido, activo, created_at, updated_at) VALUES
(1, 'admin@gestornomina.com', '$2a$11$3DqN5h5VqKnH.yZQZqOhveLKxQVNZN3wPZN5h5VqKnH.yZQZqOhve', 'Administrador', 'Sistema', true, NOW(), NOW());

-- ==============================================
-- CONSULTAS DE VERIFICACIÓN
-- ==============================================

-- Verificar roles creados
-- SELECT * FROM roles;

-- Verificar regímenes laborales
-- SELECT * FROM regimenes_laborales;

-- Verificar actividades
-- SELECT * FROM actividades;

-- Verificar reglas normativas
-- SELECT * FROM reglas_normativas WHERE activa = true;

-- Verificar feriados del año actual
-- SELECT * FROM feriados WHERE EXTRACT(YEAR FROM fecha) = EXTRACT(YEAR FROM CURRENT_DATE) ORDER BY fecha;
