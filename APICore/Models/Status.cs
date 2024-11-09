namespace APICore.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Relación con Tarea
        public ICollection<Tarea> Tareas { get; set; }
    }
}
