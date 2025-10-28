using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Application.Interfaces;

public interface IPenalizacionService
{
    Task<IReadOnlyList<Penalizacion>> ObtenerActivasAsync(Guid usuarioId, CancellationToken ct = default);
    Task<Penalizacion> CrearAsync(Guid usuarioId, Guid? prestamoId, decimal monto, DateTime fechaInicioUtc, DateTime fechaFinUtc, string motivo, CancellationToken ct = default);
    Task<Penalizacion> CerrarAsync(Guid penalizacionId, string razon, CancellationToken ct = default);
}
