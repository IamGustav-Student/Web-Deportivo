using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebDeportivo.Models;
using WebDeportivo.Models.ViewModels;

namespace webdeportivo.Controllers
{
    public class HomeController : Controller
    {
        // VISTA INDEX (Página principal con Noticias)
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
    }
}