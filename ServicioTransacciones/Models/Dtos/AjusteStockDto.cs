using System.ComponentModel.DataAnnotations;

namespace ServicioTransacciones.Models.Dtos
{
    public class AjusteStockDto
    {
        [Required]
        public long ProductoId { get; set; }

        [Required]
        [Range(-int.MaxValue, int.MaxValue, ErrorMessage = "La cantidad debe ser diferente de cero.")]
        public int Cantidad { get; set; }
    }
}
