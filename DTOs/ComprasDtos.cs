using Gym_FitByte.Models;
namespace Gym_FitByte.DTOs
{
    public class CrearCompraItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
    public class CrearCompraDto
    {
        public int ProveedorId { get; set; }
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public CompraEstado Estado { get; set; } = CompraEstado.Completada;
        public List<CrearCompraItemDto> Items { get; set; } = new();
    }
}
