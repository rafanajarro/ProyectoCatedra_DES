using System.Net.Mail;
using System.Net;

namespace ProyectoCatedra_DES.Models
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _port = 587;
        private readonly string _senderEmail = "rafanajarro2003@gmail.com";
        private readonly string _senderPassword = "jjjo svoh jwsy hrkj";

        public async Task<bool> EnviarCorreoAsync(string destinatario, string username)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpServer)
                {
                    Port = _port,
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Credenciales Sistema de Inventario y Ordenes de Compra:",
                    Body = "<h1>Nombre de usuario:</h1><br>\r\n" +
                    "<p>Este es un correo automatico que envia el nombre de usuario.</p>\r\n    " +
                    "<p><strong>Nombre de usuario:</strong> " + username + "</p>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(destinatario);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }
    }
}

