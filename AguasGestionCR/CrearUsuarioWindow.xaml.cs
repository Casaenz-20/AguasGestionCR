using AguasGestionCR.Models; 
using System;
using System.Linq;
using System.Windows;

namespace AcueductoApp.Views
{
    public partial class CrearUsuarioWindow : Window
    {
        public CrearUsuarioWindow()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtIdentificacion.Text) ||
                string.IsNullOrWhiteSpace(txtMedidor.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtClave.Password))
            {
                MessageBox.Show("Por favor complete todos los campos obligatorios (*).", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtClave.Password != txtConfirmarClave.Password)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AcueductoDbContext())
            {
                bool clienteValido = db.Clientes.Any(c =>
                    c.Identificacion == txtIdentificacion.Text.Trim() &&
                    c.NumeroMedidor == txtMedidor.Text.Trim());

                if (!clienteValido)
                {
                    MessageBox.Show("El número de medidor no está asociado a la identificación del cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool usuarioExiste = db.Usuarios.Any(u => u.NombreUsuario == txtUsuario.Text.Trim());
                if (usuarioExiste)
                {
                    MessageBox.Show("El nombre de usuario ya está registrado.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var nuevoUsuario = new Usuario
                {
                    NombreCompleto = txtNombre.Text.Trim(),
                    Identificacion = txtIdentificacion.Text.Trim(),
                    NumeroMedidor = txtMedidor.Text.Trim(),
                    CorreoElectronico = txtCorreo.Text.Trim(),
                    NombreUsuario = txtUsuario.Text.Trim(),
                    ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(txtClave.Password), 
                    Rol = cmbRol.SelectedItem.ToString(),
                    Estado = true,
                    FechaCreacion = DateTime.Now
                };

                db.Usuarios.Add(nuevoUsuario);
                db.SaveChanges();

                MessageBox.Show("¡Usuario creado con éxito!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();  
            this.Hide();
        }
    }
}