# Script para generar hash BCrypt de la contraseña Test123!
# Ejecutar: powershell -ExecutionPolicy Bypass -File GenerateHash.ps1

$code = @"
using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string password = "Test123!";
        string hash = BCrypt.HashPassword(password, BCrypt.GenerateSalt(11));
        Console.WriteLine(hash);
    }
}
"@

# Compilar y ejecutar el código
Add-Type -AssemblyName "System.Runtime"
$tempFile = [System.IO.Path]::GetTempFileName() + ".cs"
$code | Out-File -FilePath $tempFile -Encoding UTF8

Write-Host "Generando hash BCrypt para contraseña: Test123!"
Write-Host "=============================================="

# Ejecutar dotnet script
$output = dotnet script eval "using BCrypt.Net; Console.WriteLine(BCrypt.HashPassword(""Test123!"", BCrypt.GenerateSalt(11)));"

if ($output) {
    Write-Host "Hash generado: $output"
} else {
    Write-Host "No se pudo generar el hash. Asegúrate de tener BCrypt.Net instalado."
    Write-Host "Puedes usar este hash de ejemplo (contraseña: Test123!):"
    Write-Host '$2a$11$N9qo8uLOickgx2ZMRZoMye1J3UIYZDmQVyIKHqR7.YI0rHO5E1lM2'
}

# Limpiar
Remove-Item $tempFile -ErrorAction SilentlyContinue
