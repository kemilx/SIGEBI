using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> EmailExisteAsync(string email, CancellationToken ct = default);
        Task AddAsync(Usuario usuario, CancellationToken ct = default);
        Task UpdateAsync(Usuario usuario, CancellationToken ct = default);
        Task<int> ContarActivosAsync(CancellationToken ct = default);
    }
}