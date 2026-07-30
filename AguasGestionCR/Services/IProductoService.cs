using AguasGestionCR.Models;

namespace AguasGestionCR.Services
{
    /// <summary>
    /// Define las operaciones disponibles para administrar el inventario.
    /// La interfaz funciona como contrato entre la ventana WPF y la lógica
    /// que accede a Entity Framework y SQL Server.
    /// </summary>
    public interface IProductoService
    {
        List<Producto> ObtenerProductos(
            string? busqueda = null,
            string? categoria = null,
            string? estado = null,
            bool soloStockBajo = false);

        Producto? ObtenerProductoPorId(int productoId);

        (bool Exito, string Mensaje) CrearProducto(Producto producto);

        (bool Exito, string Mensaje) ActualizarProducto(Producto producto);

        (bool Exito, string Mensaje) CambiarEstadoProducto(
            int productoId,
            string nuevoEstado);
    }
}
