using System;
using System.Threading;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Repository
{
    public interface IAdminRepository
    {
        Task<int> ContarUsuariosAsync(CancellationToken ct = default);
        Task<int> ContarLibrosAsync(CancellationToken ct = default);
        Task<int> ContarPrestamosActivosAsync(CancellationToken ct = default);
        Task<int> ContarPrestamosVencidosAsync(CancellationToken ct = default);
        Task<int> ContarPenalizacionesActivasAsync(CancellationToken ct = default);
    }
}