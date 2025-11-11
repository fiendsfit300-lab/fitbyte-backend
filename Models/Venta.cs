using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_FitByte.Models
{
    public class Venta
    {
        [Key] public int Id { get; set; }
        [MaxLength(150)] public string Cliente { get; set; } = "Mostrador";
        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        public ICollection<VentaItem> Items { get; set; } = new List<VentaItem>();
    }

    public class VentaItem
    {
        [Key] public int Id { get; set; }
        [Required] public int VentaId { get; set; }
        public Venta? Venta { get; set; }

        [Required] public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

        [Required] public int Cantidad { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal PrecioUnitario { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal Subtotal { get; set; }
    }
}
