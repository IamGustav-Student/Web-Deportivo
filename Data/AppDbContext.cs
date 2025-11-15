using Microsoft.EntityFrameworkCore;
using WebDeportivo.Models;

namespace WebDeportivo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Renombramos Usuarios a Usuarios
        public DbSet<Usuario> Usuarios { get; set; }

        // Agregamos la nueva tabla Roles
        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Le decimos cual es la llave primaria de Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.UsId);

            // Le decimos cual es la llave primaria de Rol
            modelBuilder.Entity<Rol>()
                .HasKey(r => r.RoId);

            // Configuramos la relacion: Un Rol tiene muchos Usuarios
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol) // Un usuario tiene un Rol
                .WithMany(r => r.Usuarios) // Un Rol tiene muchos Usuarios
                .HasForeignKey(u => u.RoId); // La clave es RoId
        }
    }
}
