using AguasGestionCR.Models;
using Microsoft.EntityFrameworkCore;

namespace AguasGestionCR.Services
{
    /// <summary>
    /// Implementa el CRUD de productos utilizando Entity Framework Core.
    /// Mantiene la lógica de base de datos fuera de las ventanas WPF.
    /// </summary>
    public class ProductoService : IProductoService
    {
        public List<Producto> ObtenerProductos(
            string? busqueda = null,
            string? categoria = null,
            string? estado = null,
            bool soloStockBajo = false)
        {
            using var db = new AcueductoDbContext();

            IQueryable<Producto> consulta = db.Productos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                string texto = busqueda.Trim();

                consulta = consulta.Where(producto =>
                    producto.CodigoProducto.Contains(texto) ||
                    producto.Nombre.Contains(texto));
            }

            if (!string.IsNullOrWhiteSpace(categoria) &&
                !categoria.Equals("Todas", StringComparison.OrdinalIgnoreCase))
            {
                consulta = consulta.Where(producto =>
                    producto.Categoria == categoria);
            }

            if (!string.IsNullOrWhiteSpace(estado) &&
                !estado.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                consulta = consulta.Where(producto =>
                    producto.Estado == estado);
            }

            if (soloStockBajo)
            {
                consulta = consulta.Where(producto =>
                           producto.Estado == "Activo" &&
                           producto.Cantidad <= producto.CantidadMinima);
            }

            return consulta
                .OrderBy(producto => producto.CodigoProducto)
                .ToList();
        }

        public Producto? ObtenerProductoPorId(int productoId)
        {
            using var db = new AcueductoDbContext();

            return db.Productos
                .AsNoTracking()
                .FirstOrDefault(producto =>
                    producto.ProductoId == productoId);
        }

        public (bool Exito, string Mensaje) CrearProducto(Producto producto)
        {
            string? error = ValidarProducto(producto);

            if (error != null)
            {
                return (false, error);
            }

            try
            {
                using var db = new AcueductoDbContext();

                NormalizarProducto(producto);

                bool codigoDuplicado = db.Productos.Any(existente =>
                    existente.CodigoProducto == producto.CodigoProducto);

                if (codigoDuplicado)
                {
                    return (false,
                        "Ya existe un producto registrado con ese código.");
                }

                producto.ProductoId = 0;

                db.Productos.Add(producto);
                db.SaveChanges();

                return (true, "Producto registrado correctamente.");
            }
            catch (DbUpdateException)
            {
                return (false,
                    "No fue posible guardar el producto. Verifique que el código no esté repetido y que los datos sean válidos.");
            }
            catch (Exception)
            {
                return (false,
                    "Ocurrió un error inesperado al registrar el producto.");
            }
        }

        public (bool Exito, string Mensaje) ActualizarProducto(
            Producto producto)
        {
            string? error = ValidarProducto(producto);

            if (error != null)
            {
                return (false, error);
            }

            try
            {
                using var db = new AcueductoDbContext();

                NormalizarProducto(producto);

                Producto? productoExistente = db.Productos.FirstOrDefault(
                    existente => existente.ProductoId == producto.ProductoId);

                if (productoExistente == null)
                {
                    return (false,
                        "El producto que desea modificar ya no existe.");
                }

                bool codigoDuplicado = db.Productos.Any(existente =>
                    existente.CodigoProducto == producto.CodigoProducto &&
                    existente.ProductoId != producto.ProductoId);

                if (codigoDuplicado)
                {
                    return (false,
                        "Otro producto ya utiliza ese código.");
                }

                productoExistente.CodigoProducto = producto.CodigoProducto;
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Categoria = producto.Categoria;
                productoExistente.Descripcion = producto.Descripcion;
                productoExistente.Cantidad = producto.Cantidad;
                productoExistente.CantidadMinima = producto.CantidadMinima;
                productoExistente.Unidad = producto.Unidad;
                productoExistente.FechaIngreso = producto.FechaIngreso;
                productoExistente.Estado = producto.Estado;

                db.SaveChanges();

                return (true, "Producto actualizado correctamente.");
            }
            catch (DbUpdateException)
            {
                return (false,
                    "No fue posible actualizar el producto. Verifique que el código no esté repetido y que los datos sean válidos.");
            }
            catch (Exception)
            {
                return (false,
                    "Ocurrió un error inesperado al actualizar el producto.");
            }
        }

