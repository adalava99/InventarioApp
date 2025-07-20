using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioProductos.Data;
using ServicioProductos.Models;
using ServicioProductos.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicioProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ProductosDbContext dbContext;
        private readonly ILogger<ProductosController> logger;
        public ProductosController(ProductosDbContext dbContext, ILogger<ProductosController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        [HttpGet]
        [SwaggerOperation("Lista de productos", "Retorna lista de productos registrados")]
        [SwaggerResponse(200, "Lista de productos", typeof(List<Producto>))]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetProductos()
        {
            try
            {
                var productos = await dbContext.Productos.ToListAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Obtener producto por ID", "Devuelve los detalles de un producto específico según su ID")]
        [SwaggerResponse(200, "Producto encontrado", typeof(Producto))]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetProductosById(long id)
        {
            try
            {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto != null)
                {
                    return Ok(producto);
                }
                return NotFound(new { mensaje = "Producto no encontrado" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }

        }

        [HttpPost]
        [SwaggerOperation("Crear nuevo producto", "Registra un nuevo producto en la base de datos")]
        [SwaggerResponse(201, "Producto creado exitosamente", typeof(ProductoResponseDto))]
        [SwaggerResponse(400, "Datos inválidos en la solicitud")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> CrearProducto(ProductoDto crearProductoDto)
        {
            try {
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

                var responseDto = new ProductoResponseDto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Categoria = producto.Categoria,
                    Imagen = producto.Imagen,
                    Precio = producto.Precio,
                    Stock = producto.Stock
                };

                return CreatedAtAction(nameof(GetProductosById), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }

        }

        [HttpPut("{id}")]
        [SwaggerOperation("Actualizar producto", "Modifica los datos de un producto existente por su ID")]
        [SwaggerResponse(200, "Producto actualizado correctamente", typeof(Producto))]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(400, "Datos inválidos en la solicitud")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> ActualizarProducto(long id, ProductoDto actualizarProductoDto)
        {
            try {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto == null) return NotFound(new { mensaje = "Producto no encontrado" });

                producto.Nombre = actualizarProductoDto.Nombre;
                producto.Descripcion = actualizarProductoDto.Descripcion;
                producto.Precio = actualizarProductoDto.Precio;
                producto.Categoria = actualizarProductoDto.Categoria;
                producto.Imagen = actualizarProductoDto.Imagen;
                producto.Stock = actualizarProductoDto.Stock;
                producto.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return Ok(producto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }

        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Eliminar producto", "Elimina un producto existente por su ID")]
        [SwaggerResponse(200, "Producto eliminado correctamente")]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> EliminarProducto(long id)
        {
            try {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto is null) return NotFound(new { mensaje = "Producto no encontrado" });

                dbContext.Productos.Remove(producto);
                await dbContext.SaveChangesAsync();
                return Ok(new { mensaje = "Producto eliminado" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }
        }

        [HttpGet("filtrar")]
        [SwaggerOperation("Obtener productos por filtros", "Devuelve una lista de productos filtrados por nombre, categoría, precio o stock, en orden ascendente o descendente")]
        [SwaggerResponse(200, "Lista de productos filtrados", typeof(List<Producto>))]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> FiltrarProductos([FromQuery] ProductoFiltroDto filtro)
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

                var productos = await query.ToListAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}/verificarStock")]
        [SwaggerOperation("Verifica stock para ventas", "Devuelve si hay stock suficiente para realizar una venta")]
        [SwaggerResponse(200, "Verificación exitosa")]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> VerificarStock(long id, [FromQuery] int cantidad)
        {
            try {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto == null)
                    return NotFound(new { mensaje = "Producto no encontrado" });

                if (producto.Stock >= cantidad)
                    return Ok(new { disponible = true });

                return Ok(new { disponible = false });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }

        }

        [HttpGet("{id}/obtenerPrecioUnitario")]
        [SwaggerOperation("Obtener precio", "Obtiene el precio unitario de un producto por su ID")]
        [SwaggerResponse(200, "Consulta exitosa", typeof(double))]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> ObtenerPrecioUnitario(long id)
        {
            try {
                var producto = await dbContext.Productos.FindAsync(id);
                if (producto == null) return NotFound(new { mensaje = "Producto no encontrado" });

                return Ok(producto.Precio);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpPut("ajustarStock")]
        [SwaggerOperation("Ajuste de stock", "Incrementa o reduce el stock de un producto según la cantidad enviada")]
        [SwaggerResponse(200, "Stock actualizado correctamente")]
        [SwaggerResponse(400, "Error de validación")]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> AjustarStock(AjusteStockDto dto)
        {
            try
            {
                var producto = await dbContext.Productos.FindAsync(dto.ProductoId);
                if (producto == null)
                    return NotFound(new { mensaje = "Producto no encontrado" });

                producto.Stock += dto.Cantidad;

                if (producto.Stock < 0)
                    return BadRequest(new { mensaje = "El stock no puede ser negativo" });

                await dbContext.SaveChangesAsync();
                return Ok(new { mensaje = "Stock actualizado correctamente", stockActual = producto.Stock });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpPost("resumenMasivo")]
        [SwaggerOperation("Información de productos", "Devuelve nombre, id y stock de los productos cuyo id coincida con los enviados en el arreglo")]
        [SwaggerResponse(200, "Consulta exitosa")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> ObtenerResumenMasivo([FromBody] List<long> ids)
        {
            try
            {
                var productos = await dbContext.Productos
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    ProductoId = p.Id,
                    Nombre = p.Nombre,
                    Stock = p.Stock
                })
                .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }

        }

    }
}
