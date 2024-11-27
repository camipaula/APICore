using APICore.Data;
using APICore.DTOs;
using APICore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProyectoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProyectoController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/Proyecto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateProyectoDTO>>> GetProyectos()
        {
            var proyectos = await _context.Proyectos
                .Select(p => new CreateProyectoDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Avance = p.Avance
                })
                .ToListAsync();

            return Ok(proyectos);
        }



        // GET: api/Proyecto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CreateProyectoDTO>> GetProyecto(int id)
        {
            var proyecto = await _context.Proyectos
                                         .Where(p => p.Id == id)
                                         .Select(p => new CreateProyectoDTO
                                         {
                                             Id = p.Id, // Include Id here
                                             Nombre = p.Nombre,
                                             Descripcion = p.Descripcion,
                                             FechaInicio = p.FechaInicio,
                                             FechaFin = p.FechaFin,
                                             Avance = p.Avance
                                         })
                                         .FirstOrDefaultAsync();

            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            return Ok(proyecto);
        }

        [HttpGet("WithTasks")]
        public async Task<ActionResult<IEnumerable<ProyectoConTareasDTO>>> GetProyectosConTareas()
        {
            var proyectos = await _context.Proyectos
                                          .Include(p => p.Tareas)
                                          .ToListAsync();

            // Convertir a DTO para evitar ciclos de referencia
            var proyectoDtos = proyectos.Select(proyecto => new ProyectoConTareasDTO
            {
                Id = proyecto.Id,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                FechaInicio = proyecto.FechaInicio,
                FechaFin = proyecto.FechaFin,
                Avance = proyecto.Avance,
                Tareas = proyecto.Tareas.Select(tarea => new CreateTareaDTO
                {   
                    Id = tarea.Id,
                    ProyectoId = tarea.ProyectoId,
                    UsuarioId = tarea.UsuarioId,
                    CategoriaId = tarea.CategoriaId,
                    StatusId = tarea.StatusId,
                    Nombre = tarea.Nombre,
                    Descripcion = tarea.Descripcion,
                    FechaEstimada = tarea.FechaEstimada,
                    FechaReal = tarea.FechaReal
                }).ToList()
            }).ToList();

            return Ok(proyectoDtos);
        }



        /* // POST: api/Proyecto
         [HttpPost]
         public async Task<ActionResult<Proyecto>> PostProyecto(Proyecto proyecto)
         {
             _context.Proyectos.Add(proyecto);
             await _context.SaveChangesAsync();

             return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.Id }, proyecto);
         }*/
        // POST: api/Proyecto
        [HttpPost]
        public async Task<ActionResult<Proyecto>> PostProyecto(CreateProyectoDTO proyectoDto)
        {
            // Crear una instancia de Proyecto usando los datos del DTO
            var proyecto = new Proyecto
            {
                Nombre = proyectoDto.Nombre,
                Descripcion = proyectoDto.Descripcion,
                FechaInicio = proyectoDto.FechaInicio,
                FechaFin = proyectoDto.FechaFin,
                Avance = proyectoDto.Avance
            };

            _context.Proyectos.Add(proyecto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.Id }, proyecto);
        }


        /*// PUT: api/Proyecto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProyecto(int id, Proyecto proyecto)
        {
            if (id != proyecto.Id)
            {
                return BadRequest("El ID del proyecto no coincide.");
            }

            _context.Entry(proyecto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProyectoExists(id))
                {
                    return NotFound("Proyecto no encontrado.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/
        // PUT: api/Proyecto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProyecto(int id, CreateProyectoDTO proyectoDto)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            // Actualizar los campos del proyecto con los valores del DTO
            proyecto.Nombre = proyectoDto.Nombre;
            proyecto.Descripcion = proyectoDto.Descripcion;
            proyecto.FechaInicio = proyectoDto.FechaInicio;
            proyecto.FechaFin = proyectoDto.FechaFin;
            proyecto.Avance = proyectoDto.Avance;

            _context.Entry(proyecto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProyectoExists(id))
                {
                    return NotFound("Proyecto no encontrado.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }




        // DELETE: api/Proyecto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProyecto(int id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            _context.Proyectos.Remove(proyecto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProyectoExists(int id)
        {
            return _context.Proyectos.Any(e => e.Id == id);
        }





        //METODOS PARA EL CORE
        //PROYECTO AVANCE

        // GET: api/Proyecto/Avance/{proyectoId}
        [HttpGet("Avance/{proyectoId}")]
        public async Task<ActionResult> GetAvanceProyecto(int proyectoId)
        {
            var proyecto = await _context.Proyectos
                .Include(p => p.Tareas)
                .FirstOrDefaultAsync(p => p.Id == proyectoId);

            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            var totalTareas = proyecto.Tareas.Count;
            var tareasCompletadas = proyecto.Tareas.Count(t => t.StatusId == 4); // 4 es el ID de "Completada"

            var avance = totalTareas > 0 ? (double)tareasCompletadas / totalTareas * 100 : 0;

            return Ok(new
            {
                ProyectoId = proyecto.Id,
                Nombre = proyecto.Nombre,
                TotalTareas = totalTareas,
                TareasCompletadas = tareasCompletadas,
                Avance = avance
            });
        }



        //METODO 2 COMPARAR ENTRE DOS PROYECTOS 
        [HttpGet("CompararProyectos")]
        public async Task<ActionResult> CompararProyectos(int proyecto1Id, int proyecto2Id)
        {
            // Obtener información básica de los proyectos
            var proyectos = await _context.Proyectos
                .Where(p => p.Id == proyecto1Id || p.Id == proyecto2Id)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.FechaInicio,
                    p.FechaFin
                }).ToListAsync();

            if (proyectos.Count != 2)
            {
                return NotFound("Uno o ambos proyectos no existen.");
            }

            // Separar los proyectos por ID
            var proyecto1 = proyectos.FirstOrDefault(p => p.Id == proyecto1Id);
            var proyecto2 = proyectos.FirstOrDefault(p => p.Id == proyecto2Id);

            // Obtener tareas de cada proyecto
            var tareasProyecto1 = await _context.Tareas
                .Where(t => t.ProyectoId == proyecto1Id)
                .ToListAsync();

            var tareasProyecto2 = await _context.Tareas
                .Where(t => t.ProyectoId == proyecto2Id)
                .ToListAsync();

            // Obtener el tamaño del equipo de cada proyecto
            var equipoProyecto1 = await _context.Tareas
                .Where(t => t.ProyectoId == proyecto1Id)
                .Select(t => t.UsuarioId)
                .Distinct()
                .CountAsync();

            var equipoProyecto2 = await _context.Tareas
                .Where(t => t.ProyectoId == proyecto2Id)
                .Select(t => t.UsuarioId)
                .Distinct()
                .CountAsync();

            // Calcular métricas para cada proyecto
            var resumenProyecto1 = CalcularResumenProyecto(tareasProyecto1);
            var resumenProyecto2 = CalcularResumenProyecto(tareasProyecto2);

            // Comparar progreso y retrasos
            var comparacion = new
            {
                Proyecto1 = new
                {
                    proyecto1.Nombre,
                    proyecto1.FechaInicio,
                    proyecto1.FechaFin,
                    TotalTareas = resumenProyecto1.TotalTareas,
                    TareasPendientes = resumenProyecto1.TareasPendientes,
                    TareasCompletadas = resumenProyecto1.TareasCompletadas,
                    Progreso = resumenProyecto1.PorcentajeProgreso,
                    Retrasos = resumenProyecto1.Retrasos,
                    TamañoEquipo = equipoProyecto1
                },
                Proyecto2 = new
                {
                    proyecto2.Nombre,
                    proyecto2.FechaInicio,
                    proyecto2.FechaFin,
                    TotalTareas = resumenProyecto2.TotalTareas,
                    TareasPendientes = resumenProyecto2.TareasPendientes,
                    TareasCompletadas = resumenProyecto2.TareasCompletadas,
                    Progreso = resumenProyecto2.PorcentajeProgreso,
                    Retrasos = resumenProyecto2.Retrasos,
                    TamañoEquipo = equipoProyecto2
                },
                Analisis = GenerarAnalisis(
         new
         {
             proyecto1.Nombre,
             resumenProyecto1.TotalTareas,
             resumenProyecto1.TareasCompletadas,
             resumenProyecto1.Retrasos,
             resumenProyecto1.PorcentajeProgreso
         },
         new
         {
             proyecto2.Nombre,
             resumenProyecto2.TotalTareas,
             resumenProyecto2.TareasCompletadas,
             resumenProyecto2.Retrasos,
             resumenProyecto2.PorcentajeProgreso
         },
         equipoProyecto1,
         equipoProyecto2
     )
            };

            return Ok(comparacion);

        }

        // Método para calcular el resumen de un proyecto
        private dynamic CalcularResumenProyecto(List<Tarea> tareas)
        {
            var totalTareas = tareas.Count;
            var tareasCompletadas = tareas.Count(t => t.StatusId == 4); // Tareas con estado "Completado"
            var tareasPendientes = totalTareas - tareasCompletadas;
            var tareasRetrasadas = tareas.Count(t => t.FechaReal.HasValue && t.FechaReal > t.FechaEstimada);
            var porcentajeProgreso = totalTareas > 0 ? (double)tareasCompletadas / totalTareas * 100 : 0;

            return new
            {
                TotalTareas = totalTareas,
                TareasCompletadas = tareasCompletadas,
                TareasPendientes = tareasPendientes,
                Retrasos = tareasRetrasadas,
                PorcentajeProgreso = porcentajeProgreso
            };
        }

        // Método para generar el análisis comparativo
        private string GenerarAnalisis(dynamic proyecto1, dynamic proyecto2, int equipo1, int equipo2)
        {
            var analisis = new List<string>();

            // Diccionario con las métricas a comparar
            var comparaciones = new Dictionary<string, (double valor1, double valor2, string unidad)>
    {
        { "Progreso", (proyecto1.PorcentajeProgreso, proyecto2.PorcentajeProgreso, "%") },
        { "Tareas retrasadas", (proyecto1.Retrasos, proyecto2.Retrasos, "tareas") },
        { "Tamaño del equipo", (equipo1, equipo2, "personas") }
    };

            foreach (var comparacion in comparaciones)
            {
                var (nombre, (valor1, valor2, unidad)) = (comparacion.Key, comparacion.Value);

                if (valor1 > valor2)
                {
                    analisis.Add($"{proyecto1.Nombre} tiene mayor {nombre.ToLower()} ({valor1:0.00} {unidad}) que {proyecto2.Nombre} ({valor2:0.00} {unidad}).");
                }
                else if (valor2 > valor1)
                {
                    analisis.Add($"{proyecto2.Nombre} tiene mayor {nombre.ToLower()} ({valor2:0.00} {unidad}) que {proyecto1.Nombre} ({valor1:0.00} {unidad}).");
                }
                else
                {
                    analisis.Add($"Ambos proyectos tienen el mismo {nombre.ToLower()} ({valor1:0.00} {unidad}).");
                }
            }

            return string.Join(" ", analisis);
        }

    }
}
 



        