# 🔐 Credenciales de Acceso - Gestor de Nómina

## Usuario Administrador Inicial

Una vez que hayas ejecutado el script `SeedData.sql`, podrás acceder con las siguientes credenciales:

### Credenciales de Acceso
```
Email:       admin@gestornomina.com
Contraseña:  Admin123!
```

---

## Instrucciones de Primer Acceso

### 1. Aplicar Migraciones
```powershell
dotnet ef database update --project GestorNomina.Web/GestorNomina.Web.csproj
```

### 2. Cargar Datos Iniciales
```powershell
psql -U postgres -d gestor_nomina_dev -f GestorNomina.Web/Data/SeedData.sql
```

**Nota:** El script `SeedData.sql` incluye:
- ✅ 4 Roles (Administrador, Supervisor, Operador, Consulta)
- ✅ 5 Regímenes Laborales (14x14, 7x7, 20x10, 6x1, 5x2)
- ✅ 12 Actividades (HN, HE50, HE100, Licencias, Francos, etc.)
- ✅ 10 Reglas Normativas
- ✅ 15 Feriados Argentina 2024
- ✅ 1 Usuario Administrador

### 3. Ejecutar la Aplicación
```powershell
dotnet run --project GestorNomina.Web/GestorNomina.Web.csproj
```

### 4. Acceder al Sistema
Abre tu navegador en: **https://localhost:5001** (o el puerto que te indique la consola)

---

## Cambiar Contraseña al Primer Ingreso

⚠️ **IMPORTANTE:** Por seguridad, se recomienda cambiar la contraseña del administrador después del primer acceso.

1. Inicia sesión con las credenciales proporcionadas
2. Ve a **Mi Perfil** (esquina superior derecha)
3. Selecciona **Cambiar Contraseña**
4. Ingresa:
   - Contraseña actual: `Admin123!`
   - Nueva contraseña: (tu contraseña segura)
   - Confirmar contraseña: (repite tu contraseña)

---

## Crear Usuarios Adicionales

Una vez autenticado como Administrador, puedes crear usuarios adicionales:

1. Ve a **Administración → Usuarios**
2. Click en **Nuevo Usuario**
3. Completa el formulario:
   - Email
   - Contraseña
   - Nombre y Apellido
   - Rol (Administrador, Supervisor, Operador, Consulta)
4. Click en **Guardar**

---

## Roles y Permisos

| Rol | Descripción | Permisos |
|-----|-------------|----------|
| **Administrador** | Acceso total al sistema | Gestión de usuarios, configuración, reportes, validaciones |
| **Supervisor** | Gestión de proyectos y validaciones | Validación de horas, gestión de proyectos, reportes |
| **Operador** | Carga de planillas | Carga de archivos Excel, visualización de registros |
| **Consulta** | Solo lectura | Visualización de reportes y consultas |

---

## Solución de Problemas

### No puedo iniciar sesión
- Verifica que el script `SeedData.sql` se haya ejecutado correctamente
- Confirma que el email es: `admin@gestornomina.com` (sin espacios)
- La contraseña es sensible a mayúsculas: `Admin123!`

### Error de base de datos
```sql
-- Verificar que el usuario existe:
SELECT id, email, nombre, apellido, activo 
FROM usuarios 
WHERE email = 'admin@gestornomina.com';

-- Si no existe, ejecutar:
psql -U postgres -d gestor_nomina_dev -f GestorNomina.Web/Data/CreateAdminUser.sql
```

### Olvidé mi contraseña
Si necesitas resetear la contraseña del administrador:
```powershell
# Ejecutar el script de creación del admin (sobrescribe el usuario)
psql -U postgres -d gestor_nomina_dev -f GestorNomina.Web/Data/CreateAdminUser.sql
```

---

## Seguridad

⚠️ **Recomendaciones de Seguridad:**

1. ✅ Cambia la contraseña del administrador inmediatamente después del primer acceso
2. ✅ No compartas las credenciales de administrador
3. ✅ Crea usuarios individuales para cada persona del equipo
4. ✅ Usa contraseñas fuertes (mínimo 8 caracteres, letras, números y símbolos)
5. ✅ Revisa periódicamente los usuarios activos y desactiva los que ya no se usen
6. ✅ En producción, configura HTTPS y cookies seguras

---

## Información Técnica

- **Hash de Contraseña:** BCrypt con work factor 11
- **Algoritmo:** BCrypt.Net-Next v4.2.0
- **Sesiones:** 30 minutos de inactividad
- **Cookies:** HttpOnly, Secure, SameSite=Strict

---

📅 **Fecha de Creación:** 2024-10-05  
🔄 **Última Actualización:** 2024-10-05
