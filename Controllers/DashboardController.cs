using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gym_FitByte.Data;

namespace Gym_FitByte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context) => _context = context;

        [HttpGet("resumen-mensual")]
        public async Task<IActionResult> ResumenMensual([FromQuery] int year)
        {
            if (year <= 0) year = DateTime.Now.Year;

            var compras = await _context.Compras
                .Where(c => c.Estado == Models.CompraEstado.Completada && c.FechaCompra.Year == year)
                .GroupBy(c => c.FechaCompra.Month)
                .Select(g => new { Mes = g.Key, Total = g.Sum(x => x.Total) })
                .ToListAsync();

            var ventas = await _context.Ventas
                .Where(v => v.FechaVenta.Year == year)
                .GroupBy(v => v.FechaVenta.Month)
                .Select(g => new { Mes = g.Key, Total = g.Sum(x => x.Total) })
                .ToListAsync();

            return Ok(new { year, compras, ventas });
        }

        [HttpGet("resumen-diario")]
        public async Task<IActionResult> ResumenDiario([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0) year = DateTime.Now.Year;
            if (month < 1 || month > 12) month = DateTime.Now.Month;

            var compras = await _context.Compras
                .Where(c => c.Estado == Models.CompraEstado.Completada
                            && c.FechaCompra.Year == year && c.FechaCompra.Month == month)
                .GroupBy(c => c.FechaCompra.Day)
                .Select(g => new { Dia = g.Key, Total = g.Sum(x => x.Total) })
                .ToListAsync();

            var ventas = await _context.Ventas
                .Where(v => v.FechaVenta.Year == year && v.FechaVenta.Month == month)
                .GroupBy(v => v.FechaVenta.Day)
                .Select(g => new { Dia = g.Key, Total = g.Sum(x => x.Total) })
                .ToListAsync();

            return Ok(new { year, month, compras, ventas });
        }
    }
}
