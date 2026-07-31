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
            if(string.IsNullOrWhiteSpace(txtDescripcion.Text) || string.IsNullOrWhiteSpace(txtMedidor.Text) || cmbTipoAveria.SelectedItem == null || UsuarioSesion.Rol == "Administrador")
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (UsuarioSesion.Rol == "Cliente")
            {
                try
                {
                    if (cmbTipoAveria.SelectedItem == null)
                    {
                        MessageBox.Show("Por favor, seleccione un tipo de avería.", "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Detener la ejecución si no ha seleccionado nada
                    }

                    // 2. Extracción segura de valores
                    string correoContacto = UsuarioSesion.correo;
                    string descripcion = txtDescripcion.Text.Trim();
                    string medidor = txtMedidor.Text.Trim();
                    string tipoAveria = ((ComboBoxItem)cmbTipoAveria.SelectedItem).Content.ToString();
                    string sector = txtSector.Text.Trim();
                    string direccion = txtDireccion.Text.Trim();

                    // 3. Enviar el reporte a través de la clase de servicio
                    ReportesUser reporte = new ReportesUser();
                    reporte.EnviarReporte(correoContacto, descripcion, medidor, tipoAveria, sector, direccion);

                    // 4. Confirmación al usuario
                    MessageBox.Show("Reporte enviado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error al enviar el reporte. Por favor, inténtelo de nuevo más tarde." + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if(UsuarioSesion.Rol == "Administrador")
            {
                try
                {
                    EviarReporte reporte = new EviarReporte();
                    string correo = txtCorreo.Text;
                    string descripcion = txtDescripcion.Text;
                    string medidor = txtMedidor.Text;
                    string tipoAveria = ((ComboBoxItem)cmbTipoAveria.SelectedItem).Content.ToString();
                    string sector = txtSector.Text;
                    string direccion = txtDireccion.Text;

                    reporte.EnviarReporteAveria(correo, descripcion, medidor, tipoAveria, sector, direccion);
                }
                catch
                {
                    MessageBox.Show("Error al enviar el reporte. Por favor, inténtelo de nuevo más tarde.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            
        }
    }
}
