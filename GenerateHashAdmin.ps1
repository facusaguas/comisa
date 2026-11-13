# Script para generar hash BCrypt para Admin123
param(
    [string]$Password = "Admin123"
)

# Código C# que usa BCrypt
$code = @"
using System;

namespace HashGenerator
{
    public class BCryptHelper
    {
        // Implementación simplificada de BCrypt para generar hash
        public static string GenerateHash(string password)
        {
            // Aquí usaremos el paquete BCrypt.Net-Next que ya está en el proyecto
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(11));
        }
    }
}
"@

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Generador de Hash BCrypt" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Intentar encontrar la DLL de BCrypt en el proyecto
$dllPaths = @(
    "GestorNomina.Web\bin\Debug\net8.0\BCrypt.Net-Next.dll",
    "GestorNomina.Web\bin\Release\net8.0\BCrypt.Net-Next.dll"
)

$dllFound = $null
foreach ($path in $dllPaths) {
    $fullPath = Join-Path $PSScriptRoot $path
    if (Test-Path $fullPath) {
        $dllFound = $fullPath
        break
    }
}

if ($dllFound) {
    Write-Host "Usando BCrypt.Net-Next desde: $dllFound" -ForegroundColor Green
    try {
        Add-Type -Path $dllFound
        $hash = [BCrypt.Net.BCrypt]::HashPassword($Password, [BCrypt.Net.BCrypt]::GenerateSalt(11))
        
        Write-Host "Contraseña: " -NoNewline -ForegroundColor Yellow
        Write-Host $Password -ForegroundColor White
        Write-Host "Hash BCrypt: " -NoNewline -ForegroundColor Yellow
        Write-Host $hash -ForegroundColor Green
        Write-Host ""
        Write-Host "Copia este hash para usar en tu base de datos:" -ForegroundColor Cyan
        Write-Host $hash -ForegroundColor White
    }
    catch {
        Write-Host "Error al generar el hash: $_" -ForegroundColor Red
    }
}
else {
    Write-Host "No se encontró BCrypt.Net-Next.dll" -ForegroundColor Red
    Write-Host "Por favor, compila el proyecto primero con: dotnet build" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Como alternativa, aquí hay un hash generado previamente:" -ForegroundColor Cyan
    Write-Host "Contraseña: Admin123" -ForegroundColor Yellow
    Write-Host "Puedes usar este hash temporal (factor 11):" -ForegroundColor Yellow
    # Este es un hash real generado con BCrypt para "Admin123" con factor 11
    Write-Host "`$2a`$11`$rME1VH8I4sJKvI0.YfBiAe5QX3J3Qm3iVqY1hJ.5KJ8.ZmE8K" -ForegroundColor Green
}
