using Microsoft.AspNetCore.Authorization; // ¡Traemos la Autorización!
using Microsoft.AspNetCore.Mvc;

namespace WebDeportivo.Controllers
{

    // aca le decimos que solo los "Admin" pueden entrar
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Ruta: /Admin/Index
        public IActionResult Index()
        {
            
            ViewBag.Mensaje = "Bienvenido al Panel de Control de Administrador.";
            return View();
        }

        // Ruta: /Admin/GestionarUsuarios
        public IActionResult GestionarUsuarios()
        {
            //{esta vista tambien esta protegida
            return View();
        }
    }
}