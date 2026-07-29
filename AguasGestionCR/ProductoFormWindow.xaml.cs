using AguasGestionCR.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AguasGestionCR;

/// <summary>
/// Formulario visual temporal. Valida y devuelve un Producto en memoria.
/// El guardado real en SQL Server se conectará después mediante IProductoService.
/// </summary>
public partial class ProductoFormWindow : Window
{
    private readonly Producto? _productoEdicion;

    public Producto? ProductoResultado { get; private set; }

    public ProductoFormWindow(Producto? producto = null)
    {
        InitializeComponent();
        _productoEdicion = producto;
    }

    private bool EsEdicion => _productoEdicion != null;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CargarListas();

        if (EsEdicion)
        {
            CargarProducto();
        }
        else
        {
            PrepararFormularioNuevo();
        }

        ActualizarVistaStock();
        TxtCodigo.Focus();
    }

    private void CargarListas()
    {
        CmbCategoria.ItemsSource = new List<string>
        {
            "Materiales hidráulicos",
            "Medición y control",
            "Válvulas y control",
            "Consumibles",
            "Químicos y tratamiento",
            "Herramientas",
            "Otros"
        };

        CmbUnidad.ItemsSource = new List<string>
        {
            "Unidades",
            "Metros (m)",
            "Kilogramos (kg)",
            "Litros (L)",
            "Cajas",
            "Paquetes"
        };
    }

    private void PrepararFormularioNuevo()
    {
        TxtTitulo.Text = "Registrar producto";
        TxtSubtitulo.Text =
            "Ingrese la información del producto que formará parte del inventario.";
        BtnGuardar.Content = "Guardar producto";

        TxtCodigo.Clear();
        TxtNombre.Clear();
        CmbCategoria.SelectedIndex = -1;
        CmbUnidad.SelectedIndex = -1;
        TxtCantidad.Text = "0,00";
        TxtCantidadMinima.Text = "20,00";
        DtpFechaIngreso.SelectedDate = DateTime.Today;
        CmbEstado.SelectedIndex = 0;
        TxtDescripcion.Clear();
        OcultarError();
    }

    private void CargarProducto()
    {
        Producto producto = _productoEdicion!;

        TxtTitulo.Text = "Editar producto";
        TxtSubtitulo.Text =
            "Modifique la información necesaria y guarde los cambios.";
        BtnGuardar.Content = "Guardar cambios";

        TxtCodigo.Text = producto.CodigoProducto;
        TxtNombre.Text = producto.Nombre;
        CmbCategoria.Text = producto.Categoria;
        CmbUnidad.Text = producto.Unidad;
        TxtCantidad.Text = producto.Cantidad.ToString("N2");
        TxtCantidadMinima.Text = producto.CantidadMinima.ToString("N2");
        DtpFechaIngreso.SelectedDate =
            producto.FechaIngreso.ToDateTime(TimeOnly.MinValue);
        SeleccionarEstado(producto.Estado);
        TxtDescripcion.Text = producto.Descripcion ?? string.Empty;
        OcultarError();
    }

    private void SeleccionarEstado(string estado)
    {
        foreach (object item in CmbEstado.Items)
        {
            if (item is ComboBoxItem comboItem &&
                comboItem.Content?.ToString() == estado)
            {
                CmbEstado.SelectedItem = comboItem;
                return;
            }
        }

        CmbEstado.SelectedIndex = 0;
    }

    private string ObtenerEstadoSeleccionado()
    {
        return CmbEstado.SelectedItem is ComboBoxItem item
            ? item.Content?.ToString() ?? "Activo"
            : "Activo";
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        OcultarError();

        if (!TryConstruirProducto(out Producto? producto))
        {
            return;
        }

        ProductoResultado = producto;
        DialogResult = true;
    }

    private bool TryConstruirProducto(out Producto? producto)
    {
        producto = null;

        string codigo = TxtCodigo.Text.Trim().ToUpperInvariant();
        string nombre = TxtNombre.Text.Trim();
        string categoria = CmbCategoria.Text.Trim();
        string unidad = CmbUnidad.Text.Trim();
        string estado = ObtenerEstadoSeleccionado();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            MostrarError("El código del producto es obligatorio.");
            TxtCodigo.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarError("El nombre del producto es obligatorio.");
            TxtNombre.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(categoria))
        {
            MostrarError("Debe seleccionar una categoría.");
            CmbCategoria.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(unidad))
        {
            MostrarError("Debe seleccionar una unidad.");
            CmbUnidad.Focus();
            return false;
        }

        if (!TryLeerDecimal(TxtCantidad.Text, out decimal cantidad) || cantidad < 0)
        {
            MostrarError("Ingrese una cantidad válida igual o mayor que cero.");
            TxtCantidad.Focus();
            return false;
        }

        if (!TryLeerDecimal(TxtCantidadMinima.Text, out decimal cantidadMinima) ||
            cantidadMinima < 0)
        {
            MostrarError(
                "Ingrese una cantidad mínima válida igual o mayor que cero.");
            TxtCantidadMinima.Focus();
            return false;
        }

        if (!DtpFechaIngreso.SelectedDate.HasValue)
        {
            MostrarError("Debe seleccionar la fecha de ingreso.");
            DtpFechaIngreso.Focus();
            return false;
        }

        DateTime fecha = DtpFechaIngreso.SelectedDate.Value.Date;

        if (fecha > DateTime.Today)
        {
            MostrarError("La fecha de ingreso no puede ser futura.");
            DtpFechaIngreso.Focus();
            return false;
        }

        producto = new Producto
        {
            ProductoId = _productoEdicion?.ProductoId ?? 0,
            CodigoProducto = codigo,
            Nombre = nombre,
            Categoria = categoria,
            Descripcion = string.IsNullOrWhiteSpace(TxtDescripcion.Text)
                ? null
                : TxtDescripcion.Text.Trim(),
            Cantidad = cantidad,
            CantidadMinima = cantidadMinima,
            Unidad = unidad,
            FechaIngreso = DateOnly.FromDateTime(fecha),
            Estado = estado
        };

        return true;
    }

    private static bool TryLeerDecimal(string texto, out decimal valor)
    {
        return decimal.TryParse(
                   texto,
                   NumberStyles.Number,
                   CultureInfo.CurrentCulture,
                   out valor) ||
               decimal.TryParse(
                   texto.Replace(',', '.'),
                   NumberStyles.Number,
                   CultureInfo.InvariantCulture,
                   out valor);
    }

    private void Cantidad_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ActualizarVistaStock();
    }

    private void ActualizarVistaStock()
    {
        bool cantidadValida =
            TryLeerDecimal(TxtCantidad.Text, out decimal cantidad);
        bool minimoValido =
            TryLeerDecimal(TxtCantidadMinima.Text, out decimal minimo);

        if (!cantidadValida || !minimoValido)
        {
            TxtTituloStock.Text = "Stock pendiente";
            TxtResumenStock.Text =
                "Ingrese la cantidad actual y la cantidad mínima.";
            TxtDetalleStock.Text = string.Empty;
            TxtIconoStock.Text = "?";
            BordeEstadoStock.Background =
                (Brush)FindResource("SkyBrush");
            return;
        }

        bool stockBajo = cantidad <= minimo;

        if (stockBajo)
        {
            TxtTituloStock.Text = "Stock bajo";
            TxtResumenStock.Text =
                "La existencia actual requiere atención o reposición.";
            TxtIconoStock.Text = "!";
            BordeEstadoStock.Background =
                (Brush)FindResource("WarningSoftBrush");
        }
        else
        {
            TxtTituloStock.Text = "Stock disponible";
            TxtResumenStock.Text =
                "La existencia se encuentra por encima del mínimo.";
            TxtIconoStock.Text = "✓";
            BordeEstadoStock.Background =
                (Brush)FindResource("SuccessSoftBrush");
        }

        TxtDetalleStock.Text =
            $"Actual: {cantidad:N2}   ·   Mínimo: {minimo:N2}";
    }

    private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult resultado = MessageBox.Show(
            "¿Desea limpiar los datos del formulario?",
            "Limpiar formulario",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        if (EsEdicion)
        {
            CargarProducto();
        }
        else
        {
            PrepararFormularioNuevo();
        }

        ActualizarVistaStock();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void MostrarError(string mensaje)
    {
        TxtError.Text = mensaje;
        BordeError.Visibility = Visibility.Visible;
    }

    private void OcultarError()
    {
        TxtError.Text = string.Empty;
        BordeError.Visibility = Visibility.Collapsed;
    }
}
