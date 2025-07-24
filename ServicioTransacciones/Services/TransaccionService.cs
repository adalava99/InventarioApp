using Microsoft.EntityFrameworkCore;
using ServicioTransacciones.Controllers;
using ServicioTransacciones.Data;
using ServicioTransacciones.Models;
using ServicioTransacciones.Models.Dtos;

namespace ServicioTransacciones.Services
{
    public class TransaccionService
    {
        private readonly TransaccionesDbContext dbContext;
        private readonly ILogger<TransaccionService> logger;
        private readonly HttpClient httpClient;

        public TransaccionService(TransaccionesDbContext dbContext, ILogger<TransaccionService> logger, IHttpClientFactory httpClientFactory)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.httpClient = httpClientFactory.CreateClient("ProductosClient");
        }

        public async Task<List<Transaccion>> ObtenerTransacciones()
        {
            try
            {
                return await dbContext.Transacciones.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener transacciones");
                throw;
            }
        }

        public async Task<Transaccion?> ObtenerTransaccionPorId(long id)
        {
            try
            {
                return await dbContext.Transacciones.FindAsync(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener transacción por ID");
                throw;
            }
        }

        public async Task<Transaccion?> CrearTransaccion(TransaccionDto crearTransaccionDto)
        {
            try
            {
                if (crearTransaccionDto.TipoTransaccion == TipoTransaccion.Venta)
                {
                    var stockDisponible = await httpClient.GetFromJsonAsync<StockResponse>(
                        $"/api/productos/{crearTransaccionDto.ProductoId}/verificarStock?cantidad={crearTransaccionDto.Cantidad}");

                    if (stockDisponible == null || !stockDisponible.Disponible)
                        return null;
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
                        ? -transaccion.Cantidad
                        : transaccion.Cantidad
                };

                await httpClient.PutAsync(
                    $"/api/productos/ajustarStock", JsonContent.Create(ajuste));

                return transaccion;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear transacción");
                throw;
            }
        }

        public async Task<Transaccion?> EditarTransaccion(long id, EditarTransaccionDto dto)
        {
            try
            {
                var transaccion = await dbContext.Transacciones.FindAsync(id);
                if (transaccion == null)
                    return null;

                transaccion.Fecha = dto.Fecha;
                transaccion.TipoTransaccion = dto.TipoTransaccion.ToString();
                transaccion.Cantidad = dto.Cantidad;
                transaccion.PrecioTotal = transaccion.PrecioUnitario * dto.Cantidad;
                transaccion.Detalle = dto.Detalle;
                transaccion.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return transaccion;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar transacción");
                throw;
            }
        }

        public async Task<bool> EliminarTransaccion(long id)
        {
            try
            {
                var transaccion = await dbContext.Transacciones.FindAsync(id);
                if (transaccion == null)
                    return false;

                dbContext.Transacciones.Remove(transaccion);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar transacción");
                throw;
            }
        }

        public async Task<List<ProductoHistorialDto>> ObtenerHistorial(FiltroTransaccionesDto filtro)
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

                var response = await httpClient.PostAsJsonAsync("/api/productos/resumenMasivo", productoIds);
                if (!response.IsSuccessStatusCode)
                    return null;

                var productosResumen = await response.Content.ReadFromJsonAsync<List<ProductoResumenDto>>();
                if (productosResumen == null)
                    return null;

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

                return historial;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener historial");
                throw;
            }
        }

    }
}
