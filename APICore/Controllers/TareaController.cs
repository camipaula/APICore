using APICore.Data;
using APICore.DTOs;
using APICore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TareaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TareaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateTareaDTO>>> GetTareas()
        {
            var tareas = await _context.Tareas
                .Select(t => new CreateTareaDTO
                {
                    Id = t.Id,
                    ProyectoId = t.ProyectoId,
                    UsuarioId = t.UsuarioId,
                    CategoriaId = t.CategoriaId,
                    StatusId = t.StatusId,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    FechaEstimada = t.FechaEstimada,
                    FechaReal = t.FechaReal
                })
                .ToListAsync();

            return Ok(tareas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreateTareaDTO>> GetTarea(int id)
        {
            var tarea = await _context.Tareas
                .Where(t => t.Id == id)
                .Select(t => new CreateTareaDTO
                {
                    Id = t.Id,
                    ProyectoId = t.ProyectoId,
                    UsuarioId = t.UsuarioId,
                    CategoriaId = t.CategoriaId,
                    StatusId = t.StatusId,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    FechaEstimada = t.FechaEstimada,
                    FechaReal = t.FechaReal
                })
                .FirstOrDefaultAsync();

            if (tarea == null)
            {
                return NotFound();
            }

            return Ok(tarea);
        }

        //GET DE TAREAS PENDIENTES POR EMPLEADO ESPECIFICO
        [HttpGet("pendientes/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<CreateTareaDTO>>> GetTareasPendientes(string usuarioId)
        {
            var tareasPendientes = await _context.Tareas
                .Where(t => t.UsuarioId == usuarioId && (t.StatusId == 1 || t.StatusId == 2))
                .Select(t => new CreateTareaDTO
                {
                    Id = t.Id,
                    ProyectoId = t.ProyectoId,
                    UsuarioId = t.UsuarioId,
                    CategoriaId = t.CategoriaId,
                    StatusId = t.StatusId,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    FechaEstimada = t.FechaEstimada,
                    FechaReal = t.FechaReal
                })
                .ToListAsync();

            return Ok(tareasPendientes);
        }





        [HttpPost]
        public async Task<ActionResult<CreateTareaDTO>> PostTarea(CreateTareaDTO tareaDto)
        {
            try
            {
                // Verificar la existencia de entidades relacionadas
                var usuario = await _context.Users.FindAsync(tareaDto.UsuarioId);
                if (usuario == null) return BadRequest("El usuario especificado no existe.");

                var proyecto = await _context.Proyectos.FindAsync(tareaDto.ProyectoId);
                if (proyecto == null) return BadRequest("El proyecto especificado no existe.");

                var categoria = await _context.Categorias.FindAsync(tareaDto.CategoriaId);
                if (categoria == null) return BadRequest("La categoría especificada no existe.");

                // Crear la tarea
                var tarea = new Tarea
                {
                    ProyectoId = tareaDto.ProyectoId,
                    UsuarioId = tareaDto.UsuarioId,
                    CategoriaId = tareaDto.CategoriaId,
                    StatusId = tareaDto.StatusId > 0 ? tareaDto.StatusId : 1, // Establecer StatusId en 1 si no se especifica
                    Nombre = tareaDto.Nombre,
                    Descripcion = tareaDto.Descripcion,
                    FechaEstimada = tareaDto.FechaEstimada,
                    FechaReal = tareaDto.FechaReal
                };

                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();

                tareaDto.Id = tarea.Id; // Asignar el Id generado a la DTO para devolverlo si es necesario
                return CreatedAtAction("GetTarea", new { id = tarea.Id }, tareaDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PostTarea: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al crear la tarea.");
            }
        }






        // Método para que el empleado actualice solo el estado de la tarea
        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDTO updateStatusDto)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound("La tarea no existe.");
            }

            // Actualizar solo el estado de la tarea
            tarea.StatusId = updateStatusDto.StatusId;

            _context.Entry(tarea).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al actualizar el estado de la tarea.");
            }

            return NoContent();
        }


        /*[HttpPut("{id}")]
         public async Task<IActionResult> PutTarea(int id, Tarea tarea)
         {
             if (id != tarea.Id)
             {
                 return BadRequest();
             }

             _context.Entry(tarea).State = EntityState.Modified;

             try
             {
                 await _context.SaveChangesAsync();
             }
             catch (DbUpdateConcurrencyException)
             {
                 if (!TareaExists(id))
                 {
                     return NotFound();
                 }
                 else
                 {
                     throw;
                 }
             }

             return NoContent();
         }*/

        // Método para que el coordinador actualice cualquier campo de la tarea
        [HttpPut("updateTask/{id}")]
        public async Task<IActionResult> UpdateTask(int id, CreateTareaDTO updateTaskDto)
        {
            try
            {
                var tarea = await _context.Tareas.FindAsync(id);
                if (tarea == null)
                {
                    return NotFound("La tarea no existe.");
                }

                // Actualizar todos los campos permitidos
                tarea.ProyectoId = updateTaskDto.ProyectoId;
                tarea.UsuarioId = updateTaskDto.UsuarioId;
                tarea.CategoriaId = updateTaskDto.CategoriaId;
                tarea.StatusId = updateTaskDto.StatusId;
                tarea.Nombre = updateTaskDto.Nombre;
                tarea.Descripcion = updateTaskDto.Descripcion;
                tarea.FechaEstimada = updateTaskDto.FechaEstimada;
                tarea.FechaReal = updateTaskDto.FechaReal;

                _context.Entry(tarea).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateTask: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al actualizar la tarea.");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.Id == id);
        }
    }

}