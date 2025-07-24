using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioTransacciones.Data;
using ServicioTransacciones.Models;
using ServicioTransacciones.Models.Dtos;
using ServicioTransacciones.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ServicioTransacciones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly TransaccionService transaccionService;

        public TransaccionesController(TransaccionService transaccionService)
        {
            this.transaccionService = transaccionService;
        }

        [HttpGet]
        [SwaggerOperation("Lista de transacciones", "Retorna lista de transacciones registradas")]
        [SwaggerResponse(200, "Lista de transacciones", typeof(List<Transaccion>))]
        [SwaggerResponse(500, "Error interno del servidor")]
        public async Task<IActionResult> GetTransacciones()
        {
            try
            {
                var transacciones = await transaccionService.ObtenerTransacciones();
                return Ok(transacciones);
            }
            catch (Exception ex)
            {
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
                var transaccion = await transaccionService.ObtenerTransaccionPorId(id);
                if (transaccion != null)
                    return Ok(transaccion);

                return NotFound(new { mensaje = "Transacción no encontrada" });
            }
            catch (Exception ex)
            {
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
                var transaccion = await transaccionService.CrearTransaccion(crearTransaccionDto);
                if (transaccion == null)
                    return BadRequest(new { mensaje = "No hay stock suficiente para esta venta" });

                return CreatedAtAction(nameof(GetTransaccionesById), new { id = transaccion.Id }, transaccion);
            }
            catch (Exception ex)
            {
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
                var transaccion = await transaccionService.EditarTransaccion(id, editarTransaccionDto);
                if (transaccion == null)
                    return NotFound(new { mensaje = "Transacción no encontrada" });

                return Ok(transaccion);
            }
            catch (Exception ex)
            {
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
                var eliminado = await transaccionService.EliminarTransaccion(id);
                if (!eliminado)
                    return NotFound(new { mensaje = "Transacción no encontrada" });

                return Ok(new { mensaje = "Transacción eliminada" });
            }
            catch (Exception ex)
            {
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
                var historial = await transaccionService.ObtenerHistorial(filtro);
                if (historial == null)
                    return StatusCode(500, new { mensaje = "Error al obtener datos de productos" });

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

    }
}
    
