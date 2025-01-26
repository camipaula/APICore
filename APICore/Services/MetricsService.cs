namespace APICore.Services
{
    public class MetricsService
    {
        private static MetricsService _instance;
        private static readonly object _lock = new object();

        private MetricsService() { }

        // Propiedad para obtener la única instancia de la clase
        public static MetricsService Instance
        {
            get
            {
                lock (_lock) // Asegura la sincronización en entornos multihilo
                {
                    if (_instance == null)
                    {
                        _instance = new MetricsService();
                    }
                    return _instance;
                }
            }
        }

        // Método para calcular el avance del proyecto
        public double CalcularAvanceProyecto(int totalTareas, int tareasCompletadas)
        {
            return totalTareas > 0 ? (double)tareasCompletadas / totalTareas * 100 : 0;
        }

        // Método para calcular la productividad de un empleado
        public double CalcularProductividadEmpleado(int tareasATiempo, int totalTareas)
        {
            return totalTareas > 0 ? (double)tareasATiempo / totalTareas * 100 : 0;
        }
    }
}
