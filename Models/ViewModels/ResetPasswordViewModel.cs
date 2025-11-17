using System.ComponentModel.DataAnnotations;

namespace WebDeportivo.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NuevaPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NuevaPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public string ConfirmarNuevaPassword { get; set; }

        // student comment: Este campo se llenará automáticamente desde la URL
        public string Token { get; set; }
    }
}
