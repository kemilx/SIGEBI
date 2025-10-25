using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence.Repositories
{
    public class PrestamoRepository : IPrestamoRepository
    {
        private readonly SIGEBIDbContext _context;

        public PrestamoRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task<Prestamo?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Prestamos.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task AddAsync(Prestamo prestamo, CancellationToken ct = default)
        {
            await _context.Prestamos.AddAsync(prestamo, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Prestamo prestamo, CancellationToken ct = default)
        {
            _context.Prestamos.Update(prestamo);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Prestamo>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
            => await _context.Prestamos
                             .Where(p => p.UsuarioId == usuarioId)
                             .OrderByDescending(p => p.CreatedAtUtc)
                             .ToListAsync(ct);

        public async Task<IReadOnlyList<Prestamo>> ObtenerActivosPorLibroAsync(Guid libroId, CancellationToken ct = default)
            => await _context.Prestamos
                             .Where(p => p.LibroId == libroId && (p.Estado == EstadoPrestamo.Activo || p.Estado == EstadoPrestamo.Vencido))
                             .ToListAsync(ct);

        public async Task<IReadOnlyList<Prestamo>> ObtenerVencidosAsync(DateTime referenciaUtc, CancellationToken ct = default)
            => await _context.Prestamos
                             .Where(p => p.Estado == EstadoPrestamo.Activo &&
                                         p.Periodo.FechaFinCompromisoUtc < referenciaUtc)
                             .ToListAsync(ct);

        public async Task<int> ContarPorEstadoAsync(EstadoPrestamo estado, CancellationToken ct = default)
            => await _context.Prestamos.CountAsync(p => p.Estado == estado, ct);

        public async Task<bool> ExistePrestamoActivoOPendienteAsync(Guid libroId, Guid usuarioId, CancellationToken ct = default)
            => await _context.Prestamos
                             .AsNoTracking()
                             .AnyAsync(p => p.LibroId == libroId &&
                                           p.UsuarioId == usuarioId &&
                                           (p.Estado == EstadoPrestamo.Pendiente || p.Estado == EstadoPrestamo.Activo), ct);
    }
}