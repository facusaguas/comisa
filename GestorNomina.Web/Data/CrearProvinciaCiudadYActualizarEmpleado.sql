-- =============================================
-- Script para crear tablas Provincia, Ciudad
-- y actualizar tabla Empleados
-- PostgreSQL
-- =============================================

-- =============================================
-- 1. CREAR TABLA PROVINCIAS
-- =============================================

CREATE TABLE IF NOT EXISTS provincias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Crear índice en nombre de provincia
CREATE INDEX IF NOT EXISTS idx_provincias_nombre ON provincias(nombre);

-- =============================================
-- 2. CREAR TABLA CIUDADES
-- =============================================

CREATE TABLE IF NOT EXISTS ciudades (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    codigo_postal VARCHAR(10) NOT NULL,
    provincia_id INTEGER NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    
    -- Foreign Key
    CONSTRAINT fk_ciudades_provincia 
        FOREIGN KEY (provincia_id) 
        REFERENCES provincias(id) 
        ON DELETE RESTRICT
);

-- Crear índices
CREATE INDEX IF NOT EXISTS idx_ciudades_provincia_id ON ciudades(provincia_id);
CREATE INDEX IF NOT EXISTS idx_ciudades_nombre ON ciudades(nombre);
CREATE INDEX IF NOT EXISTS idx_ciudades_codigo_postal ON ciudades(codigo_postal);

-- =============================================
-- 3. AGREGAR COLUMNAS A EMPLEADOS
-- =============================================

-- Agregar columna domicilio si no existe
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'empleados' 
        AND column_name = 'domicilio'
    ) THEN
        ALTER TABLE empleados ADD COLUMN domicilio VARCHAR(200);
        RAISE NOTICE 'Columna domicilio agregada a empleados';
    ELSE
        RAISE NOTICE 'La columna domicilio ya existe en empleados';
    END IF;
END $$;

-- Agregar columna ciudad_id si no existe
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_name = 'empleados' 
        AND column_name = 'ciudad_id'
    ) THEN
        ALTER TABLE empleados ADD COLUMN ciudad_id INTEGER;
        RAISE NOTICE 'Columna ciudad_id agregada a empleados';
    ELSE
        RAISE NOTICE 'La columna ciudad_id ya existe en empleados';
    END IF;
END $$;

-- Agregar Foreign Key de empleados a ciudades si no existe
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.table_constraints 
        WHERE constraint_name = 'fk_empleados_ciudad'
        AND table_name = 'empleados'
    ) THEN
        ALTER TABLE empleados 
        ADD CONSTRAINT fk_empleados_ciudad 
        FOREIGN KEY (ciudad_id) 
        REFERENCES ciudades(id) 
        ON DELETE SET NULL;
        RAISE NOTICE 'Foreign Key fk_empleados_ciudad creada';
    ELSE
        RAISE NOTICE 'Foreign Key fk_empleados_ciudad ya existe';
    END IF;
END $$;

-- Crear índice en ciudad_id de empleados
CREATE INDEX IF NOT EXISTS idx_empleados_ciudad_id ON empleados(ciudad_id);

-- =============================================
-- 4. POBLAR PROVINCIAS
-- =============================================

INSERT INTO provincias (nombre, created_at, updated_at) 
VALUES
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
    ('Tucumán', NOW(), NOW())
ON CONFLICT DO NOTHING;

-- =============================================
-- 5. POBLAR CIUDADES PRINCIPALES
-- =============================================

-- Buenos Aires
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'La Plata', '1900', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Mar del Plata', '7600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Bahía Blanca', '8000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Tandil', '7000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Avellaneda', '1870', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Quilmes', '1878', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Isidro', '1642', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Lomas de Zamora', '1832', id, NOW(), NOW() FROM provincias WHERE nombre = 'Buenos Aires'
ON CONFLICT DO NOTHING;

-- Ciudad Autónoma de Buenos Aires
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'CABA', 'C1000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Ciudad Autónoma de Buenos Aires'
ON CONFLICT DO NOTHING;

-- Catamarca
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Fernando del Valle de Catamarca', '4700', id, NOW(), NOW() FROM provincias WHERE nombre = 'Catamarca'
ON CONFLICT DO NOTHING;

-- Chaco
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Resistencia', '3500', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chaco'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Barranqueras', '3503', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chaco'
ON CONFLICT DO NOTHING;

-- Chubut
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Rawson', '9103', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chubut'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Comodoro Rivadavia', '9000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chubut'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Puerto Madryn', '9120', id, NOW(), NOW() FROM provincias WHERE nombre = 'Chubut'
ON CONFLICT DO NOTHING;

-- Córdoba
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Córdoba', '5000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Córdoba'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Villa María', '5900', id, NOW(), NOW() FROM provincias WHERE nombre = 'Córdoba'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Río Cuarto', '5800', id, NOW(), NOW() FROM provincias WHERE nombre = 'Córdoba'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Villa Carlos Paz', '5152', id, NOW(), NOW() FROM provincias WHERE nombre = 'Córdoba'
ON CONFLICT DO NOTHING;

-- Corrientes
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Corrientes', '3400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Corrientes'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Goya', '3450', id, NOW(), NOW() FROM provincias WHERE nombre = 'Corrientes'
ON CONFLICT DO NOTHING;

