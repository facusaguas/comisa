#!/usr/bin/env dotnet-script
#r "nuget: BCrypt.Net-Next, 4.0.3"

using BCrypt.Net;

var password = "Test123!";
var hash = BCrypt.HashPassword(password, BCrypt.GenerateSalt(11));

Console.WriteLine("===========================================");
Console.WriteLine("HASH BCRYPT GENERADO");
Console.WriteLine("===========================================");
Console.WriteLine($"Contraseña: {password}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine("===========================================");
Console.WriteLine();

// Verificar
var isValid = BCrypt.Verify(password, hash);
Console.WriteLine($"Verificación: {(isValid ? "✓ OK" : "✗ ERROR")}");
Console.WriteLine();
Console.WriteLine("Copia este hash para el script SQL:");
Console.WriteLine(hash);
