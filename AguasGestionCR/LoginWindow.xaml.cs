using AguasGestionCR;
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
                MessageBox.Show($"¡Bienvenido {usuarioValido.NombreCompleto}!", "Acceso Concedido", MessageBoxButton.OK, MessageBoxImage.Information);

                // Abrir la ventana principal del sistema
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
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