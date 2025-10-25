using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Persistence.Repositories
{
    public class PenalizacionRepository : IPenalizacionRepository
    {
        private readonly SIGEBIDbContext _context;

        public PenalizacionRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task<Penalizacion?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Penalizaciones.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task AddAsync(Penalizacion penalizacion, CancellationToken ct = default)
        {
            await _context.Penalizaciones.AddAsync(penalizacion, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Penalizacion penalizacion, CancellationToken ct = default)
        {
            _context.Penalizaciones.Update(penalizacion);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Penalizacion>> ObtenerActivasPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
            => await _context.Penalizaciones
                             .Where(p => p.UsuarioId == usuarioId && p.Activa)
                             .OrderByDescending(p => p.FechaInicioUtc)
                             .ToListAsync(ct);

        public async Task<int> ContarActivasAsync(CancellationToken ct = default)
            => await _context.Penalizaciones.CountAsync(p => p.Activa, ct);
    }
}