using AguasGestionCR.Interfaces;
using AguasGestionCR.Models;
using AguasGestionCR.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace AguasGestionCR
{
    public partial class Clientes : Window
    {
        private readonly IClientes _clienteServices;
        private readonly EviarReporte _correoService = new EviarReporte();
        private string rutaArchivoTemporal = null;
        private int? clienteIdEdicion = null;

        //  Constructor por defecto 
        public Clientes()
        {
            InitializeComponent();
            _clienteServices = new ClienteServices(new AcueductoDbContext());
        }

        // Constructor nuevo para cuando se abre desde el botón editar
        public Clientes(Cliente clienteAModificar) : this()
        {
            if (clienteAModificar != null)
            {
                clienteIdEdicion = clienteAModificar.ClienteId;


                txtNombreCompleto.Text = clienteAModificar.NombreCompleto;
                txtIdentificacion.Text = clienteAModificar.Identificacion;
                txtNumeroMedidor.Text = clienteAModificar.NumeroMedidor;
                txtDireccion.Text = clienteAModificar.Direccion;
                txtTelefono.Text = clienteAModificar.Telefono;
                txtCorreo.Text = clienteAModificar.CorreoElectronico;

                // Seleccionamo los etados en los ComboBox según el cliente a modificar
                foreach (ComboBoxItem item in cmbEstado.Items)
                {
                    if (item.Content.ToString() == clienteAModificar.Estado)
                        cmbEstado.SelectedItem = item;
                }

                foreach (ComboBoxItem item in cmbEstadoPrevista.Items)
                {
                    if (item.Content.ToString() == clienteAModificar.EstadoPrevista)
                        cmbEstadoPrevista.SelectedItem = item;
                }

                if (!string.IsNullOrEmpty(clienteAModificar.NombreArchivoCedula))
                {
                    txtArchivoSeleccionado.Text = clienteAModificar.NombreArchivoCedula;
                }
            }
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
                txtArchivoSeleccionado.Text = Path.GetFileName(rutaArchivoTemporal);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            GuardarOActualizarCliente();
        }

        private void GuardarOActualizarCliente()
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

                //  Si es nuevo y está vacío, se genera. Si estamos editando, respetamos el que tiene.
                string medidorFinal = txtNumeroMedidor.Text.Trim();
                if (!clienteIdEdicion.HasValue && string.IsNullOrWhiteSpace(medidorFinal))
                {
                    medidorFinal = $"MED-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}";
                    txtNumeroMedidor.Text = medidorFinal;
                }

                byte[] archivoBytes = null;
                string nombreArchivo = null;

                if (!string.IsNullOrEmpty(rutaArchivoTemporal) && File.Exists(rutaArchivoTemporal))
                {
                    nombreArchivo = Path.GetFileName(rutaArchivoTemporal);
                    archivoBytes = File.ReadAllBytes(rutaArchivoTemporal);
                }

                string estadoSeleccionado = (cmbEstado.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Activo";
                string estadoPrevistaSeleccionado = (cmbEstadoPrevista.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Pendiente";

                // Armamos el objeto Cliente
                Cliente clienteData = new Cliente
                {
                    NombreCompleto = txtNombreCompleto.Text.Trim(),
                    Identificacion = txtIdentificacion.Text.Trim(),
                    NumeroMedidor = medidorFinal,
                    Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    CorreoElectronico = txtCorreo.Text.Trim(),
                    Estado = estadoSeleccionado,
                    EstadoPrevista = estadoPrevistaSeleccionado,
                    DocumentoCedulaPdf = archivoBytes,
                    NombreArchivoCedula = nombreArchivo,
                    UltimaActualizacion = DateTime.Now
                };


                if (clienteIdEdicion.HasValue)
                {

                    clienteData.ClienteId = clienteIdEdicion.Value;
                    bool actualizado = _clienteServices.EditarCliente(clienteData);

                    if (actualizado)
                    {
                        MessageBox.Show("Cliente actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                else
                {

                    clienteData.FechaRegistro = DateTime.Now;
                    int idGenerado = _clienteServices.RegistrarCliente(clienteData);

                    if (idGenerado > 0)
                    {
                        EnviarCorreoMedidor(txtCorreo.Text.Trim(), txtNombreCompleto.Text.Trim(), medidorFinal);

                        MessageBox.Show($"Cliente registrado con éxito.\nMedidor asignado: {medidorFinal}\nSe ha enviado un correo al cliente.",
                                        "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        LimpiarFormulario();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnviarCorreoMedidor(string correoDestino, string nombreCliente, string numeroMedidor)
        {
            _correoService.EnviarCorreoMedidor(correoDestino, nombreCliente, numeroMedidor);
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