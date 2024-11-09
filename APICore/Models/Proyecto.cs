using System.Threading;

namespace APICore.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Avance { get; set; }

        // Relación con Tarea
        public ICollection<Tarea> Tareas { get; set; }
    }
}