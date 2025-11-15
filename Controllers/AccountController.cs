using Microsoft.AspNetCore.Mvc;
using WebDeportivo.Models;
using WebDeportivo.ViewModels;
using WebDeportivo.Interfaces; // <<-- Agregamos la interfaz
using BCrypt.Net; // <<-- Agregamos Bcrypt
using Microsoft.EntityFrameworkCore;
using WebDeportivo.Data;
using WebDeportivo.Models.ViewModels; // <<-- Asegúrate de tener tu DbContext

namespace WebDeportivo.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService; // <<-- Usamos la INTERFAZ

        // Pedimos el contexto Y el servicio de email
        public AccountController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ... (Tu vista de Login GET) ...

        // LOGIN CON BCRYPT
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == model.EMail);

                // Aca comparamos la clave del form con el HASH guardado
                if (usuario != null && BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash))
                {
                    // (Logica de sesion / cookie)
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Usuario o clave incorrectos");
            }
            return View(model);
        }

        // REGISTRO (Para guardar con Hash)
        [HttpPost]
        public async Task<IActionResult> Registrar(Usuario usuario) // Asumo que tienes un ViewModel para registrar
        {
            if (ModelState.IsValid)
            {
                // Aca "hasheamos" la clave antes de guardarla
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(usuario);
        }

        // --- RECUPERACION DE CLAVE ---

        // Muestra la vista para pedir el email
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Recuperar(RecuperarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Si el email esta mal, volvemos
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (usuario != null)
            {
                // 1. Creamos una clave temporal
                string nuevaClave = Guid.NewGuid().ToString().Substring(0, 8);

                // 2. Hasheamos la nueva clave y la guardamos en la BD
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaClave);
                await _context.SaveChangesAsync();

                // 3. Enviamos el email
                string cuerpo = $"Hola {usuario.Username}, tu nueva contraseña temporal es: <b>{nuevaClave}</b>.";
                await _emailService.EnviarEmailAsync(usuario.Email, "Recuperación de cuenta", cuerpo);

                ViewBag.Mensaje = "¡Listo! Revisa tu correo.";
            }
            else
            {
                // Por seguridad, no decimos "el mail no existe"
                ViewBag.Mensaje = "Si el correo está registrado, recibirás las instrucciones.";
            }

            return View();
        }
    }
}
