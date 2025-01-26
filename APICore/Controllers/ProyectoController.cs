using APICore.DTOs;
using APICore.Models;
using APICore.Repositories;
using APICore.Services;
using Microsoft.AspNetCore.Mvc;

namespace APICore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectoController : ControllerBase
    {
        private readonly IRepository<Proyecto> _proyectoRepository;

        public ProyectoController(IRepository<Proyecto> proyectoRepository)
        {
            _proyectoRepository = proyectoRepository;
        }

        // GET: api/Proyecto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateProyectoDTO>>> GetProyectos()
        {
            var proyectos = await _proyectoRepository.GetAllAsync();
            var proyectoDtos = proyectos.Select(p => new CreateProyectoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Avance = p.Avance
            });

            return Ok(proyectoDtos);
        }

        // GET: api/Proyecto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CreateProyectoDTO>> GetProyecto(int id)
        {
            var proyecto = await _proyectoRepository.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            var proyectoDto = new CreateProyectoDTO
            {
                Id = proyecto.Id,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                FechaInicio = proyecto.FechaInicio,
                FechaFin = proyecto.FechaFin,
                Avance = proyecto.Avance
            };

            return Ok(proyectoDto);
        }

        [HttpGet("WithTasks")]
        public async Task<ActionResult<IEnumerable<ProyectoConTareasDTO>>> GetProyectosConTareas()
        {
            var proyectos = await _proyectoRepository.GetAllAsync();
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

        // POST: api/Proyecto
        [HttpPost]
        public async Task<ActionResult<Proyecto>> PostProyecto(CreateProyectoDTO proyectoDto)
        {
            var proyecto = new Proyecto
            {
                Nombre = proyectoDto.Nombre,
                Descripcion = proyectoDto.Descripcion,
                FechaInicio = proyectoDto.FechaInicio,
                FechaFin = proyectoDto.FechaFin,
                Avance = proyectoDto.Avance
            };

            await _proyectoRepository.AddAsync(proyecto);
            return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.Id }, proyecto);
        }

        // PUT: api/Proyecto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProyecto(int id, CreateProyectoDTO proyectoDto)
        {
            var proyecto = await _proyectoRepository.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            proyecto.Nombre = proyectoDto.Nombre;
            proyecto.Descripcion = proyectoDto.Descripcion;
            proyecto.FechaInicio = proyectoDto.FechaInicio;
            proyecto.FechaFin = proyectoDto.FechaFin;
            proyecto.Avance = proyectoDto.Avance;

            await _proyectoRepository.UpdateAsync(proyecto);
            return NoContent();
        }

        // DELETE: api/Proyecto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProyecto(int id)
        {
            var proyecto = await _proyectoRepository.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            await _proyectoRepository.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/Proyecto/Avance/{proyectoId}
        [HttpGet("Avance/{proyectoId}")]
        public async Task<ActionResult> GetAvanceProyecto(int proyectoId)
        {
            var proyecto = await _proyectoRepository.GetByIdAsync(proyectoId);
            if (proyecto == null)
            {
                return NotFound("Proyecto no encontrado.");
            }

            var tareas = proyecto.Tareas ?? new List<Tarea>();
            var totalTareas = tareas.Count;
            var tareasCompletadas = tareas.Count(t => t.StatusId == 4);

            var avance = MetricsService.Instance.CalcularAvanceProyecto(totalTareas, tareasCompletadas);

            return Ok(new
            {
                ProyectoId = proyecto.Id,
                Nombre = proyecto.Nombre,
                TotalTareas = totalTareas,
                TareasCompletadas = tareasCompletadas,
                Avance = avance
            });
        }

        // GET: api/Proyecto/CompararProyectos
        [HttpGet("CompararProyectos")]
        public async Task<ActionResult> CompararProyectos(int proyecto1Id, int proyecto2Id)
        {
            var proyectos = await _proyectoRepository.GetAllAsync();
            var proyecto1 = proyectos.FirstOrDefault(p => p.Id == proyecto1Id);
            var proyecto2 = proyectos.FirstOrDefault(p => p.Id == proyecto2Id);

            if (proyecto1 == null || proyecto2 == null)
            {
                return NotFound("Uno o ambos proyectos no existen.");
            }

            // Convertir las colecciones a listas para evitar el error
            var tareasProyecto1 = proyecto1.Tareas?.ToList() ?? new List<Tarea>();
            var tareasProyecto2 = proyecto2.Tareas?.ToList() ?? new List<Tarea>();

            var resumenProyecto1 = CalcularResumenProyecto(tareasProyecto1);
            var resumenProyecto2 = CalcularResumenProyecto(tareasProyecto2);

            var comparacion = new
            {
                Proyecto1 = new
                {
                    proyecto1.Nombre,
                    TotalTareas = resumenProyecto1.TotalTareas,
                    TareasPendientes = resumenProyecto1.TareasPendientes,
                    TareasCompletadas = resumenProyecto1.TareasCompletadas,
                    Progreso = resumenProyecto1.PorcentajeProgreso,
                    Retrasos = resumenProyecto1.Retrasos
                },
                Proyecto2 = new
                {
                    proyecto2.Nombre,
                    TotalTareas = resumenProyecto2.TotalTareas,
                    TareasPendientes = resumenProyecto2.TareasPendientes,
                    TareasCompletadas = resumenProyecto2.TareasCompletadas,
                    Progreso = resumenProyecto2.PorcentajeProgreso,
                    Retrasos = resumenProyecto2.Retrasos
                }
            };

            return Ok(comparacion);
        }

        private dynamic CalcularResumenProyecto(List<Tarea> tareas)
        {
            var totalTareas = tareas.Count;
            var tareasCompletadas = tareas.Count(t => t.StatusId == 4);
            var tareasPendientes = totalTareas - tareasCompletadas;
            var tareasRetrasadas = tareas.Count(t => t.FechaReal.HasValue && t.FechaReal > t.FechaEstimada);

            return new
            {
                TotalTareas = totalTareas,
                TareasCompletadas = tareasCompletadas,
                TareasPendientes = tareasPendientes,
                Retrasos = tareasRetrasadas,
                PorcentajeProgreso = MetricsService.Instance.CalcularAvanceProyecto(totalTareas, tareasCompletadas)
            };
        }
    }
}
