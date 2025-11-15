// Carpeta: Services
// Archivo: IUserService.cs


// Carpeta: Services
// Archivo: IUserService.cs


// Carpeta: Services
// Archivo: IUserService.cs


// Carpeta: Services
// Archivo: IUserService.cs

using WebDeportivo.Models;

namespace WebDeportivo.Interfaces
{
    public interface IUserService
    {
        // Método para encontrar un usuario por sus credenciales
        Task<Usuario?> ValidateCredentials(string username, string password);
    }
}
