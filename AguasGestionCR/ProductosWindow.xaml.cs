
using AguasGestionCR.Models;
using AguasGestionCR.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AguasGestionCR;

/// <summary>
/// Ventana administrativa del inventario.
/// La interfaz obtiene y modifica los datos por medio de IProductoService;
/// no accede directamente a Entity Framework ni a SQL Server.
/// </summary>
public partial class ProductosWindow : Window
{
    private readonly IProductoService _productoService;
    private List<Producto> _productosMostrados = new();
    private bool _consultaRealizada;

    public ProductosWindow()
        : this(new ProductoService())
    {
    }

    internal ProductosWindow(IProductoService productoService)
    {
        InitializeComponent();
        _productoService = productoService;
    }

    private Producto? ProductoSeleccionado =>
        DgProductos.SelectedItem as Producto;

    private void Window_Loaded(
    object sender,
    RoutedEventArgs e)
    {
        TxtFechaActual.Text =
            DateTime.Now.ToString("dd/MM/yyyy");

        CargarFiltrosVisuales();
        PrepararListadoVacio();

        try
        {
            ActualizarResumen();
        }
        catch (Exception ex)
        {
            LimpiarResumen();

            MessageBox.Show(
                "No fue posible cargar el resumen del inventario.\n\n" +
                ex.GetBaseException().Message,
                "Error de inventario",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void CargarFiltrosVisuales()
    {
        CmbCategoria.ItemsSource = new List<string>
        {
            "Todas",
            "Materiales hidráulicos",
            "Medición y Control",
            "Válvulas y Control",
            "Consumibles",
            "Químicos y Tratamiento",
            "Herramientas",
            "Otros"
        };

        CmbCategoria.SelectedIndex = 0;
        CmbEstado.SelectedIndex = 0;
    }

    /// <summary>
    /// Consulta SQL Server aplicando los filtros visibles y actualiza
    /// la tabla, las tarjetas de resumen y los botones de selección.
    /// </summary>
    private void CargarProductos(int? productoIdSeleccionar = null)
    {
        try
        {
            string busqueda = TxtBusqueda.Text.Trim();
            string categoria = CmbCategoria.SelectedItem?.ToString() ?? "Todas";
            string estado = ObtenerContenidoCombo(CmbEstado) ?? "Todos";
            bool soloStockBajo = ChkSoloStockBajo.IsChecked == true;

            _productosMostrados = _productoService.ObtenerProductos(
                busqueda,
                categoria,
                estado,
                soloStockBajo);

            DgProductos.ItemsSource = _productosMostrados;

            if (productoIdSeleccionar.HasValue)
            {
                Producto? producto = _productosMostrados.FirstOrDefault(
                    item => item.ProductoId == productoIdSeleccionar.Value);

                if (producto != null)
                {
                    DgProductos.SelectedItem = producto;
                    DgProductos.ScrollIntoView(producto);
                }
            }

            ActualizarResultadoListado();
            ActualizarResumen();
            ActualizarBotonesSeleccion();
        }
        catch (Exception ex)
        {
            DgProductos.ItemsSource = null;
            _productosMostrados.Clear();
            ActualizarResultadoListado();
            LimpiarResumen();
            ActualizarBotonesSeleccion();

            MessageBox.Show(
                "No fue posible cargar el inventario desde la base de datos.\n\n" +
                ex.GetBaseException().Message,
                "Error al cargar productos",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static string? ObtenerContenidoCombo(ComboBox comboBox)
    {
        return comboBox.SelectedItem switch
        {
            ComboBoxItem item => item.Content?.ToString(),
            string texto => texto,
            _ => null
        };
    }

    private void AplicarFiltros()
    {
        DgProductos.SelectedItem = null;

        _consultaRealizada = true;

        CargarProductos();
    }

    private void ActualizarResultadoListado()
    {
         int cantidadVisible =
        _productosMostrados.Count;

        if (cantidadVisible == 0)
        {
            TxtResultadoListado.Text =
                "No se encontraron productos con los criterios seleccionados.";

            return;
        }

        TxtResultadoListado.Text =
            cantidadVisible == 1
                ? "Mostrando 1 producto"
                : $"Mostrando {cantidadVisible} productos";
    }

    /// <summary>
    /// Las tarjetas muestran el estado general del inventario, no solamente
    /// las filas filtradas que aparecen en la tabla.
    /// </summary>
    private void ActualizarResumen()
    {
        List<Producto> todos = _productoService.ObtenerProductos();

        TxtTotalProductos.Text = todos.Count.ToString();
        TxtProductosActivos.Text = todos.Count(producto =>
            producto.Estado == "Activo").ToString();
        TxtStockBajo.Text = todos.Count(producto =>
            producto.Estado == "Activo" && producto.StockBajo).ToString();
        TxtProductosInactivos.Text = todos.Count(producto =>
            producto.Estado == "Inactivo").ToString();
    }

    private void LimpiarResumen()
    {
        TxtTotalProductos.Text = "0";
        TxtProductosActivos.Text = "0";
        TxtStockBajo.Text = "0";
        TxtProductosInactivos.Text = "0";
    }

    private void ActualizarBotonesSeleccion()
    {
        Producto? producto = ProductoSeleccionado;
        bool haySeleccion = producto != null;

        BtnEditar.IsEnabled = haySeleccion;
        BtnCambiarEstado.IsEnabled = haySeleccion;

        if (!haySeleccion || producto!.Estado == "Activo")
        {
            BtnCambiarEstado.Content = "Inactivar";
            BtnCambiarEstado.Style =
                (Style)FindResource("DangerButtonStyle");
        }
        else
        {
            BtnCambiarEstado.Content = "Reactivar";
            BtnCambiarEstado.Style =
                (Style)FindResource("PrimaryButtonStyle");
        }
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        AplicarFiltros();
    }

    private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AplicarFiltros();
        }
    }

    private void BtnLimpiarFiltros_Click(
    object sender,
    RoutedEventArgs e)
    {
        TxtBusqueda.Clear();
        CmbCategoria.SelectedIndex = 0;
        CmbEstado.SelectedIndex = 0;
        ChkSoloStockBajo.IsChecked = false;

        PrepararListadoVacio();
        TxtBusqueda.Focus();
    }

    private void BtnActualizar_Click(
    object sender,
    RoutedEventArgs e)
    {
        int? productoId =
        ProductoSeleccionado?.ProductoId;

        _consultaRealizada = true;

        CargarProductos(productoId);
    }


  

    private void PrepararListadoVacio()
    {
        _consultaRealizada = false;

        _productosMostrados =
            new List<Producto>();

        DgProductos.ItemsSource = null;
        DgProductos.SelectedItem = null;

        TxtResultadoListado.Text =
            "Presione Buscar para consultar todos los productos " +
            "o seleccione los filtros que desea aplicar.";

        ActualizarBotonesSeleccion();
    }





    

    private void BtnCambiarEstado_Click(object sender, RoutedEventArgs e)
    {
        Producto? producto = ProductoSeleccionado;

        if (producto == null)
        {
            return;
        }

        string nuevoEstado = producto.Estado == "Activo"
            ? "Inactivo"
            : "Activo";

        string accion = nuevoEstado == "Activo"
            ? "reactivar"
            : "inactivar";

        MessageBoxResult confirmacion = MessageBox.Show(
            $"¿Desea {accion} el producto {producto.CodigoProducto} - {producto.Nombre}?",
            "Confirmar cambio de estado",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmacion != MessageBoxResult.Yes)
        {
            return;
        }

        (bool exito, string mensaje) =
            _productoService.CambiarEstadoProducto(
                producto.ProductoId,
                nuevoEstado);

        MessageBox.Show(
            mensaje,
            exito ? "Estado actualizado" : "No se pudo cambiar el estado",
            MessageBoxButton.OK,
            exito ? MessageBoxImage.Information : MessageBoxImage.Warning);

        if (exito)
        {
            CargarProductos(producto.ProductoId);
        }
    }

    private void DgProductos_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        ActualizarBotonesSeleccion();
    }

    // Estos métodos se conservan para una posible barra lateral futura.
    private void BtnInicio_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

   

    private void BtnNuevo_Click(
    object sender,
    RoutedEventArgs e)
    {
        bool registrarOtro;

        do
        {
            var formulario =
                new ProductoFormWindow
                {
                    Owner = this
                };

            bool? resultadoFormulario =
                formulario.ShowDialog();

            if (resultadoFormulario != true ||
                formulario.ProductoResultado == null)
            {
                return;
            }

            Producto nuevoProducto =
                formulario.ProductoResultado;

            (bool exito, string mensaje) =
                _productoService.CrearProducto(
                    nuevoProducto);

            if (!exito)
            {
                MessageBox.Show(
                    mensaje,
                    "No se pudo registrar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (_consultaRealizada)
            {
                CargarProductos(
                    nuevoProducto.ProductoId);
            }
            else
            {
                PrepararListadoVacio();
                ActualizarResumen();
            }

            MessageBoxResult respuesta =
                MessageBox.Show(
                    mensaje +
                    "\n\n¿Desea registrar otro producto?",
                    "Producto registrado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

            registrarOtro =
                respuesta == MessageBoxResult.Yes;
        }
        while (registrarOtro);
    }

    private void BtnEditar_Click(
    object sender,
    RoutedEventArgs e)
    {
        AbrirEdicion();
    }

    private void AbrirEdicion()
    {
        Producto? seleccionado =
            ProductoSeleccionado;

        if (seleccionado == null)
        {
            MessageBox.Show(
                "Seleccione un producto para editarlo.",
                "Producto requerido",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        Producto? productoActual;

        try
        {
            productoActual =
                _productoService.ObtenerProductoPorId(
                    seleccionado.ProductoId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "No fue posible consultar el producto.\n\n" +
                ex.GetBaseException().Message,
                "Error de inventario",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        if (productoActual == null)
        {
            MessageBox.Show(
                "El producto seleccionado ya no existe.",
                "Producto no encontrado",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            AplicarFiltros();
            return;
        }

        var formulario =
            new ProductoFormWindow(productoActual)
            {
                Owner = this
            };

        if (formulario.ShowDialog() != true ||
            formulario.ProductoResultado == null)
        {
            return;
        }

        (bool exito, string mensaje) =
            _productoService.ActualizarProducto(
                formulario.ProductoResultado);

        MessageBox.Show(
            mensaje,
            exito
                ? "Producto actualizado"
                : "No se pudo actualizar",
            MessageBoxButton.OK,
            exito
                ? MessageBoxImage.Information
                : MessageBoxImage.Warning);

        if (exito)
        {
            CargarProductos(
                productoActual.ProductoId);
        }
    }

    private void DgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ProductoSeleccionado != null)
        {
            AbrirEdicion();
        }
    }

    private void ChkSoloStockBajo_Changed(
    object sender,
    RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        AplicarFiltros();
    }
}
