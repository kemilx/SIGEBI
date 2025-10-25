using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SIGEBI.Application.Interfaces;

public interface IReporteService
{
    Task<IReadOnlyDictionary<string, int>> ObtenerLibrosPorEstadoAsync(CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, int>> ObtenerPrestamosPorEstadoAsync(CancellationToken ct = default);
    Task<int> ContarPenalizacionesActivasAsync(CancellationToken ct = default);
    Task<int> ContarUsuariosActivosAsync(CancellationToken ct = default);
}
