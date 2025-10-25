using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Repository
{
    public interface ILibroRepository
    {
        Task<Libro?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Libro libro, CancellationToken ct = default);
        Task UpdateAsync(Libro libro, CancellationToken ct = default);
        Task<IReadOnlyList<Libro>> BuscarPorTituloAsync(string texto, CancellationToken ct = default);
        Task<IReadOnlyList<Libro>> BuscarPorAutorAsync(string autor, CancellationToken ct = default);
        Task<int> ContarDisponiblesAsync(CancellationToken ct = default);
        Task<int> ContarPorEstadoAsync(EstadoLibro estado, CancellationToken ct = default);
        Task<bool> IsbnExisteAsync(string isbn, Guid? excluirId = null, CancellationToken ct = default);
    }
}