using System.Text;
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
        foreach (var detalle in recibo.Detalles.OrderBy(d => d.Concepto.CodigoTango))
        {
            // Formato: [Legajo];[CodigoConcepto];[Unidades];[ImporteCalculado]
            var linea = $"{recibo.Empleado.Legajo};{detalle.Concepto.CodigoTango};{detalle.Unidades:F2};{detalle.Subtotal:F2}";
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
            foreach (var detalle in recibo.Detalles.OrderBy(d => d.Concepto.CodigoTango))
            {
                var linea = $"{recibo.Empleado.Legajo};{detalle.Concepto.CodigoTango};{detalle.Unidades:F2};{detalle.Subtotal:F2}";
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
            foreach (var detalle in recibo.Detalles.OrderBy(d => d.Concepto.CodigoTango))
            {
                sb.AppendLine($"D|{recibo.Empleado.Legajo}|{detalle.Concepto.CodigoTango}|{detalle.Unidades:F2}|{(detalle.ValorUnitario.HasValue ? detalle.ValorUnitario.Value.ToString("F2") : "")}|{detalle.Subtotal:F2}");
            }
        }

        // Línea de totales
        sb.AppendLine($"T|{recibos.Count}|{recibos.Sum(r => r.TotalBruto):F2}|{recibos.Sum(r => r.TotalDeducciones):F2}|{recibos.Sum(r => r.TotalNeto):F2}");

        return await Task.FromResult(sb.ToString());
    }
}