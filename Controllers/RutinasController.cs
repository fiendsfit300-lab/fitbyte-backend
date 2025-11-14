using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Gym_FitByte.Data;
using Gym_FitByte.Models;

namespace Gym_FitByte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RutinasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private const string ContainerName = "rutinas";  

        public RutinasController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

    
        [HttpPost("crear")]
        public async Task<IActionResult> CrearRutina([FromForm] CrearRutinaDto dto)
        {
            if (dto.Imagen == null || dto.Imagen.Length == 0)
                return BadRequest("Debe subir una imagen para la rutina.");

      
            var urlImagen = await SubirImagenABlob(dto.Imagen);

            var rutina = new Rutina
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Duracion = dto.Duracion,
                Genero = dto.Genero,
                Nivel = dto.Nivel,
                ImagenUrl = urlImagen,
                Ejercicios = dto.Ejercicios.Select(e => new Ejercicio
                {
                    Nombre = e.Nombre,
                    Series = e.Series,
                    Repeticiones = e.Repeticiones,
                    Descanso = e.Descanso,
                    Notas = e.Notas
                }).ToList()
            };

            _context.Rutinas.Add(rutina);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Rutina creada correctamente.",
                rutina.Id
            });
        }
 
        [HttpGet("porCategoria")]
        public async Task<IActionResult> ObtenerPorCategoria(string nivel, string genero)
        {
            var data = await _context.Rutinas
                .Include(r => r.Ejercicios)
                .Where(r => r.Nivel == nivel && r.Genero == genero)
                .ToListAsync();

            return Ok(data);
        }

      
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerRutina(int id)
        {
            var rutina = await _context.Rutinas
                .Include(r => r.Ejercicios)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rutina == null)
                return NotFound("Rutina no encontrada.");

            return Ok(rutina);
        }

  
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarRutina(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina == null)
                return NotFound();

            _context.Rutinas.Remove(rutina);
            await _context.SaveChangesAsync();

            return Ok("Rutina eliminada.");
        }

 
        private async Task<string> SubirImagenABlob(IFormFile archivo)
        {
            var connectionString = _config.GetConnectionString("AzureBlobStorage");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

            var nombre = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
            var blobClient = containerClient.GetBlobClient(nombre);

            using var stream = archivo.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}
