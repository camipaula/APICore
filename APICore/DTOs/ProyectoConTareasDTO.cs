namespace APICore.DTOs
{
    public class ProyectoConTareasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Avance { get; set; }
        public List<CreateTareaDTO> Tareas { get; set; }
    }
}
