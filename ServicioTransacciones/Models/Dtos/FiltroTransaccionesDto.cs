namespace ServicioTransacciones.Models.Dtos
{
    public class FiltroTransaccionesDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TipoTransaccion? TipoTransaccion { get; set; }
    }
}
