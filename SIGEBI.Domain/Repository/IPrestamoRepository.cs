using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Repository
{
    public interface IPrestamoRepository
    {
        Task<Prestamo?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Prestamo prestamo, CancellationToken ct = default);
        Task UpdateAsync(Prestamo prestamo, CancellationToken ct = default);
        Task<IReadOnlyList<Prestamo>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
        Task<IReadOnlyList<Prestamo>> ObtenerActivosPorLibroAsync(Guid libroId, CancellationToken ct = default);
        Task<IReadOnlyList<Prestamo>> ObtenerVencidosAsync(DateTime referenciaUtc, CancellationToken ct = default);
        Task<int> ContarPorEstadoAsync(EstadoPrestamo estado, CancellationToken ct = default);
        Task<bool> ExistePrestamoActivoOPendienteAsync(Guid libroId, Guid usuarioId, CancellationToken ct = default);
    }
}