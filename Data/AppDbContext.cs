using Microsoft.EntityFrameworkCore;
using WebDeportivo.Models;

namespace WebDeportivo.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
        }
        public DbSet<Usuario> Usuarios { get; set; }

    }
}
