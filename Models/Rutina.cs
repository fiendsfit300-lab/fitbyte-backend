namespace Gym_FitByte.Models
{
    public class Rutina
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = "30 min";
        public string Nivel { get; set; } = "Principiante"; // Principiante, Intermedio, Avanzado
        public string Genero { get; set; } = "Hombre"; // Hombre, Mujer
        public string ImagenUrl { get; set; } = string.Empty;

        public List<Ejercicio> Ejercicios { get; set; } = new();
    }
}
