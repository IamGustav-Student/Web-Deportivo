using System.ComponentModel.DataAnnotations;

namespace WebDeportivo.ViewModels
{
    // El modelo para la vista de recuperar clave
    public class RecuperarPasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "No parece un email valido")]
        public string Email { get; set; }
    }
}
