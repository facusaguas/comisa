using System.Text;
using ClosedXML.Excel;
using GestorNomina.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace GestorNomina.Web.Services.Liquidacion;

/// <summary>
/// Implementación del servicio de exportación a Tango Gestión
/// </summary>
public class TangoExportService : ITangoExportService
{
    private readonly ApplicationDbContext _context;

    public TangoExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> ExportarReciboAsync(int reciboId)
    {
        var recibo = await _context.Recibos
            .Include(r => r.Empleado)
            .Include(r => r.Detalles)
                .ThenInclude(d => d.Concepto)
            .FirstOrDefaultAsync(r => r.Id == reciboId);

        if (recibo == null)
            throw new InvalidOperationException($"No se encontró el recibo con ID {reciboId}");

        var sb = new StringBuilder();

        // Generar líneas del archivo Tango
        foreach (var detalle in recibo.Detalles.OrderBy(d => d.Concepto.Codigo))
        {
            // Formato: [Legajo];[CodigoConcepto];[Unidades];[ImporteCalculado]
            var linea = $"{recibo.Empleado.Legajo};{detalle.Concepto.Codigo};{detalle.Unidades:F2};{detalle.Subtotal:F2}";
            sb.AppendLine(linea);
        }

        return sb.ToString();
    }

    public async Task<string> ExportarPeriodoAsync(string periodo, int quincena)
    {
        var recibos = await _context.Recibos
            .Include(r => r.Empleado)
            .Include(r => r.Detalles)
                .ThenInclude(d => d.Concepto)
            .Where(r => r.Periodo == periodo && r.Quincena == quincena)
            .OrderBy(r => r.Empleado.Legajo)
            .ToListAsync();

        if (!recibos.Any())
            throw new InvalidOperationException($"No se encontraron recibos para el periodo {periodo} quincena {quincena}");

        var sb = new StringBuilder();

        // Agregar encabezado opcional (comentado por defecto)
        // sb.AppendLine("Legajo;CodigoConcepto;Unidades;ImporteCalculado");

        foreach (var recibo in recibos)
        {
            foreach (var detalle in recibo.Detalles.OrderBy(d => d.Concepto.Codigo))
            {
                var linea = $"{recibo.Empleado.Legajo};{detalle.Concepto.Codigo};{detalle.Unidades:F2};{detalle.Subtotal:F2}";
                sb.AppendLine(linea);
            }
        }

        return sb.ToString();
    }

    public async Task GuardarArchivoAsync(string contenido, string rutaArchivo)
    {
        var directorio = Path.GetDirectoryName(rutaArchivo);
        if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        await File.WriteAllTextAsync(rutaArchivo, contenido, Encoding.UTF8);
    }

    public async Task<string> ExportarATango(List<Models.Entities.Recibo> recibos)
    {
        var sb = new StringBuilder();

        // Encabezado del archivo
        if (recibos.Any())
        {
            var primerRecibo = recibos.First();
            sb.AppendLine($"H|{primerRecibo.Periodo}|{primerRecibo.Quincena}|{recibos.Count}|{recibos.Sum(r => r.TotalBruto):F2}|{recibos.Sum(r => r.TotalDeducciones):F2}|{recibos.Sum(r => r.TotalNeto):F2}");
        }

        // Datos por empleado
        foreach (var recibo in recibos.OrderBy(r => r.Empleado.Legajo))
        {
            // Línea de empleado
            sb.AppendLine($"E|{recibo.Empleado.Legajo}|{recibo.Empleado.Cuil}|{recibo.Empleado.Nombre}|{recibo.Empleado.Apellido}|{recibo.TotalBruto:F2}|{recibo.TotalDeducciones:F2}|{recibo.TotalNeto:F2}|{recibo.Empleado.TipoLiquidacion?.Codigo ?? "QUINCENAL"}");

            // Líneas de conceptos
            foreach (var detalle in recibo.Detalles.OrderBy(d => d.Concepto.Codigo))
            {
                sb.AppendLine($"D|{recibo.Empleado.Legajo}|{detalle.Concepto.Codigo}|{detalle.Unidades:F2}|{(detalle.ValorUnitario.HasValue ? detalle.ValorUnitario.Value.ToString("F2") : "")}|{detalle.Subtotal:F2}");
            }
        }

        // Línea de totales
        sb.AppendLine($"T|{recibos.Count}|{recibos.Sum(r => r.TotalBruto):F2}|{recibos.Sum(r => r.TotalDeducciones):F2}|{recibos.Sum(r => r.TotalNeto):F2}");

        return await Task.FromResult(sb.ToString());
    }

