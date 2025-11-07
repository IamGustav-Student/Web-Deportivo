using WebDeportivo.Data;
using WebDeportivo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // Necesario para la autenticación
using System;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. Configuración de Servicios (Inyección de Dependencias)
// ==========================================================
builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1.1. Configuración de la Autenticación
// Esto define cómo el sistema verificará y manejará la sesión de usuario.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Ruta a la que se redirige si el usuario no está autenticado
        options.LoginPath = "/Account/Login";
        // Tiempo que dura la cookie (20 minutos por defecto en este ejemplo)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// 1.2. Registro de Servicios Personalizados
// Registra la interfaz y la implementación del servicio de usuario
builder.Services.AddScoped<IUserService, UserService>();
// Registra el contexto de la base de datos (AppDbContext)
builder.Services.AddScoped<AppDbContext>();

// 1.3. Configuración de Entity Framework Core (Si usas una base de datos real)
// Descomenta y ajusta esta sección cuando conectes a una base de datos real (SQL Server, SQLite, etc.)
/*
var connectionString = 
*/

// 1.4. Agrega los servicios de MVC (Controladores y Vistas)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==========================================================
// 2. Configuración del Pipeline HTTP (Middleware)
// ==========================================================

if (!app.Environment.IsDevelopment())
{
    // Manejo de errores en producción
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// Habilita el uso de archivos estáticos (wwwroot: CSS, JS, imágenes)
app.UseStaticFiles();

app.UseRouting();

// Middleware que debe ir entre UseRouting y UseEndpoints:

// Habilita la autenticación (¿quién eres?)
app.UseAuthentication();
// Habilita la autorización (¿qué puedes hacer?)
app.UseAuthorization();

// 3. Definición de Rutas (Endpoints)
// Define el patrón de ruta predeterminado
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
