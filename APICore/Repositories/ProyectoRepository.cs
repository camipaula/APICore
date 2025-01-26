using APICore.Data;
using APICore.Models;
using Microsoft.EntityFrameworkCore;

namespace APICore.Repositories
{
    public class ProyectoRepository : IRepository<Proyecto>
    {
        private readonly ApplicationDbContext _context;

        public ProyectoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los proyectos con las tareas incluidas
        public async Task<IEnumerable<Proyecto>> GetAllAsync()
        {
            return await _context.Proyectos
                .Include(p => p.Tareas) // Incluir las tareas relacionadas
                .ToListAsync();
        }

        // Obtener un proyecto específico con las tareas incluidas
        public async Task<Proyecto> GetByIdAsync(int id)
        {
            return await _context.Proyectos
                .Include(p => p.Tareas) // Incluir las tareas relacionadas
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Agregar un nuevo proyecto
        public async Task AddAsync(Proyecto entity)
        {
            _context.Proyectos.Add(entity);
            await _context.SaveChangesAsync();
        }

        // Actualizar un proyecto existente
        public async Task UpdateAsync(Proyecto entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Eliminar un proyecto por su ID
        public async Task DeleteAsync(int id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto != null)
            {
                _context.Proyectos.Remove(proyecto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
