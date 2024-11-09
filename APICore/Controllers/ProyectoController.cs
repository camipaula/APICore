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
    }
}
 



        