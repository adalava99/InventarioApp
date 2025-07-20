using System.ComponentModel.DataAnnotations;

namespace ServicioTransacciones.Models.Dtos
{
    public class TransaccionDto
    {

        [Required(ErrorMessage = "El tipo de transacción es obligatorio.")]
        [EnumDataType(typeof(TipoTransaccion), ErrorMessage = "El tipo de transacción debe ser 'Compra' o 'Venta'")]
        public TipoTransaccion TipoTransaccion { get; set; }

        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser mayor a cero")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número entero positivo")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El detalle es obligatorio.")]
        [MaxLength(255, ErrorMessage = "El detalle no puede tener más de 255 caracteres")]
        public string Detalle { get; set; }
    }

    public enum TipoTransaccion
    {
        Compra,
        Venta
    }

}
