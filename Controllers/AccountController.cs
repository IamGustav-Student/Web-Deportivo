using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

// student comment: Traemos nuestras carpetas de buena practica
using WebDeportivo.Data;
using WebDeportivo.Models;
using WebDeportivo.ViewModels; // <-- Aca estan los ViewModels
using WebDeportivo.Interfaces; // <-- Aca estan las Interfaces
using WebDeportivo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization; // (Asegúrate de tener este using si RegisterViewModel está aquí)


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

        [Authorize(Roles = "Admin , Pendiente")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            //  Buscamos al usuario E INCLUIMOS su Rol
            var usuario = await _context.Usuarios
                                 .Include(u => u.Rol)
                                 .FirstOrDefaultAsync(u => u.UsEmail == model.Email);

            //Verificamos si existe Y si la clave hasheada coincide
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.UsPasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                return View(model);
            }

            //Aca guardamos los datos del usuario en la cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.UsEmail),
                new Claim("FullName", $"{usuario.UsNombre} {usuario.UsApellido}"),
                new Claim(ClaimTypes.Role, usuario.Rol.RoNombre), // ¡Guardamos el ROL!
                new Claim(ClaimTypes.NameIdentifier, usuario.UsId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Aca se "loguea" al usuario
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Admin");
        }

        // --- LOGOUT ---
        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // student comment: Aca "matamos" la cookie de sesion
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- REGISTRO (CON VALIDACIÓN CORREGIDA) ---
        // GET: /Account/Registrar
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Verificamos si el email ya existe
            if (await _context.Usuarios.AnyAsync(u => u.UsEmail == model.Email))
            {
                // student comment: Le avisamos al campo "Email" que hay un error
                ModelState.AddModelError("Email", "El email ya está en uso.");
                return View(model);
            }

            // 2. ¡NUEVA VALIDACION! Verificamos si el Nombre de Usuario ya existe
            if (await _context.Usuarios.AnyAsync(u => u.UsNombre == model.Nombre))
            {
                // student comment: Le avisamos al campo "Nombre" que hay un error
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

            await _context.SaveChangesAsync(); // Guardamos los roles si son nuevos

            int rolIdAsignado = (model.Email.ToLower() == "admin@mail.com")
                                ? rolAdmin.RoId
                                : rolPendiente.RoId;
            // --- Fin Asignación de Rol ---

            // 3. Creamos el nuevo usuario
            var usuario = new Usuario
            {
                UsNombre = model.Nombre,
                UsApellido = model.Apellido,
                UsEmail = model.Email,
                UsPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), // ¡Hasheamos!
                UsFechaRegistro = DateTime.Now,
                UsActivo = true,
                RoId = rolIdAsignado // ¡Asignamos el rol!
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
        public async Task<IActionResult> Recuperar(RecuperarPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsEmail == model.Email);

            if (usuario != null)
            {
                // 1. Creamos una clave temporal
                string nuevaClave = Guid.NewGuid().ToString().Substring(0, 8);

                // 2. Hasheamos la nueva clave y la guardamos en la BD
                usuario.UsPasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaClave);
                await _context.SaveChangesAsync();

                // 3. Enviamos el email
                string cuerpo = $"Hola {usuario.UsNombre}, tu nueva contraseña temporal es: <b>{nuevaClave}</b>.";
                await _emailService.EnviarEmailAsync(usuario.UsEmail, "Recuperación de cuenta", cuerpo);

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