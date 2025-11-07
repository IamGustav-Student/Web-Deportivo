// Archivo: Controllers/ConfiguracionController.cs (Carpeta Controllers)

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//  Aquí se aplica el Paso 7: 
[Authorize] // Este atributo protege todo el controlador. Solo usuarios con sesión activa pueden entrar.
public class ConfiguracionController : Controller
{
    // ...

    // Si quieres un nivel de restricción aún mayor en una acción específica:
    [Authorize(Roles = "Administrador")] //  Requiere que, además de estar logueado, su rol sea 'Administrador'.
    public IActionResult GestionUsuarios()
    {
        return View();
    }
}
