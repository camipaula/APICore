using Microsoft.AspNetCore.Identity;
using System.Threading;

namespace APICore.Models
{
    public class Usuario : IdentityUser
    {
       // public int RolId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        // Relación con Rol
       // public Rol Rol { get; set; }

        // Relación con Tarea
        public ICollection<Tarea> Tareas { get; set; }
    }
}
