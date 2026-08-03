using AguasGestionCR.Models;
using AguasGestionCR.Services;
using AguasGestionCR.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AguasGestionCR
{
    /// <summary>
    /// Lógica de interacción para WindowAdmin.xaml
    /// </summary>
    public partial class WindowAdmin : Window
    {
        public WindowAdmin()
        {
            InitializeComponent();

            txtFechaActual.Text = DateTime.Now.ToString("dd/MM/yyyy");

            var context = new AcueductoDbContext();
            var clienteService = new ClienteServices(context);

            // Asignamos el ViewModel al DataContext de toda la ventana
            this.DataContext = new ClientesViewModel(clienteService);
        }

        private void BtnInventario_Click(object sender, RoutedEventArgs e)
        {
            var ventanaInventario =
                   new ProductosWindow
                   {
                       Owner = this
                   };

            ventanaInventario.ShowDialog();
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado =
            MessageBox.Show(
           "¿Desea cerrar la sesión actual?",
           "Cerrar sesión",
           MessageBoxButton.YesNo,
           MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
            {
                return;
            }

            var login =
                new AcueductoApp.Views.LoginWindow();

            login.Show();
            Close();
        }

        private void BtnAgregarCliente_Click(object sender, RoutedEventArgs e)
        {
            Clientes clientes = new Clientes();
            clientes.Show();
        }

        private void BtnCliente_Click(object sender, RoutedEventArgs e)
        {
            ViewInicio.Visibility = Visibility.Collapsed;
            ViewClientes.Visibility = Visibility.Visible;

            BtnClientes.Style = (Style)FindResource("ActiveSidebarButton");
            BtnInicio.Style = (Style)FindResource("SidebarButton");
        }

        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            ViewInicio.Visibility = Visibility.Visible;
            ViewClientes.Visibility = Visibility.Collapsed;

            BtnInicio.Style = (Style)FindResource("ActiveSidebarButton");
            BtnClientes.Style = (Style)FindResource("SidebarButton");
        }

        private void BtnReportarAveria_Click(object sender, RoutedEventArgs e)
        {
            ReportView reportView = new ReportView();
            reportView.Show();
        }

        private void BtnEditarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ClientesViewModel viewModel)
            {
                if (viewModel.ClienteSeleccionado != null)
                {

                    Clientes ventanaClientes = new Clientes(viewModel.ClienteSeleccionado);
                    ventanaClientes.ShowDialog();

                    var context = new AcueductoDbContext();
                    var servicio = new ClienteServices(context);
                    this.DataContext = new ClientesViewModel(servicio);
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un cliente de la tabla para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void BtnEliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ClientesViewModel viewModel)
            {
                if (viewModel.ClienteSeleccionado != null)
                {
                    MessageBoxResult resultado = MessageBox.Show(
                        $"¿Desea dar de baja al cliente: {viewModel.ClienteSeleccionado.NombreCompleto}?",
                        "Confirmar eliminación",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (resultado == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var context = new AcueductoDbContext();
                            var servicio = new ClienteServices(context);

                            servicio.EliminarCliente(viewModel.ClienteSeleccionado.ClienteId);

                            MessageBox.Show("Cliente inhabilitado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Refrescamos el ViewModel para actualizar la tabla
                            this.DataContext = new ClientesViewModel(servicio);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un cliente de la tabla para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnLimpiarFiltros_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReportView reportView = new ReportView();
            reportView.Show();
        }
    }
}
