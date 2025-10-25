using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using SIGEBI.Application.Common.Exceptions;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Application.Services;

public sealed class LibroService : ILibroService
{
    private readonly ILibroRepository _libroRepository;

    public LibroService(ILibroRepository libroRepository)
    {
        _libroRepository = libroRepository;
    }

    public async Task<Libro> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => await _libroRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Libro), id);

    public async Task<IReadOnlyList<Libro>> BuscarAsync(string? titulo, string? autor, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(autor))
        {
            throw ValidationError(nameof(titulo), "Debe indicar un texto de búsqueda por título o autor.");
        }

        if (!string.IsNullOrWhiteSpace(titulo))
        {
            return await _libroRepository.BuscarPorTituloAsync(titulo.Trim(), ct);
        }

        return await _libroRepository.BuscarPorAutorAsync(autor!.Trim(), ct);
    }

    public async Task<Libro> CrearAsync(
        string titulo,
        string autor,
        int ejemplaresTotales,
        string? isbn,
        string? ubicacion,
        DateTime? fechaPublicacionUtc,
        CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(isbn) &&
            await _libroRepository.IsbnExisteAsync(isbn.Trim(), null, ct))
        {
            throw new ConflictException("El ISBN indicado ya se encuentra registrado.");
        }

        try
        {
            var libro = Libro.Create(titulo, autor, ejemplaresTotales, isbn, ubicacion, fechaPublicacionUtc);
            await _libroRepository.AddAsync(libro, ct);
            return libro;
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Libro), ex.Message);
        }
    }

    public async Task<Libro> ActualizarAsync(
        Guid id,
        string? titulo,
        string? autor,
        string? isbn,
        DateTime? fechaPublicacionUtc,
        CancellationToken ct = default)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Libro), id);

        if (!string.IsNullOrWhiteSpace(isbn))
        {
            var isbnLimpio = isbn.Trim();
            if (!string.Equals(libro.Isbn, isbnLimpio, StringComparison.OrdinalIgnoreCase) &&
                await _libroRepository.IsbnExisteAsync(isbnLimpio, id, ct))
            {
                throw new ConflictException("El ISBN indicado ya se encuentra registrado.");
            }
        }

        try
        {
            libro.ActualizarDatos(titulo, autor, isbn, fechaPublicacionUtc);
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Libro), ex.Message);
        }

        await _libroRepository.UpdateAsync(libro, ct);
        return libro;
    }

    public async Task<Libro> ActualizarUbicacionAsync(Guid id, string? ubicacion, CancellationToken ct = default)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Libro), id);

        try
        {
            libro.ActualizarUbicacion(ubicacion);
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Libro), ex.Message);
        }

        await _libroRepository.UpdateAsync(libro, ct);
        return libro;
    }

    public async Task<Libro> CambiarEstadoAsync(Guid id, EstadoLibro estado, CancellationToken ct = default)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Libro), id);

        try
        {
            switch (estado)
            {
                case EstadoLibro.Reservado:
                    libro.MarcarReservado();
                    break;
                case EstadoLibro.Dañado:
                    libro.MarcarDañado();
                    break;
                case EstadoLibro.Inactivo:
                    libro.MarcarInactivo();
                    break;
                case EstadoLibro.Prestado:
                    libro.MarcarPrestado();
                    break;
                case EstadoLibro.Disponible:
                    if (libro.EjemplaresDisponibles < libro.EjemplaresTotales)
                    {
                        libro.MarcarDevuelto();
                    }
                    else if (libro.Estado != EstadoLibro.Disponible)
                    {
                        throw ValidationError(nameof(estado), "No es posible marcar como disponible sin ejemplares prestados.");
                    }
                    break;
                default:
                    throw ValidationError(nameof(estado), "Estado no soportado.");
            }
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Libro), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            throw ValidationError(nameof(estado), ex.Message);
        }

        await _libroRepository.UpdateAsync(libro, ct);
        return libro;
    }

    private static ValidationException ValidationError(string property, string message)
        => new(new[] { new ValidationFailure(property, message) });
}
