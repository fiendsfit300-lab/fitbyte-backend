namespace Gym_FitByte.DTOs
{


public class CrearRutinaDto
{
    public string Titulo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Duracion { get; set; } = "";
    public string Nivel { get; set; } = "";
    public string Genero { get; set; } = "";
    public IFormFile Imagen { get; set; } = default!;

    // AQUI PEGAS EL JSON COMPLETO DE LOS EJERCICIOS EN SWAGGER
    public string EjerciciosJson { get; set; } = "[]";
}
}