// student comment: Usings necesarios para todo
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
// student comment: Traemos nuestras carpetas de buena practica
using WebDeportivo.Data;
using WebDeportivo.Interfaces; // <-- Aca estan las Interfaces
using WebDeportivo.Models;
using WebDeportivo.Models.ViewModels;
using WebDeportivo.ViewModels; // <-- Aca estan los ViewModels

namespace WebDeportivo.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        // student comment: Pedimos la BD y el EmailService
        public AccountController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // --- LOGIN ---
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) 
            {

                // --- CORRECCIÓN PROFESIONAL: Normalizar el email ---
                //  convertimos a minusculas y sacamos espacios
                var emailNormalizado = model.Email.ToLower().Trim();

                // student comment: Buscamos al usuario por su email normalizado
                var usuario = await _context.Usuarios
                                     .Include(u => u.Rol)
                                     .FirstOrDefaultAsync(u => u.UsEmail == emailNormalizado);

                //  Verificamos si existe Y si la clave hasheada coincide
                if (usuario == null)
                {
                    ViewData["Mensaje"] = "Usuario no encontrado";
                    return View("Login");
                }
                if (!BCrypt.Net.BCrypt.Verify(model.Password, usuario.UsPasswordHash))
                {
                    ViewData["Mensaje"] = "Tu cuenta está desactivada";
                    return View("Login");
                }
                if (!usuario.UsActivo)
                {
                    ViewData["Mensaje"] = "Tu Usuario esta inactivo o Baneado";
                    return View(model);
                }

                // Aca guardamos los datos del usuario en la cookie
                var claims = new List<Claim>
            {
                    new Claim(ClaimTypes.Email, usuario.UsEmail),
                new Claim(ClaimTypes.Name, usuario.UsNombre),
                new Claim(ClaimTypes.Surname,usuario.UsApellido),
                new Claim(ClaimTypes.Role, usuario.Rol.RoNombre), 
                new Claim(ClaimTypes.NameIdentifier, usuario.UsId.ToString())
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // student comment: Aca se "loguea" al usuario
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                // ¡Logueo exitoso! Lo mandamos al Home
                return RedirectToAction("Index", "Home");
            }
            return View("Login" , model);
        }

        // --- LOGOUT ---
        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        #region --- SOLICITAR RESTABLECIMIENTO DE CONTRASEÑA (Recuperar por Token) ---
        // GET: /Account/Solicitar
        public IActionResult Solicitar() // student comment: Nombre corto y preciso
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Solicitar(RecoveryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var emailNormalizado = model.Email.ToLower().Trim();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsEmail == emailNormalizado);

            // student comment: Por seguridad, no decimos si el email existe o no
            if (usuario != null)
            {
                // 1. Generar un token único y seguro
                var token = Guid.NewGuid().ToString();

                // 2. Guardar el token y su fecha de expiración en la BD
                usuario.UsPasswordResetToken = token;
                usuario.UsPasswordResetTokenExpires = DateTime.UtcNow.AddHours(2); // Expira en 2 horas
                await _context.SaveChangesAsync();

                // 3. Construir el enlace de restablecimiento
                var callbackUrl = Url.Action("Restablecer", "Account",
                    new { token = token }, Request.Scheme); // student comment: Enlace al nuevo metodo

                // 4. Enviar el email con el enlace
                string cuerpo = $"Hola {usuario.UsNombre},<br/><br/>" +
                                "Para restablecer tu contraseña, haz clic en este enlace: " +
                                $"<a href='{callbackUrl}'>Restablecer Contraseña</a><br/><br/>" +
                                "Este enlace expirará en 2 horas.";
                await _emailService.EnviarEmailAsync(usuario.UsEmail, "Restablecimiento de Contraseña", cuerpo);

                ViewBag.Mensaje = "¡Listo! Si el email está registrado, recibirás un enlace para restablecer tu contraseña.";
            }
            else
            {
                ViewBag.Mensaje = "Si el email está registrado, recibirás un enlace para restablecer tu contraseña.";
            }

            return View();
        }
        #endregion


        // --- REGISTRO ---
        // GET: /Account/Registrar
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // --- CORRECCIÓN PROFESIONAL: Normalizar datos ---
            var emailNormalizado = model.Email.ToLower().Trim();
            var nombreNormalizado = model.Nombre.Trim();

            // 1. Verificamos si el email ya existe
            if (await _context.Usuarios.AnyAsync(u => u.UsEmail == emailNormalizado))
            {
                ModelState.AddModelError("Email", "El email ya está en uso.");
                return View(model);
            }

            // 2. ¡VALIDACION FALTANTE! Verificamos si el Nombre de Usuario ya existe
            if (await _context.Usuarios.AnyAsync(u => u.UsNombre == nombreNormalizado))
            {
                ModelState.AddModelError("Nombre", "Ese nombre de usuario ya existe, elige otro.");
                return View(model);
            }

            // --- Asignación de Rol ---
            var rolAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.RoNombre == "Admin");
            if (rolAdmin == null)
            {
                rolAdmin = new Rol { RoNombre = "Admin" };
                _context.Roles.Add(rolAdmin);
            }

            var rolPendiente = await _context.Roles.FirstOrDefaultAsync(r => r.RoNombre == "Pendiente");
            if (rolPendiente == null)
            {
                rolPendiente = new Rol { RoNombre = "Pendiente" };
                _context.Roles.Add(rolPendiente);
            }

            await _context.SaveChangesAsync();

            // student comment: usamos el email normalizado para la logica
            int rolIdAsignado = (emailNormalizado == "admin@mail.com")
                                ? rolAdmin.RoId
                                : rolPendiente.RoId;
            // --- Fin Asignación de Rol ---

            // 3. Creamos el nuevo usuario
            var usuario = new Usuario
            {
                UsNombre = nombreNormalizado, // Guardamos el nombre normalizado
                UsApellido = model.Apellido,
                UsEmail = emailNormalizado, // Guardamos el email normalizado
                UsPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                UsFechaRegistro = DateTime.Now,
                UsActivo = true,
                RoId = rolIdAsignado
            };

            _context.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // --- RECUPERACION DE CONTRASEÑA ---
        // GET: /Account/Recuperar
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Recuperar(RecoveryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // student comment: Normalizamos el email de recuperacion
            var emailNormalizado = model.Email.ToLower().Trim();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsEmail == emailNormalizado);

            if (usuario != null)
            {
                string nuevaClave = Guid.NewGuid().ToString().Substring(0, 8);
                usuario.UsPasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaClave);
                await _context.SaveChangesAsync();

                string cuerpo = $"Hola {usuario.UsNombre}, tu nueva contraseña temporal es: <b>{nuevaClave}</b>.";
                await _emailService.EnviarEmailAsync(usuario.UsEmail, "Recuperación de cuenta", cuerpo);

                ViewBag.Mensaje = "¡Listo! Revisa tu correo.";
            }
            else
            {
                ViewBag.Mensaje = "Si el correo está registrado, recibirás las instrucciones.";
            }

            return View();
        }
        // --- RESTABLECER CONTRASEÑA (Usando el Token) ---
        // GET: /Account/Restablecer?token=XXXXXX
        public async Task<IActionResult> Restablecer(string token) // student comment: Nombre corto y preciso
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.MensajeError = "El enlace de restablecimiento es inválido.";
                return View("Solicitar"); // Lo enviamos de vuelta a solicitar
            }

            // 1. Buscamos al usuario por el token y verificamos la expiración
            var usuario = await _context.Usuarios
                                 .FirstOrDefaultAsync(u => u.UsPasswordResetToken == token &&
                                                           u.UsPasswordResetTokenExpires > DateTime.UtcNow);

            if (usuario == null)
            {
                ViewBag.MensajeError = "El enlace de restablecimiento es inválido o ha expirado.";
                return View("Solicitar"); // Lo enviamos de vuelta a solicitar
            }

            //  Pre-llenamos el token en el ViewModel para el POST
            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Restablecer(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Normalizar el token (aunque Guid.ToString() ya es consistente)
            var token = model.Token;

            // 2. Buscamos al usuario de nuevo
            var usuario = await _context.Usuarios
                                 .FirstOrDefaultAsync(u => u.UsPasswordResetToken == token &&
                                                           u.UsPasswordResetTokenExpires > DateTime.UtcNow);

            if (usuario == null)
            {
                ViewBag.MensajeError = "El enlace de restablecimiento es inválido o ha expirado. Por favor, solicita uno nuevo.";
                return View("Solicitar");
            }

            // 3. Hashear y guardar la nueva contraseña
            usuario.UsPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NuevaPassword);

            // 4. Limpiar los campos de token (importantísimo para seguridad)
            usuario.UsPasswordResetToken = null;
            usuario.UsPasswordResetTokenExpires = null;

            await _context.SaveChangesAsync();

            ViewBag.MensajeExito = "¡Tu contraseña ha sido restablecida con éxito! Ya puedes iniciar sesión.";
            return View("Login"); // Lo mandamos al login con el mensaje de éxito
        }
    }
}