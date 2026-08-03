using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AguasGestionCR.Models;
using AguasGestionCR.Services;

namespace AguasGestionCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            txtFechaActual.Text = $"📅  {DateTime.Now.ToString("dd/MM/yyyy")}";
        }

        private void btnReportarAveria_Click(object sender, RoutedEventArgs e)
        {
            ReportView reportView = new ReportView();
            reportView.Show();
            MessageBox.Show(UsuarioSesion.Rol, UsuarioSesion.correo);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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