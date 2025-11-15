using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebDeportivo.Models
{
    public class Rol
    {
        // Ro para los campos de Rol
        public int RoId { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre del Rol")]
        public string RoNombre { get; set; }

        // Un rol puede tener muchos usuarios
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}