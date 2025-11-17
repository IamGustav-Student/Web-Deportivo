
using WebDeportivo.Models;
using WebDeportivo.Data;
using WebDeportivo.Interfaces;

namespace WebDeportivo.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        // Inyección del contexto de la base de datos
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public Task<string?> AuthenticateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuario?> ValidateCredentials(string username, string password)
        {
            // 1. Buscar al usuario por nombre de usuario o email
            var user = _context.Usuarios
                .FirstOrDefault(u => u.UsNombre == username || u.UsEmail == username);

            if (user == null)
            {
                return null; // Usuario no encontrado
            }

            // 2. Simulación de verificación de contraseña
            // En un proyecto real, se usaría PasswordHasher.VerifyHashedPassword()
            if (user.UsPasswordHash == "password_hash_ejemplo" && password == "123456")
            {
                return user; // Credenciales válidas
            }

            return null; // Contraseña incorrecta
        }
    }
}