using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Application.Interfaces;

public interface ILibroService
{
    Task<Libro> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Libro>> BuscarAsync(string? titulo, string? autor, CancellationToken ct = default);
    Task<Libro> CrearAsync(string titulo, string autor, int ejemplaresTotales, string? isbn, string? ubicacion, DateTime? fechaPublicacionUtc, CancellationToken ct = default);
    Task<Libro> ActualizarAsync(Guid id, string? titulo, string? autor, string? isbn, DateTime? fechaPublicacionUtc, CancellationToken ct = default);
    Task<Libro> ActualizarUbicacionAsync(Guid id, string? ubicacion, CancellationToken ct = default);
    Task<Libro> CambiarEstadoAsync(Guid id, EstadoLibro estado, CancellationToken ct = default);
}
