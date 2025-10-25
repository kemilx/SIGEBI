using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Repository
{
    public interface IRolRepository
    {
        Task<Rol?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Rol?> GetByNombreAsync(string nombre, CancellationToken ct = default);
        Task<IReadOnlyList<Rol>> ListAsync(CancellationToken ct = default);
        Task AddAsync(Rol rol, CancellationToken ct = default);
        Task UpdateAsync(Rol rol, CancellationToken ct = default);
        Task RemoveAsync(Rol rol, CancellationToken ct = default);
        Task<bool> NombreExisteAsync(string nombre, CancellationToken ct = default);
    }
}
