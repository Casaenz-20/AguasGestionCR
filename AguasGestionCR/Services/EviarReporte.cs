using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AguasGestionCR.Services
{
    public class EviarReporte
    {
        private readonly string _correoEmpresa = "aguagestioncr@gmail.com";
        private readonly string _contrasenaApp = "gzzracochvyksoqe";
        private readonly string _hostSmtp = "smtp.gmail.com";
        private readonly int _puertoSmtp = 587;
        public void EnviarReporteAveria(
             string correoUsuario,
             string descripcion,
             string medidor,
             string tipoAveria,
             string sector,
             string direccion)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(new MailAddress(_correoEmpresa));


            mail.From = new MailAddress(correoUsuario);

            mail.ReplyToList.Add(new MailAddress(correoUsuario));

            mail.Subject = "Reporte de Avería de Cliente";
            mail.Body = $"Correo de contacto del cliente: {correoUsuario}\n\n" +
                        $"Descripción: {descripcion}\n" +
                        $"Número de medidor: {medidor}\n" +
                        $"Tipo de Avería: {tipoAveria}\n" +
                        $"Sector / Comunidad: {sector}\n" +
                        $"Dirección Exacta: {direccion}";

            using (SmtpClient smtp = new SmtpClient(_hostSmtp, _puertoSmtp))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_correoEmpresa, _contrasenaApp);
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }








        }


        public void EnviarCorreoMedidor(string correoDestino, string nombreCliente, string numeroMedidor)
        {
            if (string.IsNullOrWhiteSpace(correoDestino)) return;

            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(correoDestino.Trim()));
            mail.From = new MailAddress(_correoEmpresa); // Usa directamente el correo de la empresa como remitente

            mail.Subject = "Bienvenido a AguasGestionCR - Su número de medidor";
            mail.Body = $"Hola {nombreCliente},\n\n" +
                        $"Su registro en AguasGestionCR se ha completado exitosamente.\n\n" +
                        $"Su número de medidor asignado es: {numeroMedidor}\n\n" +
                        $"Puede utilizar este número para registrarse en nuestra aplicación móvil/web.\n\n" +
                        $"Atentamente,\nGestión de Acueducto";

            using (SmtpClient smtp = new SmtpClient(_hostSmtp, _puertoSmtp))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_correoEmpresa, _contrasenaApp);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }



    }







}

