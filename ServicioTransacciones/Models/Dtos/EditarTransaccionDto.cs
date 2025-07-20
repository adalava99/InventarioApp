using System.ComponentModel.DataAnnotations;

namespace ServicioTransacciones.Models.Dtos
{
    public class EditarTransaccionDto
    {
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El tipo de transacción es obligatorio.")]
        [EnumDataType(typeof(TipoTransaccion), ErrorMessage = "El tipo de transacción debe ser 'Compra' o 'Venta'.")]
        public TipoTransaccion TipoTransaccion { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El detalle es obligatorio.")]
        [MaxLength(255, ErrorMessage = "El detalle no puede tener más de 255 caracteres.")]
        public string Detalle { get; set; }
    }
}
