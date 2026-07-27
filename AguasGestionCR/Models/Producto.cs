using System;
using System.Collections.Generic;

namespace AguasGestionCR.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string CodigoProducto { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Cantidad { get; set; }

    public decimal CantidadMinima { get; set; }

    public string Unidad { get; set; } = null!;

    public DateOnly FechaIngreso { get; set; }

    public string Estado { get; set; } = null!;
}
