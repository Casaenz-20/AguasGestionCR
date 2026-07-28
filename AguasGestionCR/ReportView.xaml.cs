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

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("aguagestioncr@gmail.com");
                mail.To.Add(new MailAddress(txtCorreo.Text.Trim()));
                mail.Subject = "Reporte de Averias";
                mail.Body = $"Descripción: {txtDescripcion.Text}\n" +
                $"Número de medidor: {txtMedidor.Text}\n" +
                $"Tipo de Avería: {((ComboBoxItem)cmbTipoAveria.SelectedItem)?.Content}\n" +
                $"Sector / Comunidad: {txtSector.Text}\n" +
                $"Dirección Exacta: {txtDireccion.Text}";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("aguagestioncr@gmail.com", "gzzracochvyksoqe");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                MessageBox.Show("Reporte enviado correctamente", "Reporte de Averias", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar el reporte: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
           
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
