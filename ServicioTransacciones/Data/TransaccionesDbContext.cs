using Microsoft.EntityFrameworkCore;
using ServicioTransacciones.Models;

namespace ServicioTransacciones.Data
{
    public class TransaccionesDbContext :DbContext
    {
        public TransaccionesDbContext(DbContextOptions<TransaccionesDbContext> options) : base(options)
        {
        }

        public DbSet<Transaccion> Transacciones { get; set; }
    }
}
