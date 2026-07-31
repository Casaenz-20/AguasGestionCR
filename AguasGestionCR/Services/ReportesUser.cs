using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace AguasGestionCR.Services
{
    public  class ReportesUser
    {
        private readonly string _correoEmpresa = "aguagestioncr@gmail.com";
        private readonly string _contrasenaApp = "gzzracochvyksoqe";
        private readonly string _hostSmtp = "smtp.gmail.com";
        private readonly int _puertoSmtp = 587;
        public void EnviarReporte(

             string descripcion,
             string medidor,
             string tipoAveria,
             string sector,
             string direccion,
             string correoContacto)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(correoContacto));

            mail.From = new MailAddress(_correoEmpresa);

            mail.ReplyToList.Add(new MailAddress(correoContacto));
            mail.Subject = "Reporte de Avería de Cliente";
            mail.Body = $"Correo dirigido a: {correoContacto}\n\n" +
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
    }
}
