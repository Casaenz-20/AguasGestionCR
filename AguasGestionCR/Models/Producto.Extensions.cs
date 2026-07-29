using System.ComponentModel.DataAnnotations.Schema;

namespace AguasGestionCR.Models;

public partial class Producto
{
    [NotMapped]
    public bool StockBajo => Cantidad <= CantidadMinima;

    [NotMapped]
    public string EstadoStock => StockBajo ? "Stock bajo" : "Disponible";
}
