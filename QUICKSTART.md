# 🚀 Guía de Inicio Rápido - Gestor de Nómina

## Para Desarrolladores Nuevos

### 1️⃣ Instalar Prerequisitos (5 minutos)

#### PostgreSQL
```bash
# Opción A: Instalador oficial
# Descargar de: https://www.postgresql.org/download/

# Opción B: Docker (recomendado)
docker run --name postgres-nomina -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15
```

#### .NET 8 SDK
```bash
# Verificar instalación
dotnet --version

# Si necesitas instalar:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### 2️⃣ Configurar Base de Datos (2 minutos)

```bash
# Crear base de datos
createdb -U postgres gestor_nomina_dev

# O con Docker:
docker exec -it postgres-nomina createdb -U postgres gestor_nomina_dev
```

### 3️⃣ Configurar Conexión (1 minuto)

Editar `GestorNomina.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gestor_nomina_dev;Username=postgres;Password=TU_PASSWORD"
  }
}
```

### 4️⃣ Aplicar Migraciones (1 minuto)

```bash
dotnet ef database update --project GestorNomina.Web/GestorNomina.Web.csproj
```

### 5️⃣ Cargar Datos Iniciales (1 minuto)

```bash
psql -U postgres -d gestor_nomina_dev -f GestorNomina.Web/Data/SeedData.sql
```

### 6️⃣ Ejecutar la Aplicación (30 segundos)

```bash
dotnet run --project GestorNomina.Web/GestorNomina.Web.csproj
```

🎉 **¡Listo!** Abre tu navegador en `https://localhost:7000`

---

## 🔍 Verificar que Todo Funciona

### Verificar Base de Datos

```bash
# Conectar a PostgreSQL
psql -U postgres -d gestor_nomina_dev

# Ver tablas creadas (deberían ser 13)
\dt

# Verificar datos iniciales
SELECT COUNT(*) FROM roles;              -- Debería retornar 4
SELECT COUNT(*) FROM regimenes_laborales; -- Debería retornar 5
SELECT COUNT(*) FROM actividades;        -- Debería retornar 12
SELECT COUNT(*) FROM reglas_normativas;  -- Debería retornar 10

# Salir
\q
```

### Verificar Compilación

```bash
dotnet build
# Debería compilar sin errores
```

---

## ⚠️ Solución de Problemas Comunes

### Error: "database does not exist"
```bash
# Crear la base de datos manualmente
createdb -U postgres gestor_nomina_dev
```

### Error: "password authentication failed"
```bash
# Verificar contraseña en appsettings.Development.json
# O cambiar método de autenticación en pg_hba.conf a 'trust' (solo desarrollo)
```

### Error: "port 5432 is already in use"
```bash
# PostgreSQL ya está corriendo, verificar:
netstat -an | Select-String "5432"

# O detener otros contenedores PostgreSQL:
docker stop $(docker ps -q --filter ancestor=postgres)
```

### Error: "dotnet ef command not found"
```bash
# Instalar herramientas EF Core
dotnet tool install --global dotnet-ef
```

---

## 📂 Estructura de Archivos Importantes

```
GestorNomina/
├── README.md                          ← Documentación general
├── QUICKSTART.md                      ← Esta guía
├── PASO_1_README.md                   ← Detalles técnicos PASO 1
├── POSTGRESQL_SETUP.md                ← Guía completa PostgreSQL
└── GestorNomina.Web/
    ├── Program.cs                     ← Configuración de la app
    ├── appsettings.Development.json   ← Config de desarrollo
    ├── Data/
    │   ├── ApplicationDbContext.cs    ← Contexto EF Core
    │   └── SeedData.sql               ← Datos iniciales
    └── Models/Entities/               ← Entidades del dominio
```

---

## 🧪 Comandos Útiles Diarios

```bash
# Ejecutar con auto-reload (desarrollo)
dotnet watch run --project GestorNomina.Web/GestorNomina.Web.csproj

# Compilar
dotnet build

# Limpiar y recompilar
dotnet clean && dotnet build

# Crear nueva migración
dotnet ef migrations add NombreMigracion --project GestorNomina.Web/GestorNomina.Web.csproj

# Ver SQL de la migración
dotnet ef migrations script --project GestorNomina.Web/GestorNomina.Web.csproj

# Revertir última migración
dotnet ef migrations remove --project GestorNomina.Web/GestorNomina.Web.csproj
```

---

## 📚 Próximos Pasos

Una vez que tengas todo funcionando:

1. ✅ Revisa [PASO_1_README.md](./PASO_1_README.md) para entender la arquitectura
2. ⏳ Prepárate para el PASO 2: Layout con Tabler UI
3. 📖 Familiarízate con las entidades en `Models/Entities/`
4. 🗄️ Explora la base de datos con pgAdmin o DBeaver

---

## 🆘 ¿Necesitas Ayuda?

- **Problemas de PostgreSQL**: Ver [POSTGRESQL_SETUP.md](./POSTGRESQL_SETUP.md)
- **Detalles técnicos**: Ver [PASO_1_README.md](./PASO_1_README.md)
- **Documentación general**: Ver [README.md](./README.md)

---

## 🎯 Estado Actual del Proyecto

✅ **PASO 1 COMPLETADO**
- Base de datos diseñada y migraciones creadas
- 13 entidades con relaciones configuradas
- Datos iniciales disponibles
- Proyecto compila sin errores

⏳ **SIGUIENTE: PASO 2**
- Implementar Tabler UI
- Sistema de autenticación
- Layout base de la aplicación

---

**Tiempo total estimado de setup**: ~10 minutos
**Última actualización**: Octubre 2026
