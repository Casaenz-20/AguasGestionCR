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
using System.Net;
using System.Net.Mail;
using AguasGestionCR.Services;

namespace AguasGestionCR
{
    /// <summary>
    /// Lógica de interacción para ReportView.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        public ReportView()
        {
            InitializeComponent();
        }



        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (UsuarioSesion.Rol == "Cliente")
            {
                txtCorreo.Text = "aguagestioncr@gmail.com";
                txtCorreo.IsReadOnly = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
        string.IsNullOrWhiteSpace(txtMedidor.Text) ||
        cmbTipoAveria.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Extracción limpia de datos
            string tipoAveria = (cmbTipoAveria.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? cmbTipoAveria.SelectedItem.ToString();
            string descripcion = txtDescripcion.Text.Trim();
            string medidor = txtMedidor.Text.Trim();
            string sector = txtSector.Text.Trim();
            string direccion = txtDireccion.Text.Trim();

            // 3. Ejecución por Roles
            if (UsuarioSesion.Rol == "Cliente")
            {
                try
                {
                    string correoContacto = UsuarioSesion.correo;

                    ReportesUser reporte = new ReportesUser();

                    // Fix: Mantenemos el orden exacto de ReportesUser.EnviarReporte(...)
                    reporte.EnviarReporte(descripcion, medidor, tipoAveria, sector, direccion, correoContacto);

                    MessageBox.Show("Reporte enviado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al enviar el reporte. Por favor, inténtelo de nuevo más tarde.\nDetalle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (UsuarioSesion.Rol == "Administrador")
            {
                if (string.IsNullOrWhiteSpace(txtCorreo.Text))
                {
                    MessageBox.Show("Por favor, ingrese el correo de destino.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCorreo.Focus();
                    return;
                }

                try
                {
                    string correo = txtCorreo.Text.Trim();

                    EviarReporte reporte = new EviarReporte();

                    // En EviarReporte.EnviarReporteAveria(...) el correo SÍ va de primero
                    reporte.EnviarReporteAveria(correo, descripcion, medidor, tipoAveria, sector, direccion);

                    MessageBox.Show("Reporte de avería enviado correctamente por el administrador.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al enviar el reporte de administrador.\nDetalle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No se reconoce el rol del usuario actual.", "Error de Autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}
