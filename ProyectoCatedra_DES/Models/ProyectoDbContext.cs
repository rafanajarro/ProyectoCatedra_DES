using Microsoft.EntityFrameworkCore;

namespace ProyectoCatedra_DES.Models
{
    public class ProyectoDbContext : DbContext
    {
        public ProyectoDbContext(DbContextOptions<ProyectoDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<OrdenCompra> OrdenCompras { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
