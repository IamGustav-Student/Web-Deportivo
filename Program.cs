// student comment: Traemos las carpetas que creamos
using WebDeportivo.Data;
using WebDeportivo.Interfaces;
using WebDeportivo.Services;
using Microsoft.EntityFrameworkCore;
// student comment: Traemos el servicio de autenticacion (Cookies)
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- Zona de Servicios ---
// student comment: Aca "registramos" todo lo que la app va a usar

// 1. Servicio de Controladores y Vistas (El nucleo de MVC)
builder.Services.AddControllersWithViews();

// 2. Servicio de Base de Datos (DbContext)
// student comment: Le decimos a la app que use SQL Server y lea la clave del appsettings
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Servicio de Email (Inyeccion de Dependencias)
// student comment: Cuando un controlador pida "IEmailService", le damos "EmailService"
builder.Services.AddTransient<IEmailService, EmailService>();

// 4. Servicio de Autenticacion (Cookies)
// student comment: Aca configuramos la cookie de login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // student comment: Si no esta logueado, lo mandamos aca
        options.AccessDeniedPath = "/Home/AccessDenied"; // student comment: Si no tiene el rol, lo mandamos aca
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // student comment: La sesion dura 1 hora
    });
// --- Fin Zona de Servicios ---


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- Habilitar Autenticacion y Autorizacion ---
// student comment: Ojo, van en este orden
app.UseAuthentication(); // 1. Quien eres?
app.UseAuthorization();  // 2. Que puedes hacer?
// --- Fin ---

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();