using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace AguasGestionCR.Services
{
    public class Recupera_Contra
    {
        private const string CorreoOrigen = "aguagestioncr@gmail.com";
        private const string ContrasenniaApp = "ylue bwgh ijvo rhce";

        public bool EnviarCodigoRecuperacion(string correoDestino, string codigoRecuperacion)
        {
            MailMessage correo = new MailMessage();

            correo.From = new MailAddress(CorreoOrigen, "Sistema AguasGestionCR");
            correo.To.Add(correoDestino);
            correo.Subject = "Recuperación de contraseña - AguasGestionCR";

            correo.Body = $"Hola,\n\nHas solicitado restablecer tu contraseña en AguasGestionCR.\n" +
                        $"Tu código de recuperación es: {codigoRecuperacion}\n\n" +
                        $"Si no solicitaste esto, puedes ignorar este correo.";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(CorreoOrigen, ContrasenniaApp),
                EnableSsl = true
            };

            try
            {
                smtp.Send(correo);
                return true;
            }
            catch (Exception ex)
            {
                string detalleError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("Error al enviar el correo desde Gmail: " + detalleError, "Error SMTP Gmail", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                correo.Dispose();
                smtp.Dispose();
            }
        }
    }
}