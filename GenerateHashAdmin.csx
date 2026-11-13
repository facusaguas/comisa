#r "nuget: BCrypt.Net-Next, 4.0.3"

using BCrypt.Net;

var password = Args.Count > 0 ? Args[0] : "Admin123";
var hash = BCrypt.HashPassword(password, BCrypt.GenerateSalt(11));

Console.WriteLine($"Contraseña: {password}");
Console.WriteLine($"Hash BCrypt: {hash}");
