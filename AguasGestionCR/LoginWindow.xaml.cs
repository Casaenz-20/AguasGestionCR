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

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Password;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor complete todos los campos.", "Campos requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var usuarioValido = _usuarioService.Autenticar(usuario, contrasena);

            if (usuarioValido != null)
            {
                if (txtUsuario.Text.Trim().StartsWith("@admin", StringComparison.OrdinalIgnoreCase))
                {
                    UsuarioSesion.Rol = "Administrador";
                    UsuarioSesion.usuario = usuarioValido.NombreCompleto;
                    UsuarioSesion.correo = usuarioValido.CorreoElectronico;

                    MessageBox.Show($"¡Bienvenido Administrador {usuarioValido.NombreCompleto}!", "Acceso Concedido", MessageBoxButton.OK, MessageBoxImage.Information);

                    WindowAdmin menu_admin = new WindowAdmin();
                    menu_admin.Show();
                    this.Close();
                }
                else
                {
                    UsuarioSesion.Rol = "Usuario";
                    UsuarioSesion.usuario = usuarioValido.NombreCompleto;
                    UsuarioSesion.correo = usuarioValido.CorreoElectronico;

                    MessageBox.Show($"¡Bienvenido {usuarioValido.NombreCompleto}!", "Acceso Concedido", MessageBoxButton.OK, MessageBoxImage.Information);

                    MainWindow menu_cliente = new MainWindow();
                    menu_cliente.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Nombre de usuario o contraseña incorrectos.", "Error de Autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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