using SIGEBI.Application.Prestamos.Commands;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Application.Prestamos.Services;

public interface IPrestamoService
{
    Task<Prestamo> CrearAsync(CrearPrestamoCommand command, CancellationToken ct = default);
    Task<Prestamo> ActivarAsync(ActivarPrestamoCommand command, CancellationToken ct = default);
    Task<Prestamo> RegistrarDevolucionAsync(RegistrarDevolucionCommand command, CancellationToken ct = default);
    Task<Prestamo> CancelarAsync(CancelarPrestamoCommand command, CancellationToken ct = default);
    Task<Prestamo> ExtenderAsync(ExtenderPrestamoCommand command, CancellationToken ct = default);
}
