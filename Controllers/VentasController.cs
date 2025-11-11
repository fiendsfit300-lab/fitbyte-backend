using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gym_FitByte.Data;
using Gym_FitByte.Models;

namespace Gym_FitByte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VentasController(AppDbContext context) => _context = context;

        // ✅ Registrar nueva venta
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] CrearVentaDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Debe incluir al menos un producto.");

            var ids = dto.Items.Select(i => i.ProductoId).ToList();
            var productos = await _context.Productos
                .Where(p => ids.Contains(p.Id) && p.Activo)
                .ToListAsync();

            if (productos.Count != ids.Count)
                return BadRequest("Uno o más productos no existen o están inactivos.");

            var venta = new Venta
            {
                Cliente = dto.Cliente,
                FechaVenta = dto.FechaVenta,
                Total = 0,
                Items = new List<VentaItem>()
            };

            foreach (var it in dto.Items)
            {
                var producto = productos.First(p => p.Id == it.ProductoId);

                if (producto.Stock < it.Cantidad)
                    return BadRequest($"Stock insuficiente para el producto: {producto.Nombre}");

                var subtotal = it.Cantidad * it.PrecioUnitario;
                venta.Items.Add(new VentaItem
                {
                    ProductoId = it.ProductoId,
                    Cantidad = it.Cantidad,
                    PrecioUnitario = it.PrecioUnitario,
                    Subtotal = subtotal
                });

                producto.Stock -= it.Cantidad; // Descontar stock
                venta.Total += subtotal;
            }

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Venta registrada correctamente.",
                venta.Id,
                venta.Total,
                venta.Cliente
            });
        }

        // ✅ Listar ventas (sin ciclos)
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Items)
                .ThenInclude(i => i.Producto)
                .OrderByDescending(v => v.FechaVenta)
                .Select(v => new
                {
                    v.Id,
                    v.Cliente,
                    v.FechaVenta,
                    v.Total,
                    Items = v.Items.Select(i => new
                    {
                        i.ProductoId,
                        i.Producto.Nombre,
                        i.Producto.Categoria,
                        i.Cantidad,
                        i.PrecioUnitario,
                        i.Subtotal
                    })
                })
                .ToListAsync();

            return Ok(ventas);
        }

        // ✅ Detalle de una venta específica
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Detalle(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Items)
                .ThenInclude(i => i.Producto)
                .Where(v => v.Id == id)
                .Select(v => new
                {
                    v.Id,
                    v.Cliente,
                    v.FechaVenta,
                    v.Total,
                    Items = v.Items.Select(i => new
                    {
                        i.ProductoId,
                        i.Producto.Nombre,
                        i.Producto.Categoria,
                        i.Cantidad,
                        i.PrecioUnitario,
                        i.Subtotal
                    })
                })
                .FirstOrDefaultAsync();

            if (venta == null)
                return NotFound(new { mensaje = "Venta no encontrada." });

            return Ok(venta);
        }
    }

    // ===============================
    // DTOs usados en este controlador
    // ===============================

    public class CrearVentaDto
    {
        public string Cliente { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public List<VentaItemDto> Items { get; set; } = new();
    }

    public class VentaItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
