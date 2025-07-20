using Microsoft.EntityFrameworkCore;
using ServicioProductos.Models;

namespace ServicioProductos.Data
{
    public class ProductosDbContext: DbContext
    {
        public ProductosDbContext(DbContextOptions<ProductosDbContext> options) : base (options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}
