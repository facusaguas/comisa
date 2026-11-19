-- =============================================
-- Script para poblar Provincias y Ciudades de Argentina
-- =============================================

-- Insertar Provincias
INSERT INTO provincias (nombre, created_at, updated_at) VALUES
('Buenos Aires', NOW(), NOW()),
('Ciudad Autónoma de Buenos Aires', NOW(), NOW()),
('Catamarca', NOW(), NOW()),
('Chaco', NOW(), NOW()),
('Chubut', NOW(), NOW()),
('Córdoba', NOW(), NOW()),
('Corrientes', NOW(), NOW()),
('Entre Ríos', NOW(), NOW()),
('Formosa', NOW(), NOW()),
('Jujuy', NOW(), NOW()),
('La Pampa', NOW(), NOW()),
('La Rioja', NOW(), NOW()),
('Mendoza', NOW(), NOW()),
('Misiones', NOW(), NOW()),
('Neuquén', NOW(), NOW()),
('Río Negro', NOW(), NOW()),
('Salta', NOW(), NOW()),
('San Juan', NOW(), NOW()),
('San Luis', NOW(), NOW()),
('Santa Cruz', NOW(), NOW()),
('Santa Fe', NOW(), NOW()),
('Santiago del Estero', NOW(), NOW()),
('Tierra del Fuego', NOW(), NOW()),
('Tucumán', NOW(), NOW());

-- Insertar algunas ciudades principales por provincia (ejemplo)

-- Buenos Aires
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'La Plata', '1900', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Mar del Plata', '7600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Bahía Blanca', '8000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Tandil', '7000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Avellaneda', '1870', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Quilmes', '1878', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires';

-- Ciudad Autónoma de Buenos Aires
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'CABA', 'C1000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Ciudad Autónoma de Buenos Aires';

-- Catamarca
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Fernando del Valle de Catamarca', '4700', id, NOW(), NOW() FROM provincias WHERE nombre = 'Catamarca';

-- Chaco
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Resistencia', '3500', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chaco';

-- Chubut
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Rawson', '9103', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chubut';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Comodoro Rivadavia', '9000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chubut';

-- Córdoba
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Córdoba', '5000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Córdoba';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Villa María', '5900', id, NOW(), NOW() FROM provincias WHERE nombre = 'Córdoba';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Río Cuarto', '5800', id, NOW(), NOW() FROM provincias WHERE nombre = 'Córdoba';

-- Corrientes
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Corrientes', '3400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Corrientes';

-- Entre Ríos
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Paraná', '3100', id, NOW(), NOW() FROM provincias WHERE nombre = 'Entre Ríos';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Concordia', '3200', id, NOW(), NOW() FROM provincias WHERE nombre = 'Entre Ríos';

-- Formosa
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Formosa', '3600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Formosa';

-- Jujuy
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Salvador de Jujuy', '4600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Jujuy';

-- La Pampa
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Santa Rosa', '6300', id, NOW(), NOW() FROM provincias WHERE nombre = 'La Pampa';

-- La Rioja
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'La Rioja', '5300', id, NOW(), NOW() FROM provincias WHERE nombre = 'La Rioja';

-- Mendoza
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Mendoza', '5500', id, NOW(), NOW() FROM provincias WHERE nombre = 'Mendoza';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Rafael', '5600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Mendoza';

-- Misiones
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Posadas', '3300', id, NOW(), NOW() FROM provincias WHERE nombre = 'Misiones';

-- Neuquén
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Neuquén', '8300', id, NOW(), NOW() FROM provincias WHERE nombre = 'Neuquén';

-- Río Negro
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Viedma', '8500', id, NOW(), NOW() FROM provincias WHERE nombre = 'Río Negro';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Carlos de Bariloche', '8400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Río Negro';

-- Salta
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Salta', '4400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Salta';

-- San Juan
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Juan', '5400', id, NOW(), NOW() FROM provincias WHERE nombre = 'San Juan';

-- San Luis
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Luis', '5700', id, NOW(), NOW() FROM provincias WHERE nombre = 'San Luis';

-- Santa Cruz
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Río Gallegos', '9400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Cruz';

-- Santa Fe
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Santa Fe', '3000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Fe';

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Rosario', '2000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Fe';

-- Santiago del Estero
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Santiago del Estero', '4200', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santiago del Estero';

-- Tierra del Fuego
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Ushuaia', '9410', id, NOW(), NOW() FROM provincias WHERE nombre = 'Tierra del Fuego';

-- Tucumán
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Miguel de Tucumán', '4000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Tucumán';

-- Verificar inserción
SELECT 
    p.nombre as provincia,
    COUNT(c.id) as cantidad_ciudades
FROM provincias p
LEFT JOIN ciudades c ON p.id = c.provincia_id
GROUP BY p.id, p.nombre
ORDER BY p.nombre;
