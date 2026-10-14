using GestorNomina.Web.Models.Entities;
using GestorNomina.Web.Models.Enums;

namespace GestorNomina.Web.Services.Liquidacion.Payroll;

/// <summary>
/// Contexto de ejecución del motor de liquidación
/// </summary>
public class PayrollContext
{
    public Empleado Empleado { get; set; } = null!;
    public string Periodo { get; set; } = string.Empty;
    public int Quincena { get; set; }
    public List<HoraRegistroAprobado> Registros { get; set; } = new();

    // Totalizadores de Horas
    public decimal HSNOR { get; set; } // Horas Normales
    public decimal HSEXT { get; set; } // Horas Extras 50%
    public decimal HS100 { get; set; } // Horas Extras 100%
    public decimal HSNOCT { get; set; } // Horas Nocturnas Normales
    public decimal HSNOCT50 { get; set; } // Horas Nocturnas Extras 50%
    public decimal HSNOCT100 { get; set; } // Horas Nocturnas Extras 100%
    public decimal DIAS { get; set; } // Días trabajados

    // Contadores de Novedades
    public int DiasFeriado { get; set; }
    public int DiasAusenciasInjustificadas { get; set; }

    // Conceptos de Liquidación
    public List<ItemLiquidacion> ItemsRemunerativos { get; set; } = new();
    public List<ItemLiquidacion> ItemsNoRemunerativos { get; set; } = new();
    public List<ItemLiquidacion> ItemsDeducciones { get; set; } = new();

    // Totales
    public decimal TotalBrutoRemunerativo { get; set; }
    public decimal TotalBrutoNoRemunerativo { get; set; }
    public decimal TotalBruto { get; set; }
    public decimal TotalDeducciones { get; set; }
    public decimal TotalNeto { get; set; }

    // Control de Neto Convenido
    public decimal? NetoConvenidoObjetivo { get; set; }
    public decimal? DiferenciaControl { get; set; }
    public int NetoRedondeado { get; set; }
}

/// <summary>
/// Item de liquidación individual
/// </summary>
public class ItemLiquidacion
{
    public CodigoConceptoTango Concepto { get; set; }
    public decimal Unidades { get; set; }
    public decimal? ValorUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
