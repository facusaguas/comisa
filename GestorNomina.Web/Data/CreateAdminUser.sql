-- ============================================== 
-- CREAR USUARIO ADMINISTRADOR INICIAL
-- ==============================================
-- Email: admin@gestornomina.com
-- Contraseña: Admin123!
-- 
-- Ejecutar este script después de haber aplicado las migraciones
-- y cargado los datos iniciales (roles, regímenes, etc.)
-- 
-- Comando: psql -U postgres -d gestor_nomina_dev -f CreateAdminUser.sql
-- ==============================================

-- Eliminar usuario admin si ya existe (para poder volver a ejecutar el script)
DELETE FROM usuarios WHERE email = 'admin@gestornomina.com';

-- Insertar usuario administrador
-- Contraseña: Admin123!
-- Hash generado con BCrypt (work factor 11)
INSERT INTO usuarios (rol_id, email, password_hash, nombre, apellido, activo, created_at, updated_at) 
VALUES (
    1, 
    'admin@gestornomina.com', 
    '$2a$11$3DqN5h5VqKnH.yZQZqOhveLKxQVNZN3wPZN5h5VqKnH.yZQZqOhve',
    'Administrador', 
    'Sistema', 
    true, 
    NOW(), 
    NOW()
);

-- Verificar que se creó correctamente
SELECT id, email, nombre, apellido, activo, created_at 
FROM usuarios 
WHERE email = 'admin@gestornomina.com';

SELECT u.id, u.email, u.nombre, u.apellido, r.nombre as rol
FROM usuarios u
INNER JOIN roles r ON u.rol_id = r.id
WHERE u.email = 'admin@gestornomina.com';
