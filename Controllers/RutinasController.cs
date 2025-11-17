using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Gym_FitByte.Data;
using Gym_FitByte.DTOs;
using Gym_FitByte.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        // =====================
        // CREAR RUTINA
        // =====================
        [HttpPost("crear")]
        public async Task<IActionResult> CrearRutina([FromForm] CrearRutinaDto dto)
        {
            if (dto.Imagen == null)
                return BadRequest("Debe subir una imagen.");

            // SUBIR IMAGEN A AZURE
            var urlImagen = await SubirImagenABlob(dto.Imagen);

            // DESERIALIZAR JSON DE EJERCICIOS
            List<EjercicioDto>? ejerciciosDto;
            try
            {
                ejerciciosDto = JsonSerializer.Deserialize<List<EjercicioDto>>(dto.EjerciciosJson);
            }
            catch
            {
                return BadRequest("El formato del JSON en 'EjerciciosJson' no es válido.");
            }

            if (ejerciciosDto == null)
                ejerciciosDto = new List<EjercicioDto>();

            // CREAR RUTINA
            var rutina = new Rutina
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Duracion = dto.Duracion,
                Nivel = dto.Nivel,
                Genero = dto.Genero,
                ImagenUrl = urlImagen,
                Ejercicios = ejerciciosDto.Select(e => new Ejercicio
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

            return Ok(rutina);
        }

        // =====================
        // OBTENER POR CATEGORÍA
        // =====================
        [HttpGet("porCategoria")]
        public async Task<IActionResult> ObtenerPorCategoria(string nivel, string genero)
        {
            var data = await _context.Rutinas
                .Include(r => r.Ejercicios)
                .Where(r => r.Nivel == nivel && r.Genero == genero)
                .ToListAsync();

            return Ok(data);
        }

        // =====================
        // OBTENER DETALLE
        // =====================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerRutina(int id)
        {
            var rutina = await _context.Rutinas
                .Include(r => r.Ejercicios)
                .FirstOrDefaultAsync(r => r.Id == id);

            return rutina == null ? NotFound("Rutina no encontrada.") : Ok(rutina);
        }

        // =====================
        // SUBIR IMAGEN A AZURE
        // =====================
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