-- Entre Ríos
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Paraná', '3100', id, NOW(), NOW() FROM provincias WHERE nombre = 'Entre Ríos'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Concordia', '3200', id, NOW(), NOW() FROM provincias WHERE nombre = 'Entre Ríos'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Gualeguaychú', '2820', id, NOW(), NOW() FROM provincias WHERE nombre = 'Entre Ríos'
ON CONFLICT DO NOTHING;

-- Formosa
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Formosa', '3600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Formosa'
ON CONFLICT DO NOTHING;

-- Jujuy
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Salvador de Jujuy', '4600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Jujuy'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Pedro', '4500', id, NOW(), NOW() FROM provincias WHERE nombre = 'Jujuy'
ON CONFLICT DO NOTHING;

-- La Pampa
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Santa Rosa', '6300', id, NOW(), NOW() FROM provincias WHERE nombre = 'La Pampa'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'General Pico', '6360', id, NOW(), NOW() FROM provincias WHERE nombre = 'La Pampa'
ON CONFLICT DO NOTHING;

-- La Rioja
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'La Rioja', '5300', id, NOW(), NOW() FROM provincias WHERE nombre = 'La Rioja'
ON CONFLICT DO NOTHING;

-- Mendoza
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Mendoza', '5500', id, NOW(), NOW() FROM provincias WHERE nombre = 'Mendoza'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Rafael', '5600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Mendoza'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Godoy Cruz', '5501', id, NOW(), NOW() FROM provincias WHERE nombre = 'Mendoza'
ON CONFLICT DO NOTHING;

-- Misiones
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Posadas', '3300', id, NOW(), NOW() FROM provincias WHERE nombre = 'Misiones'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Oberá', '3360', id, NOW(), NOW() FROM provincias WHERE nombre = 'Misiones'
ON CONFLICT DO NOTHING;

-- Neuquén
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Neuquén', '8300', id, NOW(), NOW() FROM provincias WHERE nombre = 'Neuquén'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Martín de los Andes', '8370', id, NOW(), NOW() FROM provincias WHERE nombre = 'Neuquén'
ON CONFLICT DO NOTHING;

-- Río Negro
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Viedma', '8500', id, NOW(), NOW() FROM provincias WHERE nombre = 'Río Negro'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Carlos de Bariloche', '8400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Río Negro'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Cipolletti', '8324', id, NOW(), NOW() FROM provincias WHERE nombre = 'Río Negro'
ON CONFLICT DO NOTHING;

-- Salta
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Salta', '4400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Salta'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Orán', '4530', id, NOW(), NOW() FROM provincias WHERE nombre = 'Salta'
ON CONFLICT DO NOTHING;

-- San Juan
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Juan', '5400', id, NOW(), NOW() FROM provincias WHERE nombre = 'San Juan'
ON CONFLICT DO NOTHING;

-- San Luis
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Luis', '5700', id, NOW(), NOW() FROM provincias WHERE nombre = 'San Luis'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Villa Mercedes', '5730', id, NOW(), NOW() FROM provincias WHERE nombre = 'San Luis'
ON CONFLICT DO NOTHING;

-- Santa Cruz
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Río Gallegos', '9400', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Cruz'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Caleta Olivia', '9011', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Cruz'
ON CONFLICT DO NOTHING;

-- Santa Fe
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Santa Fe', '3000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Fe'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Rosario', '2000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Fe'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Rafaela', '2300', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Fe'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Venado Tuerto', '2600', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santa Fe'
ON CONFLICT DO NOTHING;

-- Santiago del Estero
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Santiago del Estero', '4200', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santiago del Estero'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'La Banda', '4300', id, NOW(), NOW() FROM provincias WHERE nombre = 'Santiago del Estero'
ON CONFLICT DO NOTHING;

-- Tierra del Fuego
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Ushuaia', '9410', id, NOW(), NOW() FROM provincias WHERE nombre = 'Tierra del Fuego'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Río Grande', '9420', id, NOW(), NOW() FROM provincias WHERE nombre = 'Tierra del Fuego'
ON CONFLICT DO NOTHING;

-- Tucumán
INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'San Miguel de Tucumán', '4000', id, NOW(), NOW() FROM provincias WHERE nombre = 'Tucumán'
ON CONFLICT DO NOTHING;

INSERT INTO ciudades (nombre, codigo_postal, provincia_id, created_at, updated_at)
SELECT 'Concepción', '4146', id, NOW(), NOW() FROM provincias WHERE nombre = 'Tucumán'
ON CONFLICT DO NOTHING;

-- =============================================
-- 6. VERIFICAR RESULTADOS
-- =============================================

-- Mostrar resumen de provincias y ciudades
SELECT 
    p.nombre as provincia,
    COUNT(c.id) as cantidad_ciudades
FROM provincias p
LEFT JOIN ciudades c ON p.id = c.provincia_id
GROUP BY p.id, p.nombre
ORDER BY p.nombre;

-- Mostrar estructura de la tabla empleados actualizada
SELECT 
    column_name, 
    data_type, 
    character_maximum_length,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'empleados'
AND column_name IN ('domicilio', 'ciudad_id')
ORDER BY ordinal_position;

-- Mensaje de finalización
DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Script ejecutado exitosamente';
    RAISE NOTICE 'Tablas creadas: provincias, ciudades';
    RAISE NOTICE 'Columnas agregadas a empleados: domicilio, ciudad_id';
    RAISE NOTICE '========================================';
END $$;
