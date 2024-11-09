namespace APICore.DTOs
{
    public class CreateTareaDTO
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public string UsuarioId { get; set; }  
        public int CategoriaId { get; set; }
        public int StatusId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEstimada { get; set; }  
        public DateTime? FechaReal { get; set; }
    }
}
