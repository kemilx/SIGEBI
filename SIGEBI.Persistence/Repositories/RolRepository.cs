using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Persistence.Repositories
{
    // No existe IRolRepository en Domain según las imágenes; se implementa
    // repositorio concreto mínimo para operaciones típicas sobre Roles.
    public class RolRepository : IRolRepository
    {
        private readonly SIGEBIDbContext _context;

        public RolRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task<Rol?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<Rol?> GetByNombreAsync(string nombre, CancellationToken ct = default)
            => await _context.Roles.FirstOrDefaultAsync(r =>
                r.Nombre.ToLower() == nombre.ToLower(), ct);

        public async Task<IReadOnlyList<Rol>> ListAsync(CancellationToken ct = default)
            => await _context.Roles
                             .OrderBy(r => r.Nombre)
                             .ToListAsync(ct);

        public async Task AddAsync(Rol rol, CancellationToken ct = default)
        {
            await _context.Roles.AddAsync(rol, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Rol rol, CancellationToken ct = default)
        {
            _context.Roles.Update(rol);
            await _context.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(Rol rol, CancellationToken ct = default)
        {
            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> NombreExisteAsync(string nombre, CancellationToken ct = default)
            => await _context.Roles.AnyAsync(r => r.Nombre.ToLower() == nombre.ToLower(), ct);
    }
}