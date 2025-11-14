namespace Gym_FitByte.Models
{
    public class Ejercicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Series { get; set; } = string.Empty;
        public string Repeticiones { get; set; } = string.Empty;
        public string Descanso { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;

        public int RutinaId { get; set; }
        public Rutina? Rutina { get; set; }
    }
}
