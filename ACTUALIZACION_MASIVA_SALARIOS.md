# Actualización Masiva de Salarios - Módulo de Categorías

## Resumen
Se ha implementado un módulo completo para la gestión de categorías profesionales con funcionalidad de actualización masiva de salarios básicos.

## Funcionalidades Implementadas

### 1. Vista de Categorías
- **Ruta**: `/Categorias/Index`
- **Menú**: Liquidación → Catálogos de Liquidación → Categorías
- **Descripción**: Listado de todas las categorías profesionales con sus salarios básicos organizados por convenio

#### Características de la Vista:
- Tabla responsive con las siguientes columnas:
  - **Código**: Código identificador de la categoría
  - **Nombre**: Nombre descriptivo de la categoría
  - **Convenio**: Badge con el convenio asociado
  - **Salario Básico**: Valor monetario formateado

- **Botón "Actualización Masiva"** (solo visible para administradores)
  - Abre un modal con el formulario de actualización

### 2. Modal de Actualización Masiva

El modal permite actualizar los salarios básicos de forma masiva con las siguientes opciones:

#### Filtros:
- **Convenio**: 
  - Dropdown con todos los convenios disponibles
  - Opción "Todos los convenios" para aplicar a todas las categorías
  - Si se selecciona un convenio específico, solo se actualizan las categorías de ese convenio

#### Parámetros:
- **Porcentaje de Aumento** (requerido):
  - Campo numérico con decimales
  - Validación mínima: debe ser mayor a 0
  - Advertencia automática si el porcentaje es mayor a 100%
  - Ejemplo: ingresar `15.5` para un aumento del 15.5%

- **Checkbox "Actualizar también valores de empleados"**:
  - Opción para actualizar simultáneamente los valores de los empleados
  - Cuando está activado, actualiza:
    - `valor_hora_basico`
    - `valor_convenido_diario`
  - Solo afecta a empleados con tipos de liquidación:
    - **"Directo con acuerdo"** (código: `DIRECTOS_CONVERSION_ACUERDO`)
    - **"Indirecto"** (código: `INDIRECTO`)

#### Alertas Informativas:
- Alerta azul explicando qué hace la operación
- Alerta naranja advirtiendo que la operación no se puede deshacer
- Hint explicativo sobre qué empleados serán afectados

### 3. Proceso de Actualización

#### Actualización de Categorías:
1. Se filtran las categorías según el convenio seleccionado (o todas si no se especifica)
2. Para cada categoría con salario básico:
   ```
   nuevo_salario = salario_actual + (salario_actual * porcentaje / 100)
   ```
3. Se guarda el nuevo valor en la base de datos

#### Actualización de Empleados (opcional):
1. Se identifican los tipos de liquidación objetivo
2. Se filtran empleados que cumplan:
   - Tienen tipo de liquidación "Directo con acuerdo" o "Indirecto"
   - Tienen `valor_hora_basico` o `valor_convenido_diario` definidos
   - Si se especificó convenio, también deben pertenecer a ese convenio
3. Para cada empleado:
   ```
   Si tiene valor_hora_basico:
     nuevo_valor = valor_actual + (valor_actual * porcentaje / 100)
   
   Si tiene valor_convenido_diario:
     nuevo_valor = valor_actual + (valor_actual * porcentaje / 100)
   ```
4. Se actualiza el campo `updated_at` del empleado

### 4. Mensajes de Resultado

Después de la actualización, se muestra un mensaje con:
- Cantidad de categorías actualizadas
- Porcentaje de aumento aplicado
- Cantidad de empleados actualizados (si corresponde)

Ejemplo:
> "Se actualizaron 15 categoría(s) con un aumento del 12.5% y 42 empleado(s)"

## Archivos Creados

### Servicios:
1. **ICategoriaService.cs** - Interfaz del servicio
2. **CategoriaService.cs** - Implementación del servicio

### Controladores:
3. **CategoriasController.cs** - Controlador con acciones Index y ActualizarSalariosMasivo

### Vistas:
4. **Views/Categorias/Index.cshtml** - Vista principal con tabla y modal

### Configuración:
5. **Program.cs** - Registrado el servicio ICategoriaService

### Menú:
6. **Views/Shared/_LayoutTabler.cshtml** - Actualizado enlace de Categorías

## Seguridad y Permisos

- **Vista de listado**: Accesible para todos los usuarios autenticados
- **Botón de actualización masiva**: Solo visible para administradores
- **Acción de actualización**: Protegida con atributo `[AdminOnly]`
- **Token CSRF**: Protección contra ataques CSRF con `@Html.AntiForgeryToken()`

## Uso

### Caso 1: Actualizar solo salarios de categorías
1. Ir a: **Liquidación → Catálogos de Liquidación → Categorías**
2. Click en "Actualización Masiva"
3. Seleccionar convenio (opcional)
4. Ingresar porcentaje de aumento (ej: `10` para 10%)
5. **NO** activar el checkbox de empleados
6. Click en "Aplicar Actualización"

