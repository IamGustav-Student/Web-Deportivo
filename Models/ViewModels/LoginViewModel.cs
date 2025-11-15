// Carpeta: ViewModels
// Archivo: LoginViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace WebDeportivo.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "¡El campo usuario es obligatorio!")] // Validación: No puede ser nulo o vacío
        [Display(Name = "Nombre de Usuario o Email")]           // Etiqueta que se mostrará en el formulario
        public string? Username { get; set; } // Campo para capturar el nombre de usuario

        [Required(ErrorMessage = "¡La contraseña es obligatoria!")]
        [DataType(DataType.Password)] // Indica a Razor que debe usar un input de tipo 'password' (oculta el texto)
        [Display(Name = "Contraseña")]
        public string? Password { get; set; } // Campo para capturar la contraseña

        [EmailAddress(ErrorMessage ="Debes escribir un tipo de Email correcto")]
        public string EMail { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; } // Opción para persistir la cookie

        public string? Usuario { get; internal set; }
        public string? Contrasena { get; internal set; }

        
    }
}
