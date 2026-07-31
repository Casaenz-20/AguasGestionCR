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
    }
}
