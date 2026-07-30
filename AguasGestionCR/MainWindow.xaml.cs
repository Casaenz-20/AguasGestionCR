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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReportarAveria_Click(object sender, RoutedEventArgs e)
        {
            ReportView reportView = new ReportView();
            reportView.Show();
        }
    }
}