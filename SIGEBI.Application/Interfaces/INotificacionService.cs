using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Application.Interfaces;

public interface INotificacionService
{
    Task<Notificacion> CrearAsync(Guid usuarioId, string titulo, string mensaje, string tipo, CancellationToken ct = default);
    Task<IReadOnlyList<Notificacion>> ObtenerPendientesAsync(Guid usuarioId, CancellationToken ct = default);
    Task<int> ContarPendientesAsync(Guid usuarioId, CancellationToken ct = default);
    Task<int> MarcarLeidasAsync(Guid usuarioId, CancellationToken ct = default);
}
