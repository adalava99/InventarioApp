using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioTransacciones.Data;
using ServicioTransacciones.Models;
using ServicioTransacciones.Models.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicioTransacciones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly TransaccionesDbContext dbContext;
        private readonly ILogger<TransaccionesController> logger;
        private readonly HttpClient httpClient;

        public TransaccionesController(TransaccionesDbContext dbContext, ILogger<TransaccionesController> logger, IHttpClientFactory httpClientFactory)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.httpClient = httpClientFactory.CreateClient("ProductosClient");
        }

        [HttpGet]
        [SwaggerOperation("Lista de transacciones", "Retorna lista de transacciones registradas")]
        [SwaggerResponse(200, "Lista de transacciones", typeof(List<Transaccion>))]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetTransacciones()
        {
            try
            {
                var transacciones = await dbContext.Transacciones.ToListAsync();
                return Ok(transacciones);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Obtener transacción por ID", "Devuelve los detalles de una transacción")]
        [SwaggerResponse(200, "Transacción encontrada", typeof(Transaccion))]
        [SwaggerResponse(404, "Transacción no encontrada")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetTransaccionesById(long id)
        {
            try
            {
                var transaccion = await dbContext.Transacciones.FindAsync(id);
                if (transaccion != null)
                {
                    return Ok(transaccion);
                }
                return NotFound(new { mensaje = "Transacción no encontrada" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }

        }

        [HttpPost]
        [SwaggerOperation("Crear nueva transacción", "Registra una nueva transacción en la base de datos")]
        [SwaggerResponse(201, "Transacción creada exitosamente", typeof(Transaccion))]
        [SwaggerResponse(400, "Datos inválidos en la solicitud o stock no disponible")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> CrearTransaccion(TransaccionDto crearTransaccionDto)
        {
            try
            {
                if (crearTransaccionDto.TipoTransaccion == TipoTransaccion.Venta)
                {
                    var stockDisponible = await httpClient.GetFromJsonAsync<StockResponse>(
                        $"/api/productos/{crearTransaccionDto.ProductoId}/verificarStock?cantidad={crearTransaccionDto.Cantidad}");

                    if (stockDisponible == null || !stockDisponible.Disponible)
                    {
                        return BadRequest(new { mensaje = "No hay stock suficiente para esta venta" });
                    }
                }

                var precioUnitario = await httpClient.GetFromJsonAsync<double>(
                    $"/api/productos/{crearTransaccionDto.ProductoId}/obtenerPrecioUnitario");

                var transaccion = new Transaccion
                {
                    Fecha = DateTime.Now,
                    TipoTransaccion = crearTransaccionDto.TipoTransaccion.ToString(),
                    ProductoId = crearTransaccionDto.ProductoId,
                    Cantidad = crearTransaccionDto.Cantidad,
                    PrecioUnitario = precioUnitario,
                    Detalle = crearTransaccionDto.Detalle,
                    PrecioTotal = crearTransaccionDto.Cantidad * precioUnitario
                };

                dbContext.Transacciones.Add(transaccion);
                await dbContext.SaveChangesAsync();

                var ajuste = new AjusteStockDto
                {
                    ProductoId = transaccion.ProductoId,
                    Cantidad = transaccion.TipoTransaccion == TipoTransaccion.Venta.ToString()
                    ? -transaccion.Cantidad: transaccion.Cantidad
                };

                await httpClient.PutAsync(
                    $"/api/productos/ajustarStock", JsonContent.Create(ajuste));


                return CreatedAtAction(nameof(GetTransaccionesById), new { id = transaccion.Id }, transaccion);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }

        }

        [HttpPut("{id}")]
        [SwaggerOperation("Editar transacción", "Edita una transacción existente. No se modifica el precio unitario ni el producto.")]
        [SwaggerResponse(201, "Transacción editada exitosamente", typeof(Transaccion))]
        [SwaggerResponse(400, "Datos inválidos en la solicitud")]
        [SwaggerResponse(404, "Transacción no encontrada")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> EditarTransaccion(long id, EditarTransaccionDto editarTransaccionDto)
        {
            try
            {
                var transaccion = await dbContext.Transacciones.FindAsync(id);

                if (transaccion == null) return NotFound(new { mensaje = "Transacción no encontrada" });

                transaccion.Fecha = editarTransaccionDto.Fecha;
                transaccion.TipoTransaccion = editarTransaccionDto.TipoTransaccion.ToString();
                transaccion.Cantidad = editarTransaccionDto.Cantidad;
                transaccion.PrecioTotal = transaccion.PrecioUnitario * editarTransaccionDto.Cantidad;
                transaccion.Detalle = editarTransaccionDto.Detalle;
                transaccion.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync();

                return Ok(transaccion);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }

        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Eliminar transacción", "Elimina transacción existente por su ID")]
        [SwaggerResponse(200, "Transacción eliminada")]
        [SwaggerResponse(404, "transacción no encontrada")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> EliminarTransaccion(long id)
        {
            try
            {
                var transaccion = await dbContext.Transacciones.FindAsync(id);
                if (transaccion is null) return NotFound(new { mensaje = "Transacción no encontrada" });

                dbContext.Transacciones.Remove(transaccion);
                await dbContext.SaveChangesAsync();
                return Ok(new { mensaje = "Transacción eliminada" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });

            }
        }

        [HttpGet("historial")]
        [SwaggerOperation("Historial de transacciones por cada producto", "Retorna el historial de transacciones de cada producto filtrado por fechas o tipo de transacción")]
        [SwaggerResponse(200, "Consulta exitosa")]
        [SwaggerResponse(400, "Ingreso de datos inválido")]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> ObtenerHistorial([FromQuery] FiltroTransaccionesDto filtro)
        {
            try
            {
                var query = dbContext.Transacciones.AsQueryable();

                if (filtro.FechaInicio.HasValue)
                    query = query.Where(t => t.Fecha >= filtro.FechaInicio);

                if (filtro.FechaFin.HasValue)
                    query = query.Where(t => t.Fecha <= filtro.FechaFin);

                if (filtro.TipoTransaccion.HasValue)
                    query = query.Where(t => t.TipoTransaccion == filtro.TipoTransaccion.ToString());

                var transacciones = await query.ToListAsync();

                var productoIds = transacciones.Select(t => t.ProductoId).Distinct().ToList();

                var response = await httpClient.PostAsJsonAsync($"/api/productos/resumenMasivo", productoIds);
                if (!response.IsSuccessStatusCode)
                    return StatusCode(500, new { mensaje = "Error al obtener datos de productos" });

                var productosResumen = await response.Content.ReadFromJsonAsync<List<ProductoResumenDto>>();

                var mapProductos = productosResumen.ToDictionary(p => p.ProductoId);

                var historial = productoIds.Select(id =>
                {
                    var transaccionesProducto = transacciones
                        .Where(t => t.ProductoId == id)
                        .Select(t => new TransaccionHistorialDto
                        {
                            Fecha = t.Fecha,
                            TipoTransaccion = t.TipoTransaccion,
                            Cantidad = t.Cantidad,
                            PrecioUnitario = t.PrecioUnitario,
                            PrecioTotal = t.PrecioTotal,
                            Detalle = t.Detalle
                        }).ToList();

                    var resumen = mapProductos[id];

                    return new ProductoHistorialDto
                    {
                        ProductoId = id,
                        Nombre = resumen.Nombre,
                        Stock = resumen.Stock,
                        Transacciones = transaccionesProducto
                    };
                }).ToList();

                return Ok(historial);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener historial");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

    }
}
    
