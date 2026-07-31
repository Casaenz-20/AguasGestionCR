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
    }
}
