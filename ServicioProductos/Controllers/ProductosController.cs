using Microsoft.AspNetCore.Mvc;
using ServicioProductos.Models;
using ServicioProductos.Models.Dtos;
using ServicioProductos.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicioProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoService productoService;
        public ProductosController(ProductoService productoService)
        {

            this.productoService = productoService;
        }

        [HttpGet]
        [SwaggerOperation("Lista de productos", "Retorna lista de productos registrados")]
        [SwaggerResponse(200, "Lista de productos", typeof(List<Producto>))]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetProductos()
        {
            try
            {
                var productos = await productoService.GetProductos();
                return Ok(productos);
            }
            catch (Exception ex)
            {
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
                var producto = await productoService.GetProductosById(id);
                if (producto != null)
                {
                    return Ok(producto);
                }
                return NotFound(new { mensaje = "Producto no encontrado" });
            }
            catch (Exception ex)
            {
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
            try
            {
                var productoCreado = await productoService.CrearProducto(crearProductoDto);
                return CreatedAtAction(nameof(GetProductosById), new { id = productoCreado.Id }, productoCreado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }

        }

        [HttpPut("{id}")]
        [SwaggerOperation("Actualizar producto", "Modifica los datos de un producto existente por su ID")]
        [SwaggerResponse(200, "Producto actualizado correctamente", typeof(Producto))]
        [SwaggerResponse(404, "Producto no encontrado")]
        [SwaggerResponse(400, "Datos inválidos en la solicitud")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> ActualizarProducto(long id, [FromBody] ProductoDto actualizarProductoDto)
        {
            try
            {
                var productoActualizado = await productoService.ActualizarProducto(id, actualizarProductoDto);
                if (productoActualizado == null)
                    return NotFound(new { mensaje = "Producto no encontrado" });

                return Ok(productoActualizado);
            }
            catch (Exception ex)
            {
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
            try
            {
                var eliminado = await productoService.EliminarProducto(id);
                if (!eliminado)
                    return NotFound(new { mensaje = "Producto no encontrado" });

                return Ok(new { mensaje = "Producto eliminado" });
            }
            catch (Exception ex)
            {
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
                var productos = await productoService.FiltrarProductos(filtro);
                return Ok(productos);
            }
            catch (Exception ex)
            {
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
            try
            {
                var disponible = await productoService.VerificarStock(id, cantidad);
                if (disponible == null)
                    return NotFound(new { mensaje = "Producto no encontrado" });

                return Ok(new { disponible });
            }
            catch (Exception ex)
            {
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
    try
    {
        var precio = await productoService.ObtenerPrecioUnitario(id);
        if (precio == null)
            return NotFound(new { mensaje = "Producto no encontrado" });

        return Ok(precio);
    }
    catch (Exception ex)
    {
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
                var (exito, stockActual) = await productoService.AjustarStock(dto);

                if (!exito)
                    return BadRequest(new { mensaje = "Producto no encontrado" });

                return Ok(new { mensaje = "Stock actualizado correctamente", stockActual });
            }
            catch (Exception ex)
            {
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
                var resumen = await productoService.ObtenerResumenMasivo(ids);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }

        }

    }
}
