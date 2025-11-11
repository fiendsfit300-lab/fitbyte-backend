using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gym_FitByte.Data;
using Gym_FitByte.Models;

namespace Gym_FitByte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ComprasController(AppDbContext context) => _context = context;

        // ✅ Crear nueva compra
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] CrearCompraDto dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(dto.ProveedorId);
            if (proveedor == null || !proveedor.Activo)
                return BadRequest("Proveedor inválido o inactivo.");

            if (dto.Items is null || dto.Items.Count == 0)
                return BadRequest("Debe incluir al menos un producto.");

            var ids = dto.Items.Select(i => i.ProductoId).ToList();
            var productos = await _context.Productos.Where(p => ids.Contains(p.Id)).ToListAsync();

            if (productos.Count != ids.Count || productos.Any(p => p.ProveedorId != dto.ProveedorId))
                return BadRequest("Uno o más productos no pertenecen a este proveedor.");

            var compra = new Compra
            {
                ProveedorId = dto.ProveedorId,
                FechaCompra = dto.FechaCompra,
                Estado = dto.Estado,
                Items = new List<CompraItem>()
            };

            foreach (var it in dto.Items)
            {
                var prod = productos.First(p => p.Id == it.ProductoId);
                var subtotal = it.Cantidad * it.PrecioUnitario;

                compra.Items.Add(new CompraItem
                {
                    ProductoId = it.ProductoId,
                    Cantidad = it.Cantidad,
                    PrecioUnitario = it.PrecioUnitario,
                    Subtotal = subtotal
                });

                if (dto.Estado == CompraEstado.Completada)
                    prod.Stock += it.Cantidad;
            }

            compra.Total = compra.Items.Sum(i => i.Subtotal);
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Compra registrada correctamente.",
                compra.Id,
                compra.Total,
                compra.Estado
            });
        }

        // ✅ Cancelar compra (ajusta el stock)
        [HttpPut("cancelar/{id:int}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
                return NotFound("Compra no encontrada.");

            if (compra.Estado == CompraEstado.Cancelada)
                return BadRequest("La compra ya está cancelada.");

            compra.Estado = CompraEstado.Cancelada;

            var idsProd = compra.Items.Select(i => i.ProductoId).ToList();
            var productos = await _context.Productos.Where(p => idsProd.Contains(p.Id)).ToListAsync();

            foreach (var it in compra.Items)
            {
                var prod = productos.First(p => p.Id == it.ProductoId);
                prod.Stock = Math.Max(0, prod.Stock - it.Cantidad);
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Compra cancelada correctamente." });
        }

        // ✅ Listar todas las compras
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Items)
                .OrderByDescending(c => c.FechaCompra)
                .Select(c => new
                {
                    c.Id,
                    c.FechaCompra,
                    c.Estado,
                    c.Total,
                    Proveedor = new
                    {
                        c.Proveedor.Id,
                        c.Proveedor.NombreEmpresa,
                        c.Proveedor.RFC
                    }
                })
                .ToListAsync();

            return Ok(compras);
        }

        // ✅ Detalle de una compra sin bucles
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Detalle(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Items).ThenInclude(i => i.Producto)
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.FechaCompra,
                    c.Estado,
                    c.Total,
                    Proveedor = new
                    {
                        c.Proveedor.Id,
                        c.Proveedor.NombreEmpresa,
                        c.Proveedor.RFC,
                        c.Proveedor.Email
                    },
                    Items = c.Items.Select(i => new
                    {
                        i.ProductoId,
                        i.Producto.Nombre,
                        i.Producto.Categoria,
                        i.Producto.FotoUrl,
                        i.Cantidad,
                        i.PrecioUnitario,
                        i.Subtotal
                    })
                })
                .FirstOrDefaultAsync();

            if (compra == null)
                return NotFound(new { mensaje = "Compra no encontrada." });

            return Ok(compra);
        }
    }

    // ==========================
    // DTOs usados en este módulo
    // ==========================

    public class CrearCompraDto
    {
        public int ProveedorId { get; set; }
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public CompraEstado Estado { get; set; } = CompraEstado.Completada;
        public List<CrearCompraItemDto> Items { get; set; } = new();
    }

    public class CrearCompraItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
