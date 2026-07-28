using System;
using System.Linq;
using System.Windows;
using AguasGestionCR.Models;
using AguasGestionCR.Services; 

namespace AguasGestionCR.Views
{
    public partial class OlvidarContrasenaWindow : Window
    {
        private string codigoGenerado = "";
        private Usuario usuarioEncontrado = null;

        public OlvidarContrasenaWindow()
        {
            InitializeComponent();
        }

        private void btnEnviarCodigo_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreo.Text.Trim();

            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show("Por favor, ingrese su correo electrónico.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AcueductoDbContext())
            {
                usuarioEncontrado = db.Usuarios.FirstOrDefault(u => u.CorreoElectronico == correo);

                if (usuarioEncontrado == null)
                {
                    MessageBox.Show("El correo ingresado no pertenece a ninguna cuenta registrada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            codigoGenerado = new Random().Next(100000, 999999).ToString();

            Recupera_Contra servicioCorreo = new Recupera_Contra();
            bool enviado = servicioCorreo.EnviarCodigoRecuperacion(correo, codigoGenerado);

            if (enviado)
            {
                MessageBox.Show("Se ha enviado un código de verificación a tu correo.", "Correo Enviado", MessageBoxButton.OK, MessageBoxImage.Information);

                panelPaso1.Visibility = Visibility.Collapsed;
                panelPaso2.Visibility = Visibility.Visible;
            }
        }

        private void btnVerificarCodigo_Click(object sender, RoutedEventArgs e)
        {
            string codigoIngresado = txtCodigoIngresado.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoIngresado))
            {
                MessageBox.Show("Por favor, ingrese el código recibido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (codigoIngresado == codigoGenerado)
            {
                MessageBox.Show("¡Código verificado correctamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                panelPaso2.Visibility = Visibility.Collapsed;
                panelPaso3.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("El código ingresado es incorrecto. Verifíquelo e intente de nuevo.", "Código Inválido", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCambiarClave_Click(object sender, RoutedEventArgs e)
        {
            string nuevaClave = txtNuevaClave.Password;
            string confirmarClave = txtConfirmarClave.Password;

            if (string.IsNullOrWhiteSpace(nuevaClave) || string.IsNullOrWhiteSpace(confirmarClave))
            {
                MessageBox.Show("Por favor complete los campos de contraseña.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (nuevaClave != confirmarClave)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AcueductoDbContext())
            {
                var userToUpdate = db.Usuarios.FirstOrDefault(u => u.UsuarioId == usuarioEncontrado.UsuarioId);

                if (userToUpdate != null)
                {
                    userToUpdate.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(nuevaClave);

                    db.SaveChanges();

                    MessageBox.Show("¡Su contraseña ha sido actualizada con éxito! Ya puede iniciar sesión.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}