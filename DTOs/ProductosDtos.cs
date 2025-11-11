namespace Gym_FitByte.DTOs
{
    public class CrearProductoDto
    {
        public int ProveedorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public int StockInicial { get; set; } = 0;
        public IFormFile? Foto { get; set; }
    }
    public class ActualizarProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public IFormFile? Foto { get; set; }
    }
}
