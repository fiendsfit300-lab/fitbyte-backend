using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_FitByte.Models
{
    public enum CompraEstado { Completada = 1, Cancelada = 2 }

    public class Compra
    {
        [Key] public int Id { get; set; }
        [Required] public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public CompraEstado Estado { get; set; } = CompraEstado.Completada;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public ICollection<CompraItem> Items { get; set; } = new List<CompraItem>();
    }

    public class CompraItem
    {
        [Key] public int Id { get; set; }
        [Required] public int CompraId { get; set; }
        public Compra? Compra { get; set; }

        [Required] public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        [Required] public int Cantidad { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal PrecioUnitario { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal Subtotal { get; set; }
    }
}
