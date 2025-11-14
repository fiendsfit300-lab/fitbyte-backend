public class CrearRutinaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Duracion { get; set; } = "30 min";
    public string Nivel { get; set; } = "Principiante";
    public string Genero { get; set; } = "Hombre";
    public IFormFile Imagen { get; set; } = default!;
    public List<EjercicioDto> Ejercicios { get; set; } = new();
}

public class EjercicioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public string Repeticiones { get; set; } = string.Empty;
    public string Descanso { get; set; } = string.Empty;
    public string Notas { get; set; } = "";
}