### Caso 2: Actualizar salarios y valores de empleados
1. Ir a: **Liquidación → Catálogos de Liquidación → Categorías**
2. Click en "Actualización Masiva"
3. Seleccionar convenio (opcional)
4. Ingresar porcentaje de aumento (ej: `12.5` para 12.5%)
5. **Activar** el checkbox "Actualizar también valores de empleados"
6. Click en "Aplicar Actualización"

### Caso 3: Actualizar solo un convenio específico
1. Ir a: **Liquidación → Catálogos de Liquidación → Categorías**
2. Click en "Actualización Masiva"
3. Seleccionar el convenio deseado del dropdown
4. Ingresar porcentaje de aumento
5. Activar o no el checkbox según se necesite
6. Click en "Aplicar Actualización"

## Ejemplos de Actualización

### Ejemplo 1: Aumento del 15% solo categorías
**Datos iniciales:**
- Categoría "Oficial Especializado": $50,000
- Categoría "Supervisor": $75,000

**Proceso:**
- Convenio: Todos
- Porcentaje: 15%
- Actualizar empleados: NO

**Resultados:**
- Categoría "Oficial Especializado": $57,500 (50,000 + 7,500)
- Categoría "Supervisor": $86,250 (75,000 + 11,250)
- Mensaje: "Se actualizaron 2 categoría(s) con un aumento del 15%"

### Ejemplo 2: Aumento del 10% con empleados
**Datos iniciales:**
- Categorías de Convenio "UOCRA": 8 categorías
- Empleados con tipo "Directo con acuerdo": 25 empleados
- Empleados con tipo "Indirecto": 10 empleados

**Proceso:**
- Convenio: UOCRA
- Porcentaje: 10%
- Actualizar empleados: SÍ

**Resultados:**
- Se actualizan las 8 categorías del convenio UOCRA
- Se actualizan los `valor_hora_basico` y `valor_convenido_diario` de los 35 empleados
- Mensaje: "Se actualizaron 8 categoría(s) con un aumento del 10% y 35 empleado(s)"

## Tipos de Liquidación Afectados

La actualización de empleados **solo** afecta a:

### 1. Directo con Acuerdo
- **Código**: `DIRECTOS_CONVERSION_ACUERDO`
- **Descripción**: Empleados directos con acuerdo de conversión
- **Campos actualizados**: 
  - `valor_hora_basico`
  - `valor_convenido_diario`

### 2. Indirecto
- **Código**: `INDIRECTO`
- **Descripción**: Empleados indirectos
- **Campos actualizados**: 
  - `valor_hora_basico`
  - `valor_convenido_diario`

### Tipos NO Afectados:
- Empleados con tipo "Directo" (sin acuerdo)
- Cualquier otro tipo de liquidación personalizado
- Empleados sin tipo de liquidación definido

## Validaciones

### En el Frontend:
- Porcentaje de aumento es requerido
- Porcentaje debe ser mayor a 0
- Campo numérico con paso decimal (0.01)

### En el Backend:
- Validación de porcentaje mayor a 0
- Advertencia si el porcentaje es mayor a 100%
- Solo administradores pueden ejecutar la actualización
- Validación de token CSRF

## Logging

El sistema registra en logs:
- Inicio de actualización masiva con parámetros
- Cantidad de categorías actualizadas
- Cantidad de empleados actualizados
- Errores durante el proceso

Ejemplo de log:
```
[Information] Actualización masiva de salarios: 8 categorías actualizadas con 10% de aumento
[Information] Actualización masiva de empleados: 35 empleados actualizados con 10% de aumento
```

## Manejo de Errores

- Si hay un error durante la actualización, se muestra mensaje de error
- Los cambios en categorías se guardan antes de actualizar empleados
- Si falla la actualización de empleados, las categorías ya fueron actualizadas
- Logs detallados de cualquier excepción

## Consideraciones Importantes

### ⚠️ La operación NO se puede deshacer
- No hay rollback automático
- Se recomienda hacer backup de la base de datos antes de actualizaciones importantes
- Verificar bien el porcentaje antes de confirmar

### ⚠️ Filtro por convenio en empleados
- Si se selecciona un convenio específico, los empleados también se filtran por ese convenio
- Empleados sin convenio asignado no serán actualizados aunque tengan el tipo de liquidación correcto

### ⚠️ Solo actualiza valores existentes
- Solo se actualizan salarios/valores que ya tienen un monto definido
- No se crean valores nuevos donde no existan
- Valores en 0 o NULL no se actualizan

## Mejoras Futuras Sugeridas

1. **Preview de cambios**: Mostrar una tabla con los valores antes/después antes de confirmar
2. **Historial de actualizaciones**: Registrar en base de datos cada actualización masiva
3. **Exportar reporte**: Generar Excel con el detalle de todos los cambios realizados
4. **Rollback**: Implementar función para deshacer la última actualización
5. **Actualización por categoría individual**: Permitir actualizar categorías específicas
6. **Validación de rangos**: Configurar rangos mínimos/máximos permitidos por categoría
7. **Notificaciones**: Enviar email a los administradores después de cada actualización
8. **Programación**: Permitir programar actualizaciones para una fecha futura

## Estado
✅ **Completado y Funcional** - El módulo está implementado, compila correctamente y listo para usar.
