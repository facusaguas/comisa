-- ============================================== 
-- SCRIPT RÁPIDO: CREAR ROLES
-- ==============================================
-- Solo crea los 4 roles básicos del sistema
-- Ejecutar: Copiar y pegar en pgAdmin o DBeaver
-- ==============================================

INSERT INTO roles (nombre, descripcion, created_at, updated_at) VALUES
('Administrador', 'Acceso completo al sistema', NOW(), NOW()),
('Supervisor', 'Gestión de proyectos y validaciones', NOW(), NOW()),
('Operador', 'Carga de planillas Excel', NOW(), NOW()),
('Consulta', 'Solo lectura de reportes', NOW(), NOW())
ON CONFLICT DO NOTHING;

-- Verificar
SELECT * FROM roles ORDER BY id;
