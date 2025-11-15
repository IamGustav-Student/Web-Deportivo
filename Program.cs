// Agrega estos usings arriba de todo
using WebDeportivo.Interfaces;
using WebDeportivo.Services;
using WebDeportivo.Data; // (El using de tu DbContext)
using Microsoft.EntityFrameworkCore; // (El using de EF Core)

var builder = WebApplication.CreateBuilder(args);

// (Aca esta tu builder.Services.AddControllersWithViews();)

// Conexion a la BD (ya deberias tenerla)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Aca "conectamos" la interfaz con la clase
// Cuando alguien pida IEmailService, le damos la clase EmailService
builder.Services.AddTransient<IEmailService, EmailService>();


var app = builder.Build();

// ... (El resto de tu archivo Program.cs) ...