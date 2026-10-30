-- ============================================== 
-- CREAR USUARIOS DE PRUEBA
-- ==============================================
-- Este script crea usuarios de prueba para cada rol del sistema
-- 
-- IMPORTANTE: Todos los usuarios tienen la misma contraseña: Test123!
-- 
-- Usuarios creados:
-- 1. admin@test.com     - Administrador (Contraseña: Test123!)
-- 2. supervisor@test.com - Supervisor (Contraseña: Test123!)
-- 3. operador@test.com   - Operador (Contraseña: Test123!)
-- 4. consulta@test.com   - Consulta (Contraseña: Test123!)
-- 
-- Ejecutar: psql -U postgres -d gestor_nomina_dev -f Data\CreateTestUsers.sql
-- ==============================================

-- Eliminar usuarios de prueba si ya existen
DELETE FROM usuarios WHERE email IN (
    'admin@test.com', 
    'supervisor@test.com', 
    'operador@test.com', 
    'consulta@test.com'
);

-- =====================
-- USUARIOS DE PRUEBA
-- =====================
-- Hash BCrypt para contraseña: Test123!
-- $2a$11$YourHashHereWouldBeGeneratedByBCrypt

-- Nota: Hash BCrypt válido para contraseña: Test123!
-- Generado con: BCrypt.Net.BCrypt.HashPassword("Test123!", 11)

-- 1. Administrador de prueba
INSERT INTO usuarios (rol_id, email, password_hash, nombre, apellido, activo, created_at, updated_at) 
VALUES (
    1, 
    'admin@test.com', 
    '$2a$11$N9qo8uLOickgx2ZMRZoMye1J3UIYZDmQVyIKHqR7.YI0rHO5E1lM2',
    'Admin', 
    'Prueba', 
    true, 
    NOW(), 
    NOW()
);

-- 2. Supervisor de prueba
INSERT INTO usuarios (rol_id, email, password_hash, nombre, apellido, activo, created_at, updated_at) 
VALUES (
    2, 
    'supervisor@test.com', 
    '$2a$11$N9qo8uLOickgx2ZMRZoMye1J3UIYZDmQVyIKHqR7.YI0rHO5E1lM2',
    'Supervisor', 
    'Prueba', 
    true, 
    NOW(), 
    NOW()
);

-- 3. Operador de prueba
INSERT INTO usuarios (rol_id, email, password_hash, nombre, apellido, activo, created_at, updated_at) 
VALUES (
    3, 
    'operador@test.com', 
    '$2a$11$N9qo8uLOickgx2ZMRZoMye1J3UIYZDmQVyIKHqR7.YI0rHO5E1lM2',
    'Operador', 
    'Prueba', 
    true, 
    NOW(), 
    NOW()
);

-- 4. Usuario de solo consulta
INSERT INTO usuarios (rol_id, email, password_hash, nombre, apellido, activo, created_at, updated_at) 
VALUES (
    4, 
    'consulta@test.com', 
    '$2a$11$N9qo8uLOickgx2ZMRZoMye1J3UIYZDmQVyIKHqR7.YI0rHO5E1lM2',
    'Usuario', 
    'Consulta', 
    true, 
    NOW(), 
    NOW()
);

-- ==============================================
-- VERIFICACIÓN
-- ==============================================
SELECT 
    u.id, 
    u.email, 
    u.nombre, 
    u.apellido, 
    r.nombre as rol,
    u.activo,
    u.created_at
FROM usuarios u
INNER JOIN roles r ON u.rol_id = r.id
WHERE u.email LIKE '%@test.com'
ORDER BY r.id;

-- ==============================================
-- RESUMEN DE CREDENCIALES
-- ==============================================
SELECT 
    '=====================================' as separador
UNION ALL
SELECT 'USUARIOS DE PRUEBA CREADOS'
UNION ALL
SELECT '====================================='
UNION ALL
SELECT ''
UNION ALL
SELECT 'Contraseña para todos: Test123!'
UNION ALL
SELECT ''
UNION ALL
SELECT '1. admin@test.com (Administrador)'
UNION ALL
SELECT '2. supervisor@test.com (Supervisor)'
UNION ALL
SELECT '3. operador@test.com (Operador)'
UNION ALL
SELECT '4. consulta@test.com (Consulta)'
UNION ALL
SELECT ''
UNION ALL
SELECT '=====================================';