    public async Task<MemoryStream> GenerarExcelTangoAsync(List<Models.Entities.Recibo> recibos)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Liquidacion");

        // Conceptos que van en ValorEntero (en lugar de Monto)
        var conceptosValorEntero = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "HsNormales",
            "HsExt",
            "HsExt50",
            "HsExt100",
            "Fenix",
            "PlusFenix",
            "PlusElectrico",
            "Presentismo"
        };

        int filaActual = 1;

        // Procesar cada recibo
        foreach (var recibo in recibos.OrderBy(r => r.Empleado.Legajo))
        {
            // Determinar la fecha según el período y quincena
            DateTime fecha;
            if (!string.IsNullOrEmpty(recibo.Periodo))
            {
                var partes = recibo.Periodo.Split('-');
                if (partes.Length == 2 && int.TryParse(partes[0], out int anio) && int.TryParse(partes[1], out int mes))
                {
                    // Si es quincena 1, usar día 15, si es quincena 2, usar último día del mes
                    int dia = recibo.Quincena == 1 ? 15 : DateTime.DaysInMonth(anio, mes);
                    fecha = new DateTime(anio, mes, dia);
                }
                else
                {
                    fecha = DateTime.Now; // Fallback
                }
            }
            else
            {
                fecha = DateTime.Now; // Fallback
            }

            // Procesar cada detalle del recibo
            foreach (var detalle in recibo.Detalles.OrderBy(d => d.Concepto.Codigo))
            {
                // Determinar si el concepto va en ValorEntero o Monto
                bool esValorEntero = conceptosValorEntero.Contains(detalle.Concepto.Codigo);

                // Columna A: Fecha
                worksheet.Cell(filaActual, 1).Value = fecha;
                worksheet.Cell(filaActual, 1).Style.DateFormat.Format = "dd/MM/yyyy";

                // Columna B: Legajo
                worksheet.Cell(filaActual, 2).Value = recibo.Empleado.Legajo;

                // Columna C: CodigoConcepto
                worksheet.Cell(filaActual, 3).Value = detalle.Concepto.Codigo;

                // Columna D: ValorEntero (solo para conceptos específicos)
                if (esValorEntero)
                {
                    worksheet.Cell(filaActual, 4).Value = detalle.Unidades;
                    worksheet.Cell(filaActual, 4).Style.NumberFormat.Format = "0.00";
                }
                else
                {
                    worksheet.Cell(filaActual, 4).Value = "";
                }

                // Columna E: Monto (para el resto de conceptos)
                if (!esValorEntero)
                {
                    worksheet.Cell(filaActual, 5).Value = detalle.Subtotal;
                    worksheet.Cell(filaActual, 5).Style.NumberFormat.Format = "0.00";
                }
                else
                {
                    worksheet.Cell(filaActual, 5).Value = "";
                }

                filaActual++;
            }
        }

        // Ajustar anchos de columna
        worksheet.Column(1).Width = 12; // Fecha
        worksheet.Column(2).Width = 10; // Legajo
        worksheet.Column(3).Width = 20; // CodigoConcepto
        worksheet.Column(4).Width = 12; // ValorEntero
        worksheet.Column(5).Width = 12; // Monto

        // Guardar en MemoryStream
        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return await Task.FromResult(stream);
    }
}