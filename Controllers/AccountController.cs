// Carpeta: Controllers
// Archivo: AccountController.cs

using Microsoft.AspNetCore.Mvc;
using WebDeportivo.ViewModels;
using WebDeportivo.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebDeportivo.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Muestra la vista de inicio de sesión
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Validar las credenciales usando el servicio
                var user = await _userService.ValidateCredentials(model.Username!, model.Password!);

                if (user != null)
                {
                    // 2. Si es válido, crear la cookie de autenticación (Claims)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username!),
                        // Puedes añadir más claims, como el rol
                        // new Claim(ClaimTypes.Role, "Administrador") 
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        // Persistir la cookie si el usuario marcó "Recordarme"
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(model.RememberMe ? 60 : 20)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // 3. Redirigir al usuario (ej. a la página de inicio)
                    return RedirectToAction("Actividades", "Home");
                }

                // Si la validación falla
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
            }

            // Si el modelo no es válido (ej. campos vacíos), volver a mostrar el formulario
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
