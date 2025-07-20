namespace ServicioTransacciones.Models
{
    public class Transaccion
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoTransaccion { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double PrecioTotal { get; set; }
        public string Detalle { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
