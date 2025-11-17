
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Necesario para ForeignKey

namespace WebDeportivo.Models
{
    public class Usuario
    {
        [Key]
        public int UsId { get; set; } // Antes 'Id'

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string UsNombre { get; set; } // Antes 'Nombre'

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string UsApellido { get; set; } // Campo nuevo

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email incorrecto")]
        [StringLength(150)]
        public string UsEmail { get; set; } // Antes 'Email'

        [Required]
        public string UsPasswordHash { get; set; } // Antes 'Password', ahora indica que es un hash

        [Display(Name = "Fecha de Registro")]
        public DateTime UsFechaRegistro { get; set; } // Campo nuevo

        public bool UsActivo { get; set; } // Campo nuevo (para banear o inactivar)

        public string? UsPasswordResetToken { get; set; }
        public DateTime? UsPasswordResetTokenExpires { get; set; }

        [Display(Name = "Rol")]
        public int RoId { get; set; } // La clave foránea

        [ForeignKey("RoId")] // Le decimos a EF Core que esta es la clave
        public virtual Rol Rol { get; set; } // La propiedad de navegación
    }
}

