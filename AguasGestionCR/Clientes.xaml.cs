using AguasGestionCR.Interfaces;
using AguasGestionCR.Models;
using AguasGestionCR.Services;
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
    /// Interaction logic for Clientes.xaml
    /// </summary>
    public partial class Clientes : Window
    {
        private readonly IClientes _clienteServices;
        public Clientes()
        {
            InitializeComponent();
            _clienteServices = new ClienteServices(new AcueductoDbContext());
        }

        private void BtnAdjuntarPDF_Click(object sender, RoutedEventArgs e)
        {
            

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
                     string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                     string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                     string.IsNullOrWhiteSpace(txtArchivoSeleccionado.Text))

                {
                    MessageBox.Show("Por favor, complete todos los campos de texto del formulario.", "Campos Vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al adjuntar el PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
