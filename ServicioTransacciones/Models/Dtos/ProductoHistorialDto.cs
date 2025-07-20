namespace ServicioTransacciones.Models.Dtos
{
    public class ProductoHistorialDto
    {
        public long ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public int Stock { get; set; }
        public List<TransaccionHistorialDto> Transacciones { get; set; } = new();
    }
}
