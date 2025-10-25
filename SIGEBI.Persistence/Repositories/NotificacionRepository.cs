using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Persistence.Repositories
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly SIGEBIDbContext _context;

        public NotificacionRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notificacion notificacion, CancellationToken ct = default)
        {
            await _context.Notificaciones.AddAsync(notificacion, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Notificacion notificacion, CancellationToken ct = default)
        {
            _context.Notificaciones.Update(notificacion);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Notificacion>> ObtenerNoLeidasPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
            => await _context.Notificaciones
                             .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                             .OrderByDescending(n => n.CreatedAtUtc)
                             .ToListAsync(ct);

        public async Task<int> ContarNoLeidasAsync(Guid usuarioId, CancellationToken ct = default)
            => await _context.Notificaciones
                             .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida, ct);
    }
}