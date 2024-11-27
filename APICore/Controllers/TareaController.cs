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
        public async Task<ActionResult> GetTareasPendientes(string usuarioId)
        {
            var tareasPendientes = await _context.Tareas
                .Include(t => t.Categoria) // Incluye la relación con Categoría
                .Where(t => t.UsuarioId == usuarioId && (t.StatusId == 1 || t.StatusId == 2))
                .Select(t => new
                {
                    t.Id,
                    t.ProyectoId,
                    t.UsuarioId,
                    t.CategoriaId,
                    CategoriaNombre = t.Categoria.Nombre, // Proyecta el nombre de la categoría
                    t.StatusId,
                    t.Nombre,
                    t.Descripcion,
                    t.FechaEstimada
                })
                .ToListAsync();

            return Ok(tareasPendientes);
        }
        //get completadas
        [HttpGet("completadas/{usuarioId}")]
        public async Task<ActionResult> GetTareasCompletadas(string usuarioId)
        {
            var tareasCompletadas = await _context.Tareas
                .Include(t => t.Categoria) // Incluye la relación con Categoría
                .Where(t => t.UsuarioId == usuarioId && t.StatusId == 4) // Solo completadas
                .Select(t => new
                {
                    t.Id,
                    t.ProyectoId,
                    t.UsuarioId,
                    t.CategoriaId,
                    CategoriaNombre = t.Categoria.Nombre, // Proyecta el nombre de la categoría
                    t.StatusId,
                    t.Nombre,
                    t.Descripcion,
                    t.FechaEstimada,
                    t.FechaReal
                })
                .ToListAsync();

            return Ok(tareasCompletadas);
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

                // Validar que la fecha estimada esté dentro del rango de fechas del proyecto
                if (tareaDto.FechaEstimada < proyecto.FechaInicio || tareaDto.FechaEstimada > proyecto.FechaFin)
                {
                    return BadRequest("La fecha estimada de la tarea debe estar dentro del rango de fechas del proyecto.");
                }

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

            // Si el estado es "Completada" el ID de "Completada" es 4, establecer la fecha real si no está ya definida
            if (updateStatusDto.StatusId == 4 && !tarea.FechaReal.HasValue)
            {
                tarea.FechaReal = DateTime.Now;
            }

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

                var proyecto = await _context.Proyectos.FindAsync(updateTaskDto.ProyectoId);
                if (proyecto == null)
                {
                    return BadRequest("El proyecto especificado no existe.");
                }

                // Validar que la fecha estimada esté dentro del rango de fechas del proyecto
                if (updateTaskDto.FechaEstimada < proyecto.FechaInicio || updateTaskDto.FechaEstimada > proyecto.FechaFin)
                {
                    return BadRequest("La fecha estimada de la tarea debe estar dentro del rango de fechas del proyecto.");
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

        //METODOS PARA EL CORE
        //EMPLEADO PRODUCTIVIDAD
        // GET: api/Tarea/ProductividadEmpleado/{usuarioId}
        [HttpGet("ProductividadEmpleado/{usuarioId}")]
        public async Task<ActionResult> GetProductividadEmpleado(string usuarioId, int? categoriaId = null, int? proyectoId = null)
        {
            try
            {
                // Filtrar todas las tareas del empleado para determinar el total de tareas
                var todasTareasEmpleado = _context.Tareas
                    .Where(t => t.UsuarioId == usuarioId);

                // Filtrar las tareas según los parámetros (categoría o proyecto)
                var tareasFiltradas = todasTareasEmpleado
                    .Include(t => t.Categoria)
                    .Include(t => t.Proyecto)
                    .AsQueryable();

                if (categoriaId.HasValue)
                {
                    tareasFiltradas = tareasFiltradas.Where(t => t.CategoriaId == categoriaId.Value);
                }

                if (proyectoId.HasValue)
                {
                    tareasFiltradas = tareasFiltradas.Where(t => t.ProyectoId == proyectoId.Value);
                }

                // Obtener el total de tareas sin filtros
                var totalTareas = await todasTareasEmpleado.CountAsync();

                // Obtener el total de tareas filtradas
                var totalTareasFiltradas = await tareasFiltradas.CountAsync();

                // Filtrar las tareas completadas
                var tareasCompletadas = await tareasFiltradas
                    .Where(t => t.FechaReal.HasValue && t.StatusId == 4) // Solo tareas completadas
                    .ToListAsync();

                // Calcular las tareas completadas a tiempo
                var tareasATiempo = tareasCompletadas
                    .Count(t => t.FechaReal <= t.FechaEstimada);

                // Si no hay tareas filtradas, devolvemos valores vacíos
                if (totalTareasFiltradas == 0)
                {
                    return Ok(new
                    {
                        TotalTareas = totalTareas, // Todas las tareas del empleado
                        TotalTareasFiltradas = 0,
                        TareasCompletadasATiempo = 0,
                        Productividad = "Sin datos",
                        TiempoPromedioDeCompletitud = "N/A",
                        DistribucionCategorias = new List<object>()
                    });
                }

                // Calcular la productividad como un porcentaje
                var productividad = (double)tareasATiempo / totalTareasFiltradas * 100;

                // Calcular el tiempo promedio de completitud (en días) para tareas completadas
                double? tiempoPromedioDeCompletitud = null;
                if (tareasCompletadas.Any())
                {
                    tiempoPromedioDeCompletitud = tareasCompletadas
                        .Average(t => (t.FechaReal.Value - t.FechaEstimada).TotalDays);
                }

                // Distribución de tareas por categoría
                var distribucionCategorias = await tareasFiltradas
                    .GroupBy(t => t.Categoria.Nombre)
                    .Select(g => new
                    {
                        Categoria = g.Key,
                        Cantidad = g.Count(),
                        Porcentaje = (double)g.Count() / totalTareasFiltradas * 100
                    })
                    .ToListAsync();

                // Devolvemos los datos correctamente estructurados
                return Ok(new
                {
                    TotalTareas = totalTareas, // Todas las tareas del empleado
                    TotalTareasFiltradas = totalTareasFiltradas, // Tareas según los filtros aplicados
                    TareasCompletadasATiempo = tareasATiempo,
                    Productividad = productividad,
                    TiempoPromedioDeCompletitud = tiempoPromedioDeCompletitud.HasValue
                        ? tiempoPromedioDeCompletitud.Value.ToString("F2")
                        : "N/A",
                    DistribucionCategorias = distribucionCategorias
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProductividadEmpleado: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }


    }




}