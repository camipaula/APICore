namespace APICore.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        // Relación con Tarea
        public ICollection<Tarea> Tareas { get; set; }
    }
}
