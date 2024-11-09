namespace APICore.Models
{
    public class Tarea
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

        // Relaciones
        public Proyecto Proyecto { get; set; }
        public Usuario Usuario { get; set; }
        public Categoria Categoria { get; set; }
        public Status Status { get; set; }
    }
}
