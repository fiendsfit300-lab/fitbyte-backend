// DTOs/ComprasDetalleDtos.cs
using Gym_FitByte.Models;

namespace Gym_FitByte.DTOs
{
    public class CompraDetalleDto
    {
        public int Id { get; set; }
        public ProveedorBasicoDto Proveedor { get; set; } = default!;
        public DateTime FechaCompra { get; set; }
        public CompraEstado Estado { get; set; }
        public decimal Total { get; set; }
        public List<CompraItemDetalleDto> Items { get; set; } = new();
    }

    public class ProveedorBasicoDto
    {
        public int Id { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
    }

    public class CompraItemDetalleDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
