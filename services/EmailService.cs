using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WebDeportivo.Interfaces; // Importante: Traemos la interfaz

namespace WebDeportivo.Services
{
    // Esta clase "cumple" el contrato de IEmailService
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        // Pedimos la config para leer el appsettings
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmailAsync(string emailDestino, string asunto, string mensaje)
        {
            // Leemos los datos de GMAIL desde appsettings
            var correoOrigen = _config["EmailSettings:Correo"];
            var claveAplicacion = _config["EmailSettings:Clave"]; // Ojo: Es la clave de App de Google

            var clienteSmtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(correoOrigen, claveAplicacion),
                EnableSsl = true, // Gmail usa SSL
            };

            var correo = new MailMessage
            {
                From = new MailAddress(correoOrigen),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true, // Para mandar HTML (como el <b>)
            };

            correo.To.Add(emailDestino);

            // Aca se manda el mail
            await clienteSmtp.SendMailAsync(correo);
        }
    }
}
