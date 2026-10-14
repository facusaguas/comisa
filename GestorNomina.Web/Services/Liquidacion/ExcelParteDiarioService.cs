using ClosedXML.Excel;
using GestorNomina.Web.Data;
using GestorNomina.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Implementación del servicio de importación de horas desde Excel a horas_registros_raw
/// </summary>
public class ExcelParteDiarioService : IExcelParteDiarioService
{
    private readonly ApplicationDbContext _context;
    private readonly ICatalogResolver _catalogResolver;

    // Mapeo de columnas según formato real del Excel
    private const int FILA_TITULO_PERIODO = 1; // Fila con "2Q SEPTIEMBRE 2026"
    private const int FILA_ENCABEZADO = 12; // Fila con headers (LEG, APELLIDO, NOMBRE, etc.) - Excel es 1-indexed
    private const int COL_LEGAJO = 1; // A - LEG
    private const int COL_APELLIDO = 2; // B - APELLIDO
    private const int COL_NOMBRE = 3; // C - NOMBRE
    private const int COL_F_INGRESO = 5; // E - F.INGRESO
    private const int COL_CUIL = 6; // F - CUIL
    private const int COL_AFILIADO = 7; // G - Afiliado
    private const int COL_GREMIO_ESPECIALIDAD = 8; // H - Gremio/Especialidad
    private const int COL_CATEGORIA = 9; // I - CATEGORIA
    private const int COL_INICIO_DIAS = 10; // J - Primera columna de días (1, 2, 3...)
    private const int DIAS_POR_MES = 31; // Máximo de días que puede tener un mes

    public ExcelParteDiarioService(ApplicationDbContext context, ICatalogResolver catalogResolver)
    {
        _context = context;
        _catalogResolver = catalogResolver;
    }

    public async Task<int> ImportarParteDiarioAsync(string filePath, string periodo, int quincena, int proyectoId)
    {
        using var stream = File.OpenRead(filePath);
        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var fileHash = CalculateHash(fileBytes);
        var fileName = Path.GetFileName(filePath);

        return await ImportarParteDiarioAsync(stream, periodo, quincena, proyectoId, fileName, fileHash);
    }

    public async Task<int> ImportarParteDiarioAsync(Stream stream, string periodo, int quincena, int proyectoId)
    {
        var fileHash = CalculateStreamHash(stream);
        stream.Position = 0; // Reset stream position after hashing
        return await ImportarParteDiarioAsync(stream, periodo, quincena, proyectoId, "archivo_cargado.xlsx", fileHash);
    }

