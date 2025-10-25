using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Repository
{
    public interface IPenalizacionRepository
    {
        Task<Penalizacion?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Penalizacion penalizacion, CancellationToken ct = default);
        Task UpdateAsync(Penalizacion penalizacion, CancellationToken ct = default);
        Task<IReadOnlyList<Penalizacion>> ObtenerActivasPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
        Task<int> ContarActivasAsync(CancellationToken ct = default);
    }
}