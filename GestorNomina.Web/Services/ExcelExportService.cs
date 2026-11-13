using ClosedXML.Excel;
using GestorNomina.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GestorNomina.Web.Services;

/// <summary>
/// Servicio para la exportación de datos a Excel
/// </summary>
public class ExcelExportService : IExcelExportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExcelExportService> _logger;

    public ExcelExportService(ApplicationDbContext context, ILogger<ExcelExportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<byte[]> GenerarExcelEmpleadosProyectoAsync(int proyectoId, int mes, int anio, int quincena)
    {
        try
        {
            // Obtener datos del proyecto
            var proyecto = await _context.Proyectos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
                throw new Exception("Proyecto no encontrado");

            // Obtener empleados del proyecto
            var empleados = await _context.Empleados
                .AsNoTracking()
                .Include(e => e.TipoLiquidacion)
                .Include(e => e.Categoria)
                .Include(e => e.Disciplina)
                .Where(e => e.ProyectoId == proyectoId && e.Activo)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            // Crear workbook
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Empleados");

            // Configurar nombre del mes
            var nombreMes = new CultureInfo("es-ES").DateTimeFormat.GetMonthName(mes).ToUpper();
            
            // Determinar días según quincena
            int diaInicio = quincena == 1 ? 1 : 16;
            int diaFin = quincena == 1 ? 15 : DateTime.DaysInMonth(anio, mes);
            int cantidadDias = diaFin - diaInicio + 1;

            // TÍTULO DEL EXCEL (quincena del mes) - FILA 1
            var tituloQuincena = quincena == 1 ? "1Q" : "2Q";
            worksheet.Cell(1, 1).Value = $"{tituloQuincena} {nombreMes}";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 10 + cantidadDias).Merge();
            worksheet.Range(1, 1, 1, 10 + cantidadDias).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // FILA 2 - Letras de los días de la semana
            int rowDiasSemana = 2;
            for (int dia = diaInicio; dia <= diaFin; dia++)
            {
                var fecha = new DateTime(anio, mes, dia);
                var diaSemana = fecha.ToString("ddd", new CultureInfo("es-ES")).Substring(0, 1).ToUpper();
                
                int colIndex = 10 + (dia - diaInicio); // Columna J en adelante
                
                worksheet.Cell(rowDiasSemana, colIndex).Value = diaSemana;
                worksheet.Cell(rowDiasSemana, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(rowDiasSemana, colIndex).Style.Font.Bold = true;
                worksheet.Cell(rowDiasSemana, colIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // FILA 3 - ENCABEZADOS
            int rowHeader = 3;
            
            // Encabezados fijos (columnas A-I)
            worksheet.Cell(rowHeader, 1).Value = "LEGAJO";
            worksheet.Cell(rowHeader, 2).Value = "APELLIDO";
            worksheet.Cell(rowHeader, 3).Value = "NOMBRE";
            worksheet.Cell(rowHeader, 4).Value = "TIPO CONTRATACION";
            worksheet.Cell(rowHeader, 5).Value = "INGRESO";
            worksheet.Cell(rowHeader, 6).Value = "CUIL";
            worksheet.Cell(rowHeader, 7).Value = "AFILIADO";
            worksheet.Cell(rowHeader, 8).Value = "DISCIPLINA";
            worksheet.Cell(rowHeader, 9).Value = "CATEGORIA";

            // Números de los días (columna J en adelante)
            for (int dia = diaInicio; dia <= diaFin; dia++)
            {
                int colIndex = 10 + (dia - diaInicio);
                worksheet.Cell(rowHeader, colIndex).Value = dia;
                worksheet.Cell(rowHeader, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Columna TOTAL al final de los días
            int colTotal = 10 + cantidadDias;
            worksheet.Cell(rowHeader, colTotal).Value = "TOTAL";

            // Estilo de encabezados (filas 2 y 3)
            var headerRange = worksheet.Range(rowDiasSemana, 1, rowHeader, colTotal);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // DATOS DE EMPLEADOS
            int currentRow = rowHeader + 1;
            foreach (var empleado in empleados)
            {
                // Columnas A-F: Datos básicos
                worksheet.Cell(currentRow, 1).Value = empleado.Legajo;
                worksheet.Cell(currentRow, 2).Value = empleado.Apellido.ToUpper();
                worksheet.Cell(currentRow, 3).Value = empleado.Nombre.ToUpper();
                worksheet.Cell(currentRow, 4).Value = empleado.TipoLiquidacion?.Nombre ?? "N/A";
                worksheet.Cell(currentRow, 5).Value = empleado.FechaIngreso?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cell(currentRow, 6).Value = empleado.Cuil ?? "";
                
                // Columna G: AFILIADO (vacía para completar)
                worksheet.Cell(currentRow, 7).Value = "";
                
                // Columna H: DISCIPLINA
                worksheet.Cell(currentRow, 8).Value = empleado.Disciplina?.Nombre ?? "";
                
                // Columna I: CATEGORIA
                worksheet.Cell(currentRow, 9).Value = empleado.Categoria?.Nombre ?? "";

                // Columnas J en adelante: Días (vacíos para que el usuario los llene)
                for (int dia = diaInicio; dia <= diaFin; dia++)
                {
                    int colIndex = 10 + (dia - diaInicio);
                    worksheet.Cell(currentRow, colIndex).Value = "";
                    worksheet.Cell(currentRow, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Última columna: TOTAL (vacío)
                worksheet.Cell(currentRow, colTotal).Value = "";
                worksheet.Cell(currentRow, colTotal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Estilo de fila
                var dataRange = worksheet.Range(currentRow, 1, currentRow, colTotal);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow++;
            }

            // Ajustar anchos de columna
            worksheet.Column(1).Width = 10;  // LEGAJO
            worksheet.Column(2).Width = 15;  // APELLIDO
            worksheet.Column(3).Width = 15;  // NOMBRE
            worksheet.Column(4).Width = 20;  // TIPO CONTRATACION
            worksheet.Column(5).Width = 12;  // INGRESO
            worksheet.Column(6).Width = 13;  // CUIL
            worksheet.Column(7).Width = 10;  // AFILIADO
            worksheet.Column(8).Width = 15;  // DISCIPLINA
            worksheet.Column(9).Width = 15;  // CATEGORIA

            // Días - columnas más estrechas (columna J en adelante)
            for (int i = 0; i < cantidadDias; i++)
            {
                worksheet.Column(10 + i).Width = 5;
            }

            // Columna TOTAL
            worksheet.Column(colTotal).Width = 8;

            // Congelar paneles (filas de encabezado y primeras 3 columnas)
            worksheet.SheetView.FreezeRows(rowHeader);
            worksheet.SheetView.FreezeColumns(3);

            // Guardar a memoria
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar Excel de empleados para proyecto {ProyectoId}", proyectoId);
            throw;
        }
    }
}
