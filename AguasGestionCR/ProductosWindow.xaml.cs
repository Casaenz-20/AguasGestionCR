using AcueductoApp.Views;
using AguasGestionCR.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AguasGestionCR;

/// <summary>
/// Prototipo visual temporal del módulo de inventario.
/// Trabaja en memoria para permitir diseñar y probar la interfaz
/// antes de conectar IProductoService y SQL Server.
/// </summary>
public partial class ProductosWindow : Window
{
    private readonly ObservableCollection<Producto> _productos = new();
    private ICollectionView? _vistaProductos;

    public ProductosWindow()
    {
        InitializeComponent();
    }

    private Producto? ProductoSeleccionado =>
        DgProductos.SelectedItem as Producto;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        TxtFechaActual.Text = DateTime.Now.ToString("dd/MM/yyyy");
        CargarFiltrosVisuales();
        CargarDatosDemostracion();
        ConfigurarVistaFiltrada();
        ActualizarResumen();
        ActualizarBotonesSeleccion();
    }

    private void CargarFiltrosVisuales()
    {
        CmbCategoria.ItemsSource = new List<string>
        {
            "Todas",
            "Materiales hidráulicos",
            "Medición y control",
            "Válvulas y control",
            "Consumibles",
            "Químicos y tratamiento",
            "Herramientas",
            "Otros"
        };

        CmbCategoria.SelectedIndex = 0;
        CmbEstado.SelectedIndex = 0;
    }

    /// <summary>
    /// Datos temporales únicamente para poder visualizar el diseño.
    /// Se eliminarán cuando ProductosWindow use ProductoService.
    /// </summary>
    private void CargarDatosDemostracion()
    {
        if (_productos.Count > 0)
        {
            return;
        }

        _productos.Add(new Producto
        {
            ProductoId = 1,
            CodigoProducto = "PRD-000123",
            Nombre = "Tubería PVC 1/2 pulgada",
            Categoria = "Materiales hidráulicos",
            Descripcion = "Tubería para conexiones domiciliarias.",
            Cantidad = 12m,
            CantidadMinima = 20m,
            Unidad = "Metros (m)",
            FechaIngreso = DateOnly.FromDateTime(DateTime.Today.AddDays(-35)),
            Estado = "Activo"
        });

        _productos.Add(new Producto
        {
            ProductoId = 2,
            CodigoProducto = "PRD-000124",
            Nombre = "Medidor de agua 1/2 pulgada",
            Categoria = "Medición y control",
            Descripcion = "Medidor residencial.",
            Cantidad = 48m,
            CantidadMinima = 15m,
            Unidad = "Unidades",
            FechaIngreso = DateOnly.FromDateTime(DateTime.Today.AddDays(-28)),
            Estado = "Activo"
        });

        _productos.Add(new Producto
        {
            ProductoId = 3,
            CodigoProducto = "PRD-000125",
            Nombre = "Válvula de compuerta",
            Categoria = "Válvulas y control",
            Descripcion = "Válvula para aislamiento de línea.",
            Cantidad = 7m,
            CantidadMinima = 10m,
            Unidad = "Unidades",
            FechaIngreso = DateOnly.FromDateTime(DateTime.Today.AddDays(-21)),
            Estado = "Activo"
        });

        _productos.Add(new Producto
        {
            ProductoId = 4,
            CodigoProducto = "PRD-000126",
            Nombre = "Cinta teflón",
            Categoria = "Consumibles",
            Descripcion = "Sellado de conexiones roscadas.",
            Cantidad = 85m,
            CantidadMinima = 25m,
            Unidad = "Unidades",
            FechaIngreso = DateOnly.FromDateTime(DateTime.Today.AddDays(-14)),
            Estado = "Activo"
        });

        _productos.Add(new Producto
        {
            ProductoId = 5,
            CodigoProducto = "PRD-000127",
            Nombre = "Llave ajustable",
            Categoria = "Herramientas",
            Descripcion = "Herramienta de mantenimiento.",
            Cantidad = 4m,
            CantidadMinima = 5m,
            Unidad = "Unidades",
            FechaIngreso = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            Estado = "Inactivo"
        });
    }

    private void ConfigurarVistaFiltrada()
    {
        _vistaProductos = CollectionViewSource.GetDefaultView(_productos);
        _vistaProductos.Filter = FiltrarProducto;
        DgProductos.ItemsSource = _vistaProductos;
        ActualizarResultadoListado();
    }

    private bool FiltrarProducto(object elemento)
    {
        if (elemento is not Producto producto)
        {
            return false;
        }

        string texto = TxtBusqueda.Text.Trim();
        string categoria = CmbCategoria.SelectedItem?.ToString() ?? "Todas";
        string estado = ObtenerContenidoCombo(CmbEstado) ?? "Todos";
        bool soloStockBajo = ChkSoloStockBajo.IsChecked == true;

        bool coincideTexto = string.IsNullOrWhiteSpace(texto) ||
            producto.CodigoProducto.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
            producto.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase);

        bool coincideCategoria = categoria == "Todas" ||
            producto.Categoria == categoria;

        bool coincideEstado = estado == "Todos" ||
            producto.Estado == estado;

        bool coincideStock = !soloStockBajo || producto.StockBajo;

        return coincideTexto &&
               coincideCategoria &&
               coincideEstado &&
               coincideStock;
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
        _vistaProductos?.Refresh();
        DgProductos.SelectedItem = null;
        ActualizarResultadoListado();
        ActualizarBotonesSeleccion();
    }

    private void ActualizarResultadoListado()
    {
        int cantidadVisible = 0;

        if (_vistaProductos != null)
        {
            foreach (object _ in _vistaProductos)
            {
                cantidadVisible++;
            }
        }

        TxtResultadoListado.Text = cantidadVisible == 1
            ? "Mostrando 1 producto"
            : $"Mostrando {cantidadVisible} productos";
    }

    private void ActualizarResumen()
    {
        TxtTotalProductos.Text = _productos.Count.ToString();
        TxtProductosActivos.Text = _productos.Count(p => p.Estado == "Activo").ToString();
        TxtStockBajo.Text = _productos.Count(p => p.Estado == "Activo" && p.StockBajo).ToString();
        TxtProductosInactivos.Text = _productos.Count(p => p.Estado == "Inactivo").ToString();
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
            BtnCambiarEstado.Style = (Style)FindResource("DangerButtonStyle");
        }
        else
        {
            BtnCambiarEstado.Content = "Reactivar";
            BtnCambiarEstado.Style = (Style)FindResource("PrimaryButtonStyle");
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

    private void BtnLimpiarFiltros_Click(object sender, RoutedEventArgs e)
    {
        TxtBusqueda.Clear();
        CmbCategoria.SelectedIndex = 0;
        CmbEstado.SelectedIndex = 0;
        ChkSoloStockBajo.IsChecked = false;
        AplicarFiltros();
    }

    private void BtnActualizar_Click(object sender, RoutedEventArgs e)
    {
        AplicarFiltros();
        ActualizarResumen();
    }

    private void BtnNuevo_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ProductoFormWindow
        {
            Owner = this
        };

        if (ventana.ShowDialog() == true && ventana.ProductoResultado != null)
        {
            Producto nuevo = ventana.ProductoResultado;
            nuevo.ProductoId = _productos.Count == 0
                ? 1
                : _productos.Max(p => p.ProductoId) + 1;

            _productos.Add(nuevo);
            AplicarFiltros();
            ActualizarResumen();
        }
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        AbrirEdicion();
    }

    private void DgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ProductoSeleccionado != null)
        {
            AbrirEdicion();
        }
    }

    private void AbrirEdicion()
    {
        Producto? seleccionado = ProductoSeleccionado;

        if (seleccionado == null)
        {
            MessageBox.Show(
                "Seleccione un producto para editarlo.",
                "Producto requerido",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var copia = new Producto
        {
            ProductoId = seleccionado.ProductoId,
            CodigoProducto = seleccionado.CodigoProducto,
            Nombre = seleccionado.Nombre,
            Categoria = seleccionado.Categoria,
            Descripcion = seleccionado.Descripcion,
            Cantidad = seleccionado.Cantidad,
            CantidadMinima = seleccionado.CantidadMinima,
            Unidad = seleccionado.Unidad,
            FechaIngreso = seleccionado.FechaIngreso,
            Estado = seleccionado.Estado
        };

        var ventana = new ProductoFormWindow(copia)
        {
            Owner = this
        };

        if (ventana.ShowDialog() == true && ventana.ProductoResultado != null)
        {
            Producto editado = ventana.ProductoResultado;

            seleccionado.CodigoProducto = editado.CodigoProducto;
            seleccionado.Nombre = editado.Nombre;
            seleccionado.Categoria = editado.Categoria;
            seleccionado.Descripcion = editado.Descripcion;
            seleccionado.Cantidad = editado.Cantidad;
            seleccionado.CantidadMinima = editado.CantidadMinima;
            seleccionado.Unidad = editado.Unidad;
            seleccionado.FechaIngreso = editado.FechaIngreso;
            seleccionado.Estado = editado.Estado;

            _vistaProductos?.Refresh();
            ActualizarResumen();
            ActualizarBotonesSeleccion();
        }
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

        MessageBoxResult confirmacion = MessageBox.Show(
            $"¿Desea cambiar el producto a estado {nuevoEstado}?",
            "Confirmar cambio de estado",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmacion != MessageBoxResult.Yes)
        {
            return;
        }

        producto.Estado = nuevoEstado;
        _vistaProductos?.Refresh();
        ActualizarResumen();
        ActualizarBotonesSeleccion();
    }

    private void DgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ActualizarBotonesSeleccion();
    }

    private void BtnInicio_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "El acceso al panel administrativo general se integrará con el trabajo del equipo.",
            "Integración pendiente",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult resultado = MessageBox.Show(
            "¿Desea cerrar la sesión actual?",
            "Cerrar sesión",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        var login = new LoginWindow();
        login.Show();
        Close();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }
}