        public (bool Exito, string Mensaje) CambiarEstadoProducto(
            int productoId,
            string nuevoEstado)
        {
            if (nuevoEstado != "Activo" && nuevoEstado != "Inactivo")
            {
                return (false, "El estado indicado no es válido.");
            }

            try
            {
                using var db = new AcueductoDbContext();

                Producto? producto = db.Productos.FirstOrDefault(
                    existente => existente.ProductoId == productoId);

                if (producto == null)
                {
                    return (false,
                        "El producto seleccionado ya no existe.");
                }

                producto.Estado = nuevoEstado;
                db.SaveChanges();

                string accion = nuevoEstado == "Activo"
                    ? "reactivado"
                    : "inactivado";

                return (true,
                    $"Producto {accion} correctamente.");
            }
            catch (DbUpdateException)
            {
                return (false,
                    "No fue posible cambiar el estado del producto.");
            }
            catch (Exception)
            {
                return (false,
                    "Ocurrió un error inesperado al cambiar el estado.");
            }
        }

        private static string? ValidarProducto(Producto? producto)
        {
            if (producto == null)
            {
                return "No se recibió la información del producto.";
            }

            if (string.IsNullOrWhiteSpace(producto.CodigoProducto))
            {
                return "El código del producto es obligatorio.";
            }

            if (producto.CodigoProducto.Trim().Length > 50)
            {
                return "El código no puede superar los 50 caracteres.";
            }

            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                return "El nombre del producto es obligatorio.";
            }

            if (producto.Nombre.Trim().Length > 100)
            {
                return "El nombre no puede superar los 100 caracteres.";
            }

            if (string.IsNullOrWhiteSpace(producto.Categoria))
            {
                return "Debe seleccionar una categoría.";
            }

            if (producto.Categoria.Trim().Length > 50)
            {
                return "La categoría no puede superar los 50 caracteres.";
            }

            if (producto.Cantidad < 0)
            {
                return "La cantidad no puede ser negativa.";
            }

            if (producto.CantidadMinima < 0)
            {
                return "La cantidad mínima no puede ser negativa.";
            }

            if (string.IsNullOrWhiteSpace(producto.Unidad))
            {
                return "Debe seleccionar una unidad.";
            }

            if (producto.Unidad.Trim().Length > 30)
            {
                return "La unidad no puede superar los 30 caracteres.";
            }

            if (producto.FechaIngreso == default)
            {
                return "Debe seleccionar la fecha de ingreso.";
            }

            DateOnly fechaActual = DateOnly.FromDateTime(DateTime.Today);

            if (producto.FechaIngreso > fechaActual)
            {
                return "La fecha de ingreso no puede ser futura.";
            }

            if (producto.Estado != "Activo" &&
                producto.Estado != "Inactivo")
            {
                return "El estado debe ser Activo o Inactivo.";
            }

            return null;
        }

        private static void NormalizarProducto(Producto producto)
        {
            producto.CodigoProducto = producto.CodigoProducto
                .Trim()
                .ToUpperInvariant();

            producto.Nombre = producto.Nombre.Trim();
            producto.Categoria = producto.Categoria.Trim();
            producto.Unidad = producto.Unidad.Trim();
            producto.Estado = producto.Estado.Trim();

            producto.Descripcion = string.IsNullOrWhiteSpace(
                producto.Descripcion)
                ? null
                : producto.Descripcion.Trim();
        }
    }
}
