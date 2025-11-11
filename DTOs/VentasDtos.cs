namespace Gym_FitByte.DTOs
{
    public class CrearVentaItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } // opcional: si 0, toma Precio del producto
    }
    public class CrearVentaDto
    {
        public string Cliente { get; set; } = "Mostrador";
        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public List<CrearVentaItemDto> Items { get; set; } = new();
    }
}
