using Gym_FitByte.Data;
using Gym_FitByte.DTOs;
using Gym_FitByte.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_FitByte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EjerciciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EjerciciosController(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // CREAR EJERCICIO
        // ============================
        [HttpPost("crear")]
        public async Task<IActionResult> CrearEjercicio([FromBody] CrearEjercicioDto dto)
        {
            var rutina = await _context.Rutinas.FindAsync(dto.RutinaId);

            if (rutina == null)
                return BadRequest("La rutina seleccionada no existe.");

            var ejercicio = new Ejercicio
            {
                Nombre = dto.Nombre,
                Series = dto.Series,
                Repeticiones = dto.Repeticiones,
                Descanso = dto.Descanso,
                Notas = dto.Notas,
                RutinaId = dto.RutinaId
            };

            _context.Ejercicios.Add(ejercicio);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ejercicio creado correctamente.",
                ejercicio = new
                {
                    ejercicio.Id,
                    ejercicio.Nombre,
                    ejercicio.Series,
                    ejercicio.Repeticiones,
                    ejercicio.Descanso,
                    ejercicio.Notas,
                    ejercicio.RutinaId
                }
            });
        }


        // ============================
        // LISTAR EJERCICIOS DE UNA RUTINA
        // ============================
        [HttpGet("por-rutina/{rutinaId:int}")]
        public async Task<IActionResult> ObtenerEjerciciosPorRutina(int rutinaId)
        {
            var ejercicios = await _context.Ejercicios
                .Where(e => e.RutinaId == rutinaId)
                .ToListAsync();

            return Ok(ejercicios);
        }

        // ============================
        // DETAILS
        // ============================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerEjercicio(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);

            if (ejercicio == null)
                return NotFound();

            return Ok(ejercicio);
        }

        // ============================
        // DELETE
        // ============================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarEjercicio(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);

            if (ejercicio == null)
                return NotFound();

            _context.Ejercicios.Remove(ejercicio);
            await _context.SaveChangesAsync();

            return Ok("Ejercicio eliminado.");
        }
    }
}
