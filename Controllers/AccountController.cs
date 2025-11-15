// ... (asegúrate de tener estos usings)
using Microsoft.AspNetCore.Mvc;
using WebDeportivo.Models;
using WebDeportivo.Models.ViewModels; // Cambiamos el using
using WebDeportivo.Interfaces;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using WebDeportivo.Data;
using System; // Para DateTime
using System.Threading.Tasks; // Para Task

namespace WebDeportivo.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AccountController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ... (Tus metodos Login, Recuperar, etc. se quedan igual por ahora) ...
        // ... (Tu metodo Registrar GET) ...

        // MODIFICAMOS EL REGISTRO
        [HttpPost]
        public async Task<IActionResult> Registrar(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Verificamos si el email ya existe
                if (await _context.Usuarios.AnyAsync(u => u.UsEmail == model.Email))
                {
                    ModelState.AddModelError(string.Empty, "El email ya está en uso.");
                    return View(model);
                }

                // --- Asignación de Rol por Defecto ---
                // Buscamos el rol "Pendiente"
                var rolPorDefecto = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoNombre == "Pendiente");

                // Si no existe (la primera vez que corre), lo creamos rapido
                if (rolPorDefecto == null)
                {
                    rolPorDefecto = new Rol { RoNombre = "Pendiente" };
                    _context.Roles.Add(rolPorDefecto);
                    // Guardamos solo para asegurarnos que el rol exista
                    await _context.SaveChangesAsync();
                }
                // --- Fin Asignación de Rol ---


                // 2. Creamos el nuevo usuario con los campos reformulados
                var usuario = new Usuario
                {
                    UsNombre = model.Nombre,
                    UsApellido = model.Apellido, // El nuevo campo
                    UsEmail = model.Email,
                    UsPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hasheamos
                    UsFechaRegistro = DateTime.Now, // Ponemos la fecha de hoy
                    UsActivo = true, // Lo activamos por defecto
                    RoId = rolPorDefecto.RoId // ¡Asignamos el ID del rol "Pendiente"!
                };

                // 3. Guardamos el usuario nuevo
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // (Opcional: enviar email de bienvenida)
                // await _emailService.EnviarEmailAsync(usuario.UsEmail, "Bienvenido", "Gracias por registrarte");

                return RedirectToAction("Login");
            }

            // Si el modelo no es valido, volvemos a la vista
            return View(model);
        }
    }
}