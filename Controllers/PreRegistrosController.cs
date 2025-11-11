using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gym_FitByte.Data;
using Gym_FitByte.Models;

namespace Gym_FitByte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreRegistrosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PreRegistrosController(AppDbContext context)
        {
            _context = context;
        }

        // ========== GET: api/PreRegistros ==========
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var hoy = DateTime.Now;

            // ✅ Traemos los registros y luego hacemos la lógica en memoria
            var preRegistros = await _context.PreRegistros
                .AsNoTracking()
                .ToListAsync();

            // ✅ Evaluamos la fecha de expiración en memoria (ya fuera del IQueryable)
            foreach (var p in preRegistros)
            {
                var fechaExpiracion = p.FechaRegistro.AddDays(3);
                if (p.Estado == EstadoPreRegistro.Pendiente && fechaExpiracion < hoy)
                {
                    p.Estado = EstadoPreRegistro.Vencido;
                }
            }

            return Ok(preRegistros);
        }

        // ========== POST: api/PreRegistros/crear ==========
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] PreRegistro nuevo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            nuevo.FechaRegistro = DateTime.Now;
            nuevo.Estado = EstadoPreRegistro.Pendiente;

            _context.PreRegistros.Add(nuevo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Pre-registro enviado con éxito.", nuevo.Id });
        }

        // ========== PUT: api/PreRegistros/aceptar/{id} ==========
        [HttpPut("aceptar/{id:int}")]
        public async Task<IActionResult> Aceptar(int id)
        {
            var pre = await _context.PreRegistros.FindAsync(id);
            if (pre == null)
                return NotFound("Pre-registro no encontrado.");

            if (pre.Estado != EstadoPreRegistro.Pendiente)
                return BadRequest("El pre-registro ya fue procesado.");

            // Marcar como aceptado
            pre.Estado = EstadoPreRegistro.Aceptado;

            // Crear membresía automáticamente
            var nuevaMembresia = new Membresia
            {
                CodigoCliente = $"CLI{DateTime.Now:yyyyMMddHHmmss}",
                Nombre = pre.Nombre,
                Correo = pre.Correo,
                Telefono = pre.Telefono,
                Direccion = pre.Direccion,
                Edad = pre.Edad,
                Activa = true,
                FechaRegistro = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddMonths(1),
                MontoPagado = 0,
                Tipo = "Inscripción",
                Nivel = "Básica"
            };

            _context.Membresias.Add(nuevaMembresia);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Pre-registro aceptado y membresía creada.", nuevaMembresia.Id });
        }

        // ========== PUT: api/PreRegistros/rechazar/{id} ==========
        [HttpPut("rechazar/{id:int}")]
        public async Task<IActionResult> Rechazar(int id)
        {
            var pre = await _context.PreRegistros.FindAsync(id);
            if (pre == null)
                return NotFound("Pre-registro no encontrado.");

            if (pre.Estado != EstadoPreRegistro.Pendiente)
                return BadRequest("El pre-registro ya fue procesado.");

            pre.Estado = EstadoPreRegistro.Rechazado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Pre-registro rechazado." });
        }
    }
}
