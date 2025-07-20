using System.ComponentModel.DataAnnotations;

namespace ServicioProductos.Models.Dtos
{
    public class ProductoDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres.")]
        public string Descripcion { get; set; } = null!;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(255, ErrorMessage = "La categoría no puede exceder 255 caracteres.")]
        public string Categoria { get; set; } = null!;

        [StringLength(255, ErrorMessage = "La URL de la imagen no puede exceder 255 caracteres.")]
        public string? Imagen { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser un número entero positivo.")]
        public int Stock { get; set; }

    }
}
