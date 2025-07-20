using System.ComponentModel.DataAnnotations;

namespace ServicioProductos.Models.Dtos
{
    public class ProductoFiltroDto
    {
        public string? Nombre { get; set; }
        public string? Categoria { get; set; }

        [EnumDataType(typeof(CampoOrden), ErrorMessage = "OrdenarPor debe ser 'precio' o 'stock'")]
        public CampoOrden? OrdenarPor { get; set; }

        [EnumDataType(typeof(DireccionOrden), ErrorMessage = "DireccionOrden debe ser 'asc' o 'desc'")]
        public DireccionOrden? DireccionOrden { get; set; }
    }

    public enum CampoOrden
    {
        precio,
        stock
    }

    public enum DireccionOrden
    {
        asc,
        desc
    }
}
