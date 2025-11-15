// Carpeta: Models
// Archivo: Usuario.cs

namespace WebDeportivo.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        // La contraseña NUNCA se almacena en texto plano. Se usa un Hash.
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }

        // Puedes agregar más campos como Nombre, RolId, etc.
    }
}
