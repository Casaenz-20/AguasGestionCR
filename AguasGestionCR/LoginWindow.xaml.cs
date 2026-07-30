using AguasGestionCR;
using AguasGestionCR.Models;
using AguasGestionCR.Services;
using AguasGestionCR.Views;
using System.Windows;

namespace AcueductoApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IUsuarioService _usuarioService;

        public LoginWindow()
        {
            InitializeComponent();
            IPasswordHasher passwordHasher = new BCryptPasswordHasher();
            _usuarioService = new UsuarioService(passwordHasher);
        }

        private void btnIngresar_Click(
     object sender,
     RoutedEventArgs e)
        {
            string usuario =
                txtUsuario.Text.Trim();

            string contrasena =
                txtContrasena.Password;

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show(
                    "Por favor complete todos los campos.",
                    "Campos requeridos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            Usuario? usuarioValido;

            try
            {
                usuarioValido =
                    _usuarioService.Autenticar(
                        usuario,
                        contrasena);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible consultar la base de datos.\n\n" +
                    ex.GetBaseException().Message,
                    "Error de conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (usuarioValido == null)
            {
                MessageBox.Show(
                    "Nombre de usuario o contraseña incorrectos.",
                    "Error de autenticación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                txtContrasena.Clear();
                txtContrasena.Focus();
                return;
            }

            UsuarioSesion.Rol =
                usuarioValido.Rol;

            UsuarioSesion.usuario =
                usuarioValido.NombreCompleto;

            UsuarioSesion.correo =
                usuarioValido.CorreoElectronico
                ?? string.Empty;

            bool esAdministrador =
                string.Equals(
                    usuarioValido.Rol?.Trim(),
                    "Administrador",
                    StringComparison.OrdinalIgnoreCase);

            bool esCliente =
                string.Equals(
                    usuarioValido.Rol?.Trim(),
                    "Cliente",
                    StringComparison.OrdinalIgnoreCase);

            if (!esAdministrador && !esCliente)
            {
                MessageBox.Show(
                    $"El usuario tiene un rol no reconocido: " +
                    $"'{usuarioValido.Rol}'.",
                    "Rol no válido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                esAdministrador
                    ? $"¡Bienvenido Administrador " +
                      $"{usuarioValido.NombreCompleto}!"
                    : $"¡Bienvenido " +
                      $"{usuarioValido.NombreCompleto}!",
                "Acceso concedido",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Window ventanaDestino =
                esAdministrador
                    ? new WindowAdmin()
                    : new MainWindow();

            ventanaDestino.Show();
            Close();
        }

        private void btnCrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            CrearUsuarioWindow crearWindow = new CrearUsuarioWindow();
            crearWindow.ShowDialog();
            this.Close();
        }
         
        private void btnOlvidarContrasena_Click(object sender, RoutedEventArgs e)
        {
            OlvidarContrasenaWindow olvidarWindow = new OlvidarContrasenaWindow();
            olvidarWindow.ShowDialog();
            this.Close();
        }
    }
}