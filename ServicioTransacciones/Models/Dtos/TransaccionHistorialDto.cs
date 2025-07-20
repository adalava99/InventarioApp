namespace ServicioTransacciones.Models.Dtos
{
    public class TransaccionHistorialDto
    {
        public DateTime Fecha { get; set; }
        public string TipoTransaccion { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double PrecioTotal { get; set; }
        public string Detalle { get; set; }
    }
}
