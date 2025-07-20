namespace ServicioProductos.Models.Dtos
{
    public class ProductoResponseDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public string? Imagen { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
    }
}
