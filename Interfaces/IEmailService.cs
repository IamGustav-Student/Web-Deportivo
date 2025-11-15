using System.Threading.Tasks;

// Un namespace nuevo para las interfaces
namespace WebDeportivo.Interfaces
{
    // El "contrato" que debe cumplir nuestro servicio de email
    public interface IEmailService
    {
        Task EnviarEmailAsync(string emailDestino, string asunto, string mensaje);
    }
}
