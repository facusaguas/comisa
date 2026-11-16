using System.Text.RegularExpressions;
using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace GestorNomina.Web.Services;

/// <summary>
/// Implementación del servicio de procesamiento de PDFs para carga masiva de empleados
/// </summary>
public class PdfEmpleadoService : IPdfEmpleadoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PdfEmpleadoService> _logger;

    public PdfEmpleadoService(ApplicationDbContext context, ILogger<PdfEmpleadoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Success, string Nombre, string Apellido, string Cuit, DateTime? FechaInicio, decimal? RetribPactada)> 
        ExtraerDatosDelPdfAsync(IFormFile archivo)
    {
        try
        {
            if (archivo == null || archivo.Length == 0)
                return (false, "", "", "", null, null);

            // Extraer texto del PDF usando iTextSharp
            var contenido = await Task.Run(() => ExtraerTextoDePdf(archivo));

            if (string.IsNullOrEmpty(contenido))
            {
                _logger.LogWarning("No se pudo extraer texto del PDF {Archivo}", archivo.FileName);
                return (false, "", "", "", null, null);
            }

            // Extraer datos usando patrones regex
            var nombre = ExtraerNombre(contenido);
            var apellido = ExtraerApellido(contenido);
            var cuit = ExtraerCuit(contenido);
            var fechaInicio = ExtraerFechaInicio(contenido);
            var retribPactada = ExtraerRetribPactada(contenido);

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(cuit))
            {
                _logger.LogWarning("No se pudieron extraer datos completos del PDF {Archivo}", archivo.FileName);
                return (false, "", "", "", null, null);
            }

            return (true, nombre, apellido, cuit, fechaInicio, retribPactada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer datos del PDF {Archivo}", archivo.FileName);
            return (false, "", "", "", null, null);
        }
    }

    public async Task<ResultadoProcesamentoPdfViewModel> ProcesarEmpleadoAsync(
        string nombre,
        string apellido,
        string cuit,
        DateTime? fechaInicio,
        decimal? retribPactada)
    {
        try
        {
            // Limpiar datos
            nombre = nombre?.Trim() ?? "";
            apellido = apellido?.Trim() ?? "";
            cuit = cuit?.Trim() ?? "";

            // Validar datos mínimos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(cuit))
            {
                return new ResultadoProcesamentoPdfViewModel
                {
                    Exitoso = false,
                    Mensaje = "Faltan datos obligatorios: Nombre, Apellido y CUIT",
                    Nombre = nombre,
                    Apellido = apellido,
                    Cuit = cuit,
                    FechaInicio = fechaInicio,
                    RetribPactada = retribPactada
                };
            }

            // Verificar si el CUIT ya existe
            var empleadoExistente = await _context.Empleados
                .FirstOrDefaultAsync(e => e.Cuil == cuit);

            if (empleadoExistente != null)
            {
                // CUIT existe: reactivar y actualizar fecha
                empleadoExistente.Activo = true;
                if (fechaInicio.HasValue)
                {
                    empleadoExistente.FechaIngreso = DateTime.SpecifyKind(fechaInicio.Value, DateTimeKind.Utc);
                }
                if (retribPactada.HasValue)
                {
                    empleadoExistente.ValorConvenidoDiario = retribPactada.Value;
                }
                empleadoExistente.UpdatedAt = DateTime.UtcNow;

                _context.Empleados.Update(empleadoExistente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Empleado reactivado: {Legajo} - {Apellido}, {Nombre}", 
                    empleadoExistente.Legajo, apellido, nombre);

                return new ResultadoProcesamentoPdfViewModel
                {
                    Exitoso = true,
                    Mensaje = "Empleado reactivado y actualizado correctamente",
                    Nombre = nombre,
                    Apellido = apellido,
                    Cuit = cuit,
                    FechaInicio = fechaInicio,
                    RetribPactada = retribPactada,
                    Legajo = empleadoExistente.Legajo,
                    EsEmpleadoNuevo = false,
                    TipoOperacion = "Reactivado"
                };
            }

            // CUIT no existe: crear nuevo empleado
            // Generar próximo legajo
            var nuevoLegajo = await GenerarProximoLegajoAsync();

            var nuevoEmpleado = new Empleado
            {
                Legajo = nuevoLegajo,
                Nombre = nombre,
                Apellido = apellido,
                Cuil = cuit,
                FechaIngreso = fechaInicio.HasValue 
                    ? DateTime.SpecifyKind(fechaInicio.Value, DateTimeKind.Utc)
                    : (DateTime?)null,
                ValorConvenidoDiario = retribPactada,
                Activo = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Empleados.Add(nuevoEmpleado);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Empleado creado: {Legajo} - {Apellido}, {Nombre}", 
                nuevoEmpleado.Legajo, apellido, nombre);

            return new ResultadoProcesamentoPdfViewModel
            {
                Exitoso = true,
                Mensaje = "Empleado creado correctamente",
                Nombre = nombre,
                Apellido = apellido,
                Cuit = cuit,
                FechaInicio = fechaInicio,
                RetribPactada = retribPactada,
                Legajo = nuevoEmpleado.Legajo,
                EsEmpleadoNuevo = true,
                TipoOperacion = "Creado"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar empleado {Apellido}, {Nombre}", apellido, nombre);
            return new ResultadoProcesamentoPdfViewModel
            {
                Exitoso = false,
                Mensaje = $"Error al procesar: {ex.Message}",
                Nombre = nombre,
                Apellido = apellido,
                Cuit = cuit,
                FechaInicio = fechaInicio,
                RetribPactada = retribPactada
            };
        }
    }

    public async Task<List<ResultadoProcesamentoPdfViewModel>> ProcesarMultiplesPdfsAsync(List<IFormFile> archivos)
    {
        var resultados = new List<ResultadoProcesamentoPdfViewModel>();

        foreach (var archivo in archivos)
        {
            try
            {
                // Extraer datos del PDF
                var (success, nombre, apellido, cuit, fechaInicio, retribPactada) = 
                    await ExtraerDatosDelPdfAsync(archivo);

                if (!success)
                {
                    resultados.Add(new ResultadoProcesamentoPdfViewModel
                    {
                        Archivo = archivo.FileName,
                        Exitoso = false,
                        Mensaje = "No se pudieron extraer los datos del PDF"
                    });
                    continue;
                }

                // Procesar empleado
                var resultado = await ProcesarEmpleadoAsync(nombre, apellido, cuit, fechaInicio, retribPactada);
                resultado.Archivo = archivo.FileName;
                resultados.Add(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando archivo {Archivo}", archivo.FileName);
                resultados.Add(new ResultadoProcesamentoPdfViewModel
                {
                    Archivo = archivo.FileName,
                    Exitoso = false,
                    Mensaje = $"Error: {ex.Message}"
                });
            }
        }

        return resultados;
    }

    /// <summary>
    /// Extrae el nombre del contenido del PDF
    /// Prioridad: "Apellido y nombre:" (formato AFIP) donde la segunda+ palabras son el nombre
    /// </summary>
    private string ExtraerNombre(string contenido)
    {
        // PRIORIDAD 1: Formato AFIP "Apellido y nombre: APELLIDO NOMBRE"
        var patronAfip = @"(?:Apellido y nombre|Apellido y Nombre)[\s:]+([A-ZÁÉÍÓÚa-záéíóú]+(?:\s+[A-ZÁÉÍÓÚa-záéíóú]+)+)";
        var matchAfip = Regex.Match(contenido, patronAfip, RegexOptions.IgnoreCase);
        if (matchAfip.Success)
        {
            var textoCompleto = matchAfip.Groups[1].Value.Trim();
            // Separar: primera palabra = apellido, resto = nombre
            var palabras = textoCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (palabras.Length >= 2)
            {
                // Todo excepto la primera palabra es el nombre
                var nombre = string.Join(" ", palabras.Skip(1));
                return nombre;
            }
        }

        // PRIORIDAD 2: Patrones tradicionales
        var patrones = new[]
        {
            @"(?:Nombre|Empleado|Trabajador|Persona|NOMBRE)[\s:]*([A-ZÁÉÍÓÚa-záéíóú\s]+?)(?:\n|,|\r|$)",
            @"(?:First Name|FIRST NAME)[\s:]*([A-ZÁÉÍÓÚa-záéíóú\s]+?)(?:\n|,|\r|$)",
            @"(?:Nombre de Pila|NOMBRE DE PILA)[\s:]*([A-ZÁÉÍÓÚa-záéíóú\s]+?)(?:\n|,|\r|$)",
        };

        foreach (var patron in patrones)
        {
            var match = Regex.Match(contenido, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var texto = match.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(texto) && texto.Length > 1)
                    return texto;
            }
        }

        return "";
    }

    /// <summary>
    /// Extrae el apellido del contenido del PDF
    /// Prioridad: "Apellido y nombre:" (formato AFIP) donde la primera palabra es el apellido
    /// </summary>
    private string ExtraerApellido(string contenido)
    {
        // PRIORIDAD 1: Formato AFIP "Apellido y nombre: APELLIDO NOMBRE"
        var patronAfip = @"(?:Apellido y nombre|Apellido y Nombre)[\s:]+([A-ZÁÉÍÓÚa-záéíóú]+(?:\s+[A-ZÁÉÍÓÚa-záéíóú]+)+)";
        var matchAfip = Regex.Match(contenido, patronAfip, RegexOptions.IgnoreCase);
        if (matchAfip.Success)
        {
            var textoCompleto = matchAfip.Groups[1].Value.Trim();
            // Separar: primera palabra = apellido
            var palabras = textoCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (palabras.Length >= 1)
            {
                return palabras[0];
            }
        }

        // PRIORIDAD 2: Patrones tradicionales
        var patrones = new[]
        {
            @"(?:Apellido|Paterno|Surname|APELLIDO|LAST NAME)[\s:]*([A-ZÁÉÍÓÚa-záéíóú\s]+?)(?:\n|,|\r|$)",
            @"(?:Apellidos|Surnames)[\s:]*([A-ZÁÉÍÓÚa-záéíóú\s]+?)(?:\n|,|\r|$)",
            @"(?:Last Name|LAST NAME)[\s:]*([A-ZÁÉÍÓÚa-záéíóú\s]+?)(?:\n|,|\r|$)",
        };

        foreach (var patron in patrones)
        {
            var match = Regex.Match(contenido, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var texto = match.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(texto) && texto.Length > 1)
                    return texto;
            }
        }

        return "";
    }

    /// <summary>
    /// Extrae el CUIT del contenido del PDF
    /// Busca patrones: XX-XXXXXXXX-X o XXXXXXXXXXX
    /// </summary>
    private string ExtraerCuit(string contenido)
    {
        var patrones = new[]
        {
            @"(?:CUIL|C\.U\.I\.L|CUIL)[\s:]*(\d{2}[-\s]?\d{8}[-\s]?\d{1})",
            @"(?:CUIL)[\s:]*(\d{11})",
            @"\b(\d{2}[-\s]?\d{8}[-\s]?\d{1})\b",
            @"(?:Identificación|Id|N°|Nro|Documento|Doc)[\s:]*(\d{2}[-\s]?\d{8}[-\s]?\d{1})"
        };

        foreach (var patron in patrones)
        {
            var match = Regex.Match(contenido, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var cuit = match.Groups[1].Value.Trim();
                // Normalizar: remover guiones y espacios
                cuit = Regex.Replace(cuit, @"[-\s]", "");
                
                if (cuit.Length == 11 && cuit.All(char.IsDigit))
                    return cuit;
            }
        }

        return "";
    }

    /// <summary>
    /// Extrae la fecha de inicio del contenido del PDF
    /// Busca patrones: dd/mm/yyyy, dd-mm-yyyy, etc.
    /// </summary>
    private DateTime? ExtraerFechaInicio(string contenido)
    {
        var patrones = new[]
        {
            @"(?:Fecha de Inicio|Inicio|Ingreso|Fecha Ingreso|Fecha de Incorporación|FECHA INICIO)[\s:]*(\d{1,2}[-/]\d{1,2}[-/]\d{2,4})",
            @"(?:Desde|From|DESDE)[\s:]*(\d{1,2}[-/]\d{1,2}[-/]\d{2,4})",
            @"(?:Vigencia desde|VIGENCIA DESDE)[\s:]*(\d{1,2}[-/]\d{1,2}[-/]\d{2,4})",
            @"(\d{1,2}[-/]\d{1,2}[-/]\d{2,4})"
        };

        foreach (var patron in patrones)
        {
            var match = Regex.Match(contenido, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var fechaStr = match.Groups[1].Value.Trim();
                // Intentar parsear la fecha con diferentes formatos
                var formatos = new[] { "dd/MM/yyyy", "dd-MM-yyyy", "d/M/yyyy", "d-M-yyyy", "dd/MM/yy", "dd-MM-yy" };
                foreach (var formato in formatos)
                {
                    if (DateTime.TryParseExact(fechaStr, formato, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var fecha))
                    {
                        return fecha;
                    }
                }
                // Fallback: intentar parse genérico
                if (DateTime.TryParse(fechaStr, out var fechaGenerico))
                {
                    return fechaGenerico;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Extrae la retribución pactada del contenido del PDF
    /// Prioridad: "Retrib. pactada: $2098129,42" (formato AFIP sin separadores de miles)
    /// </summary>
    private decimal? ExtraerRetribPactada(string contenido)
    {
        // PRIORIDAD 1: Formato AFIP "Retrib. pactada: $2098129,42" (sin separadores de miles)
        var patronAfip = @"(?:Retrib\.\s*pactada|RETRIB\.\s*PACTADA)[\s:]*\$?\s*(\d+,\d{2})";
        var matchAfip = Regex.Match(contenido, patronAfip, RegexOptions.IgnoreCase);
        if (matchAfip.Success)
        {
            var montoStr = matchAfip.Groups[1].Value.Trim();
            // Formato argentino: coma es decimal (2098129,42 -> 2098129.42)
            montoStr = montoStr.Replace(",", ".");
            
            if (decimal.TryParse(montoStr, System.Globalization.CultureInfo.InvariantCulture, out var monto) && monto > 0)
            {
                return monto;
            }
        }

        // PRIORIDAD 2: Formato con separadores de miles
        var patrones = new[]
        {
            @"(?:Retrib|Sueldo|Salario|Monto|Valor|Pago|RETRIBUCIÓN|RETRIB)[\s:]*\$?\s*(\d{1,3}(?:[.,]\d{3})*(?:[.,]\d{2})?)",
            @"(?:Remuneración|REMUNERACIÓN)[\s:]*\$?\s*(\d{1,3}(?:[.,]\d{3})*(?:[.,]\d{2})?)",
            @"(?:Suma Total|SUMA TOTAL|TOTAL)[\s:]*\$?\s*(\d{1,3}(?:[.,]\d{3})*(?:[.,]\d{2})?)",
            @"\$\s*(\d{1,3}(?:[.,]\d{3})*(?:[.,]\d{2})?)",
            @"(?:Pactado|Acordado|Convenido)[\s:]*(\d+(?:[.,]\d{2})?)"
        };

        foreach (var patron in patrones)
        {
            var match = Regex.Match(contenido, patron, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var montoStr = match.Groups[1].Value.Trim();
                // Normalizar: detectar si usa . o , como separador decimal
                // Si tiene punto seguido de 3 dígitos, es separador de miles
                // Si tiene coma seguida de 2 dígitos al final, es separador decimal
                
                // Contar patrones para determinar formato
                var tienePuntoMiles = Regex.IsMatch(montoStr, @"\.\d{3}");
                var tieneComaMiles = Regex.IsMatch(montoStr, @",\d{3}");
                var tieneComaDecimal = Regex.IsMatch(montoStr, @",\d{2}$");
                var tienePuntoDecimal = Regex.IsMatch(montoStr, @"\.\d{2}$");

                if (tieneComaDecimal && !tieneComaMiles)
                {
                    // Formato argentino: punto=miles, coma=decimal (ej: 50.000,00)
                    montoStr = montoStr.Replace(".", "").Replace(",", ".");
                }
                else if (tienePuntoDecimal && !tienePuntoMiles)
                {
                    // Formato internacional: coma=miles, punto=decimal (ej: 50,000.00)
                    montoStr = montoStr.Replace(",", "");
                }
                else if (tienePuntoMiles)
                {
                    // Tiene punto como miles, remover todos los puntos
                    montoStr = montoStr.Replace(".", "").Replace(",", ".");
                }
                else if (tieneComaMiles)
                {
                    // Tiene coma como miles, remover todas las comas
                    montoStr = montoStr.Replace(",", "");
                }
                
                if (decimal.TryParse(montoStr, System.Globalization.CultureInfo.InvariantCulture, out var monto) && monto > 0)
                {
                    return monto;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Genera el próximo legajo disponible
    /// Obtiene el máximo legajo numérico existente y suma 1
    /// Si no hay legajos numéricos, comienza en 1000
    /// </summary>
    private async Task<string> GenerarProximoLegajoAsync()
    {
        try
        {
            var legajos = await _context.Empleados
                .Select(e => e.Legajo)
                .ToListAsync();

            var legajosNumericos = legajos
                .Where(l => int.TryParse(l, out _))
                .Select(int.Parse)
                .OrderByDescending(x => x)
                .ToList();

            int proximoNumero = legajosNumericos.Any() 
                ? legajosNumericos.First() + 1
                : 1000;

            return proximoNumero.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar próximo legajo");
            // Fallback: usar timestamp
            return $"L{DateTime.Now.Ticks}";
        }
    }

    /// <summary>
    /// Extrae texto de un archivo PDF usando iTextSharp
    /// Maneja PDFs comprimidos (FlateDecode) correctamente
    /// </summary>
    private string ExtraerTextoDePdf(IFormFile archivo)
    {
        try
        {
            using (var stream = archivo.OpenReadStream())
            {
                var reader = new PdfReader(stream);
                var text = new System.Text.StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    try
                    {
                        var pageText = PdfTextExtractor.GetTextFromPage(reader, i);
                        text.Append(pageText);
                        text.Append("\n");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error extrayendo página {Pagina} del PDF {Archivo}", i, archivo.FileName);
                        // Continuar con la siguiente página
                    }
                }

                reader.Close();
                return text.ToString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer texto del PDF {Archivo}", archivo.FileName);
            return "";
        }
    }
}