    private async Task<int> ImportarParteDiarioAsync(Stream stream, string periodo, int quincena, int proyectoId, string fileName, string fileHash)
    {
        var registrosImportados = 0;
        try
        {// Validar parámetros

            if (quincena != 1 && quincena != 2)
                throw new ArgumentException("La quincena debe ser 1 o 2", nameof(quincena));

            if (!_catalogResolver.IsLoaded)
                await _catalogResolver.LoadCatalogsAsync();

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();

            // Leer título del período de la primera fila (ejemplo: "2Q SEPTIEMBRE 2026")
            var tituloPeriodo = worksheet.Cell(FILA_TITULO_PERIODO, 5).GetString().Trim(); // Columna E

            // Extraer año y mes del parámetro periodo (formato: "2026-09")
            var año = int.Parse(periodo.Substring(0, 4));
            var mes = int.Parse(periodo.Substring(5, 2));

            // Validar que el proyecto existe
            var proyectoExiste = await _context.Proyectos.AnyAsync(p => p.Id == proyectoId);
            if (!proyectoExiste)
                throw new InvalidOperationException($"No se encontró el proyecto con ID: {proyectoId}");

            // Crear o encontrar periodo de carga
            var periodoCarga = await _context.PeriodosCarga
                .FirstOrDefaultAsync(pc => pc.ProyectoId == proyectoId
                                        && pc.Mes == mes
                                        && pc.Anio == año
                                        && pc.Quincena == quincena);

            if (periodoCarga == null)
            {
                periodoCarga = new PeriodoCarga
                {
                    ProyectoId = proyectoId,
                    Mes = mes,
                    Anio = año,
                    Quincena = quincena,
                    Estado = "Cargando",
                    NombreArchivoOriginal = fileName,
                    HashArchivo = fileHash,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.PeriodosCarga.Add(periodoCarga);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Actualizar datos del archivo
                periodoCarga.NombreArchivoOriginal = fileName;
                periodoCarga.HashArchivo = fileHash;
                periodoCarga.UpdatedAt = DateTime.UtcNow;
            }

            
            var filaActual = FILA_ENCABEZADO + 1;

            // Leer headers de días desde la fila de encabezado (columnas J en adelante)
            var diasDisponibles = new List<int>();
            for (int col = COL_INICIO_DIAS; col < COL_INICIO_DIAS + DIAS_POR_MES; col++)
            {
                var celda = worksheet.Cell(FILA_ENCABEZADO, col);
                if (celda.IsEmpty()) break;

                var diaStr = celda.GetString().Trim();
                if (int.TryParse(diaStr, out var dia))
                {
                    diasDisponibles.Add(dia);
                }
                else if (diaStr.Equals("TOTAL", StringComparison.OrdinalIgnoreCase))
                {
                    break; // Llegamos a la columna TOTAL
                }
            }

            // Procesar cada fila de empleado
            while (!worksheet.Cell(filaActual, COL_LEGAJO).IsEmpty())
            {
                var legajoStr = worksheet.Cell(filaActual, COL_LEGAJO).GetString().Trim();
                if (string.IsNullOrEmpty(legajoStr))
                {
                    filaActual++;
                    continue;
                }

                // Leer datos adicionales del empleado (opcional, para validación futura)
                var apellido = worksheet.Cell(filaActual, COL_APELLIDO).GetString().Trim();
                var nombre = worksheet.Cell(filaActual, COL_NOMBRE).GetString().Trim();
                var cuil = worksheet.Cell(filaActual, COL_CUIL).GetString().Trim();
                var categoriaStr = worksheet.Cell(filaActual, COL_CATEGORIA).GetString().Trim();

                // Procesar cada día del mes
                for (int diaIndex = 0; diaIndex < diasDisponibles.Count; diaIndex++)
                {
                    var dia = diasDisponibles[diaIndex];

                    // Verificar si el día pertenece a la quincena solicitada
                    if (quincena == 1 && dia > 15) continue;
                    if (quincena == 2 && dia <= 15) continue;

                    // Crear fecha con Kind UTC para PostgreSQL
                    var fecha = DateTime.SpecifyKind(new DateTime(año, mes, dia), DateTimeKind.Utc);
                    var colDia = COL_INICIO_DIAS + diaIndex;

                    // Leer horas de la celda
                    var cell = worksheet.Cell(filaActual, colDia);
                    if (cell.IsEmpty()) continue;

                    decimal horas = 0;
                    if (cell.TryGetValue(out double horasValue))
                    {
                        horas = (decimal)horasValue;
                    }
                    else
                    {
                        var textoHoras = cell.GetString().Trim();
                        if (decimal.TryParse(textoHoras, out var horasParsed))
                            horas = horasParsed;
                    }

                    if (horas <= 0) continue; // Omitir si no hay horas

                    // Crear registro raw
                    var registroRaw = new HoraRegistroRaw
                    {
                        PeriodoCargaId = periodoCarga.Id,
                        EmpleadoLegajo = legajoStr,
                        ActividadCodigo = "HN", // Por defecto Horas Normales (puede mejorarse)
                        Fecha = fecha,
                        HorasCargadas = horas,
                        FilaExcelOrigen = filaActual,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.HorasRegistrosRaw.Add(registroRaw);
                    registrosImportados++;
                }

                filaActual++;
            }

            // Actualizar estado del periodo
            periodoCarga.Estado = "Cargado";
            await _context.SaveChangesAsync();
        }
        catch(Exception e)
        {

        }
        return registrosImportados;
    }

    private string CalculateHash(byte[] fileBytes)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(fileBytes);
        return Convert.ToHexString(hashBytes);
    }

    private string CalculateStreamHash(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(stream);
        return Convert.ToHexString(hashBytes);
    }

}
