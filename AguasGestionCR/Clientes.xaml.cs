using AguasGestionCR.Interfaces;
using AguasGestionCR.Models;
using AguasGestionCR.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace AguasGestionCR
{
    /// <summary>
    /// Interaction logic for Clientes.xaml
    /// </summary>
    public partial class Clientes : Window
    {
        private readonly IClientes _clienteServices;
        private string rutaArchivoTemporal = null;
        public Clientes()
        {
            InitializeComponent();
            _clienteServices = new ClienteServices(new AcueductoDbContext());
        }

        private void BtnAdjuntarPDF_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf",
                Title = "Seleccionar Cédula en PDF"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                rutaArchivoTemporal = openFileDialog.FileName;
                // Muestra el nombre del archivo en el TextBlock que definiste en el XAML
                txtArchivoSeleccionado.Text = Path.GetFileName(rutaArchivoTemporal);
            }

        }


        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            RegistrarCliente();
        }

        private void RegistrarCliente()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombreCompleto.Text) ||
                    string.IsNullOrWhiteSpace(txtIdentificacion.Text) ||
                    string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                    string.IsNullOrWhiteSpace(txtCorreo.Text))
                {
                    MessageBox.Show("Por favor, complete los campos obligatorios principales.", "Campos Vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Generamos el medidor de antemano para ponerlo en el TextBox y enviarlo
                string medidorGenerado = $"MED-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}";

                // Lo mostramos de una vez en el TextBox de tu diseño
                txtNumeroMedidor.Text = medidorGenerado;

                byte[] archivoBytes = null;
                string nombreArchivo = null;

                if (!string.IsNullOrEmpty(rutaArchivoTemporal) && File.Exists(rutaArchivoTemporal))
                {
                    nombreArchivo = Path.GetFileName(rutaArchivoTemporal);
                    archivoBytes = File.ReadAllBytes(rutaArchivoTemporal);
                }

                string estadoSeleccionado = (cmbEstado.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Activo";
                string estadoPrevistaSeleccionado = (cmbEstadoPrevista.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Pendiente";

                Cliente nuevoCliente = new Cliente
                {
                    NombreCompleto = txtNombreCompleto.Text.Trim(),
                    Identificacion = txtIdentificacion.Text.Trim(),
                    NumeroMedidor = medidorGenerado, // Asignamos el medidor generado
                    Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    CorreoElectronico = txtCorreo.Text.Trim(),
                    Estado = estadoSeleccionado,
                    EstadoPrevista = estadoPrevistaSeleccionado,
                    DocumentoCedulaPdf = archivoBytes,
                    NombreArchivoCedula = nombreArchivo,
                    FechaRegistro = DateTime.Now,
                    UltimaActualizacion = DateTime.Now
                };

                int idGenerado = _clienteServices.RegistrarCliente(nuevoCliente);

                if (idGenerado > 0)
                {
                    // ENVIAR CORREO ELECTRÓNICO CON EL NÚMERO DE MEDIDOR
                    EnviarCorreoMedidor(txtCorreo.Text.Trim(), txtNombreCompleto.Text.Trim(), medidorGenerado);

                    MessageBox.Show($"Cliente registrado con éxito.\nMedidor asignado: {medidorGenerado}\nSe ha enviado un correo al cliente.",
                                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormulario();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al registrar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnviarCorreoMedidor(string correoDestino, string nombreCliente, string numeroMedidor)
        {
            try
            {
                System.Net.Mail.MailMessage mensaje = new System.Net.Mail.MailMessage();
                mensaje.To.Add(correoDestino);
                mensaje.Subject = "Bienvenido a AguasGestionCR - Su número de medidor";
                mensaje.Body = $"Hola {nombreCliente},\n\nSu registro en AguasGestionCR se ha completado exitosamente.\n\n" +
                               $"Su número de medidor asignado es: {numeroMedidor}\n\n" +
                               $"Puede utilizar este número para registrarse en nuestra aplicación móvil/web.\n\nAtentamente,\nGestión de Acueducto";
                mensaje.From = new System.Net.Mail.MailAddress("tu_correo@gmail.com"); // Cambia por tu correo remitente

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new System.Net.NetworkCredential("tu_correo@gmail.com", "tu_contraseña_de_aplicacion");
                smtp.EnableSsl = true;

                //  Ejecutar en segundo plano para que la interfaz no se congele al enviar el correo
                smtp.Send(mensaje);
            }
            catch (Exception ex)
            {
                // Si falla el correo por falta de internet, puedes avisar pero el cliente ya quedó guardado
                MessageBox.Show($"El cliente se guardó, pero hubo un error al enviar el correo: {ex.Message}", "Aviso de Correo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }
        private void LimpiarFormulario()
        {
            txtNombreCompleto.Clear();
            txtIdentificacion.Clear();
            txtNumeroMedidor.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            cmbEstado.SelectedIndex = 0;
            cmbEstadoPrevista.SelectedIndex = 0;
            rutaArchivoTemporal = null;
            txtArchivoSeleccionado.Text = "Ningún archivo seleccionado";
        }
    }
}
