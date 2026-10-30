# ============================================== 
# Script para inicializar la base de datos
# ==============================================
# Este script ejecuta el archivo SQL de inicialización
# 
# Uso: .\InitializeDB.ps1
# ============================================== 

param(
    [string]$Host = "localhost",
    [string]$Port = "5432",
    [string]$Database = "gestor_nomina_dev",
    [string]$User = "postgres",
    [string]$Password = "postgres"
)

Write-Host "=============================================="  -ForegroundColor Cyan
Write-Host "INICIALIZANDO BASE DE DATOS"  -ForegroundColor Cyan
Write-Host "=============================================="  -ForegroundColor Cyan
Write-Host ""
Write-Host "Host: $Host"
Write-Host "Puerto: $Port"
Write-Host "Base de datos: $Database"
Write-Host "Usuario: $User"
Write-Host ""

# Verificar si psql está disponible
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue

if (-not $psqlPath) {
    Write-Host "❌ ERROR: psql no está disponible en el PATH" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor, ejecuta el script SQL manualmente:" -ForegroundColor Yellow
    Write-Host "1. Abre pgAdmin o DBeaver"
    Write-Host "2. Conecta a la base de datos: $Database"
    Write-Host "3. Ejecuta el archivo: Data\InitializeDatabase.sql"
    Write-Host ""
    Write-Host "O ejecuta este comando en PowerShell:" -ForegroundColor Yellow
    Write-Host '$env:PGPASSWORD="' + $Password + '"; psql -h ' + $Host + ' -U ' + $User + ' -d ' + $Database + ' -f Data\InitializeDatabase.sql' -ForegroundColor Green
    exit 1
}

# Establecer password
$env:PGPASSWORD = $Password

# Ejecutar el script SQL
Write-Host "Ejecutando script de inicialización..." -ForegroundColor Yellow
Write-Host ""

try {
    psql -h $Host -p $Port -U $User -d $Database -f "Data\InitializeDatabase.sql"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "=============================================="  -ForegroundColor Green
        Write-Host "✓ BASE DE DATOS INICIALIZADA CORRECTAMENTE"  -ForegroundColor Green
        Write-Host "=============================================="  -ForegroundColor Green
        Write-Host ""
        Write-Host "CREDENCIALES DE PRUEBA:" -ForegroundColor Cyan
        Write-Host "  Contraseña: Test123!" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "  1. admin@test.com       (Administrador)"
        Write-Host "  2. supervisor@test.com  (Supervisor)"
        Write-Host "  3. operador@test.com    (Operador)"
        Write-Host "  4. consulta@test.com    (Consulta)"
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "❌ ERROR: Falló la ejecución del script" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host ""
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    # Limpiar password del entorno
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}
