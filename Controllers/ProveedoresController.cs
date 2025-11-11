using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gym_FitByte.Data;
using Gym_FitByte.Models;
using Gym_FitByte.DTOs;

namespace Gym_FitByte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProveedoresController(AppDbContext context) => _context = context;

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] CrearProveedorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreEmpresa) || string.IsNullOrWhiteSpace(dto.RFC))
                return BadRequest("Nombre de la empresa y RFC son obligatorios.");

            if (await _context.Proveedores.AnyAsync(p => p.RFC == dto.RFC))
                return Conflict("Ya existe un proveedor con ese RFC.");

            var p = new Proveedor
            {
                NombreEmpresa = dto.NombreEmpresa,
                PersonaContacto = dto.PersonaContacto,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                RFC = dto.RFC,
                Activo = true
            };
            _context.Proveedores.Add(p);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Proveedor registrado.", p.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProveedorDto dto)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) return NotFound("Proveedor no encontrado.");

            if (!string.Equals(p.RFC, dto.RFC, StringComparison.OrdinalIgnoreCase) &&
                await _context.Proveedores.AnyAsync(x => x.RFC == dto.RFC))
                return Conflict("Ya existe un proveedor con ese RFC.");

            p.NombreEmpresa = dto.NombreEmpresa;
            p.PersonaContacto = dto.PersonaContacto;
            p.Telefono = dto.Telefono;
            p.Email = dto.Email;
            p.Direccion = dto.Direccion;
            p.RFC = dto.RFC;
            p.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Proveedor actualizado." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) return NotFound("Proveedor no encontrado.");
            p.Activo = false;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Proveedor desactivado." });
        }

        [HttpPut("activar/{id:int}")]
        public async Task<IActionResult> Activar(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) return NotFound("Proveedor no encontrado.");
            p.Activo = true;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Proveedor activado." });
        }

        [HttpGet("activos")]
        public async Task<IActionResult> Activos()
        {
            var lista = await _context.Proveedores
                .Where(p => p.Activo)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return Ok(lista);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Detalle(int id)
        {
            var p = await _context.Proveedores
                .Include(x => x.Productos!.Where(pr => pr.Activo))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return NotFound("Proveedor no encontrado.");
            return Ok(p);
        }
    }
}
