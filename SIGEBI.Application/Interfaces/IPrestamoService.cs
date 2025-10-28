using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Application.Prestamos.Commands;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Application.Interfaces;

public interface IPrestamoService
{
    Task<Prestamo> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Prestamo>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default);
    Task<IReadOnlyList<Prestamo>> ObtenerActivosPorLibroAsync(Guid libroId, CancellationToken ct = default);
    Task<IReadOnlyList<Prestamo>> ObtenerVencidosAsync(DateTime referenciaUtc, CancellationToken ct = default);
    Task<Prestamo> CrearAsync(CrearPrestamoCommand command, CancellationToken ct = default);
    Task<Prestamo> ActivarAsync(ActivarPrestamoCommand command, CancellationToken ct = default);
    Task<Prestamo> RegistrarDevolucionAsync(RegistrarDevolucionCommand command, CancellationToken ct = default);
    Task<Prestamo> CancelarAsync(CancelarPrestamoCommand command, CancellationToken ct = default);
    Task<Prestamo> ExtenderAsync(ExtenderPrestamoCommand command, CancellationToken ct = default);
}
