using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebDeportivo.Models;
using WebDeportivo.Models.ViewModels;
using BCrypt.Net;

namespace webdeportivo.Controllers
{
    public class HomeController : Controller
    {
        // (Página principal con Noticias)
        [Authorize]
        public IActionResult Index()
        {
            var noticias = new List<Noticia>
            {
                new Noticia { Id = 1, Titulo = "¡Gol de Último Minuto!", Resumen = "Agónico remate de cabeza...", ImagenUrl = "/images/noticia-gol.jpg" },
                new Noticia { Id = 2, Titulo = "Tenista Estrella Anuncia su Retiro", Resumen = "La leyenda del tenis cuelga la raqueta...", ImagenUrl = "/images/noticia-tenis.jpg" }
            };
            return View(noticias);
        }

        // ----------------------------------------------
        // VISTAS DE LOGIN
        // ----------------------------------------------

        // GET: Muestra el formulario
        public IActionResult Login()
        {
            return View();
        }

        // POST: Procesa el formulario y redirige
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // CREDENCIALES DE PRUEBA
                const string usuarioValido = "admin";
                const string contrasenaValida = "1234";

                if (model.Usuario == usuarioValido && model.Contrasena == contrasenaValida)
                {
                    // ÉXITO: REDIRECCIÓN A LA PÁGINA SIGUIENTE
                    return RedirectToAction("Actividades", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                }
            }
            return View(model);
        }

        // ----------------------------------------------
        // VISTA DE ACTIVIDADES (PÁGINA SIGUIENTE)
        // ----------------------------------------------
        [Authorize] // Solo Usuarios logueados lo van  a ver
        public IActionResult Actividades()
        {
            // Enviamos la hora actual a la vista
            ViewBag.HoraActual = DateTime.Now.ToString("dd MMMM yyyy - HH:mm:ss");
            return View();
        }

        // ----------------------------------------------
        // OTROS MÉTODOS
        // ----------------------------------------------
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult TestHash()
        {
            return View();
        }

        // POST: /Home/TestHash
        // student comment: Este metodo solo prueba la CREACION de hash
        [HttpPost]
        public IActionResult TestHash(string textToHash)
        {
            if (!string.IsNullOrEmpty(textToHash))
            {
                ViewBag.HashedText = BCrypt.Net.BCrypt.HashPassword(textToHash);
            }
            return View();
        }

        // POST: /Home/TestVerify
        // student comment: Este metodo solo prueba la VERIFICACION
        [HttpPost]
        public IActionResult TestVerify(string plainText, string hashToVerify)
        {
            if (!string.IsNullOrEmpty(plainText) && !string.IsNullOrEmpty(hashToVerify))
            {
                try
                {
                    // student comment: Aca intentamos verificar
                    bool isVerified = BCrypt.Net.BCrypt.Verify(plainText, hashToVerify);
                    ViewBag.VerificationResult = isVerified
                        ? "¡ÉXITO! La contraseña y el hash coinciden."
                        : "¡FALLO! La contraseña y el hash NO coinciden.";
                }
                catch (Exception ex)
                {
                    // student comment: Si el hash es invalido (ej: texto plano), esto da error
                    ViewBag.VerificationResult = $"ERROR: {ex.Message}. (Probablemente el hash no es válido)";
                }
            }
            return View("TestHash"); // Volvemos a la misma vista
        }
    }
}