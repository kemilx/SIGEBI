using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Repository
{
    public interface INotificacionRepository
    {
        Task AddAsync(Notificacion notificacion, CancellationToken ct = default);
        Task UpdateAsync(Notificacion notificacion, CancellationToken ct = default);
        Task<IReadOnlyList<Notificacion>> ObtenerNoLeidasPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
        Task<int> ContarNoLeidasAsync(Guid usuarioId, CancellationToken ct = default);
    }
}