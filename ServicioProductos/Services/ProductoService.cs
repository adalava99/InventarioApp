using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioProductos.Controllers;
using ServicioProductos.Data;
using ServicioProductos.Models;
using ServicioProductos.Models.Dtos;

namespace ServicioProductos.Services
{
    public class ProductoService
    {
        private readonly ProductosDbContext dbContext;
        private readonly ILogger<ProductoService> logger;
        public ProductoService(ProductosDbContext dbContext, ILogger<ProductoService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<List<Producto>> GetProductos()
        {
            try
            {
                return await dbContext.Productos.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener productos");
                throw;

            }
        }

        public async Task<Producto?> GetProductosById(long id)
        {
            try
            {
                return await dbContext.Productos.FindAsync(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener producto por ID");
                throw;
            }
        }

        public async Task<ProductoResponseDto> CrearProducto(ProductoDto crearProductoDto)
        {
            try
            {
                var producto = new Producto
                {
                    Nombre = crearProductoDto.Nombre,
                    Descripcion = crearProductoDto.Descripcion,
                    Categoria = crearProductoDto.Categoria,
                    Imagen = crearProductoDto.Imagen,
                    Precio = crearProductoDto.Precio,
                    Stock = crearProductoDto.Stock,
                    CreatedAt = DateTime.Now
                };

                dbContext.Productos.Add(producto);
                await dbContext.SaveChangesAsync();

                return new ProductoResponseDto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Categoria = producto.Categoria,
                    Imagen = producto.Imagen,
                    Precio = producto.Precio,
                    Stock = producto.Stock
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear producto");
                throw;
            }
        }

        public async Task<Producto?> ActualizarProducto(long id, ProductoDto actualizarProductoDto)
        {
            try
            {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto == null) return null;

                producto.Nombre = actualizarProductoDto.Nombre;
                producto.Descripcion = actualizarProductoDto.Descripcion;
                producto.Precio = actualizarProductoDto.Precio;
                producto.Categoria = actualizarProductoDto.Categoria;
                producto.Imagen = actualizarProductoDto.Imagen;
                producto.Stock = actualizarProductoDto.Stock;
                producto.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return producto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar producto");
                throw;
            }
        }

        public async Task<bool> EliminarProducto(long id)
        {
            try
            {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto == null) return false;

                dbContext.Productos.Remove(producto);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar producto");
                throw;
            }
        }

        public async Task<List<Producto>> FiltrarProductos(ProductoFiltroDto filtro)
        {
            try
            {
                var query = dbContext.Productos.AsQueryable();

                if (!string.IsNullOrEmpty(filtro.Nombre))
                    query = query.Where(p => p.Nombre.Contains(filtro.Nombre));

                if (!string.IsNullOrEmpty(filtro.Categoria))
                    query = query.Where(p => p.Categoria.Contains(filtro.Categoria));

                if (filtro.OrdenarPor.HasValue)
                {
                    bool asc = filtro.DireccionOrden != DireccionOrden.desc;
                    query = filtro.OrdenarPor switch
                    {
                        CampoOrden.precio => asc ? query.OrderBy(p => p.Precio) : query.OrderByDescending(p => p.Precio),
                        CampoOrden.stock => asc ? query.OrderBy(p => p.Stock) : query.OrderByDescending(p => p.Stock),
                        _ => query
                    };
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar productos");
                throw;
            }
        }

        public async Task<bool?> VerificarStock(long id, int cantidad)
        {
            try
            {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto == null) return null;

                return producto.Stock >= cantidad;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al verificar stock");
                throw;
            }
        }

        public async Task<double?> ObtenerPrecioUnitario(long id)
        {
            try
            {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto == null) return null;

                return producto.Precio;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener precio unitario");
                throw;
            }
        }

        public async Task<(bool exito, int? stockActual)> AjustarStock(AjusteStockDto dto)
        {
            try
            {
                var producto = await dbContext.Productos.FindAsync(dto.ProductoId);
                if (producto == null || producto.Stock + dto.Cantidad < 0)
                    return (false, null);

                producto.Stock += dto.Cantidad;
                await dbContext.SaveChangesAsync();

                return (true, producto.Stock);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al ajustar stock");
                throw;
            }
        }

        public async Task<List<ResumenProductoDto>> ObtenerResumenMasivo(List<long> ids)
        {
            try
            {
                return await dbContext.Productos
                    .Where(p => ids.Contains(p.Id))
                    .Select(p => new ResumenProductoDto
                    {
                        ProductoId = p.Id,
                        Nombre = p.Nombre,
                        Stock = p.Stock
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener resumen masivo");
                throw;
            }
        }

    }
}
