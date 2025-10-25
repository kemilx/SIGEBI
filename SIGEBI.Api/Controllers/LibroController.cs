using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using SIGEBI.Api.Dtos;
using SIGEBI.Application.Common.Exceptions;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibroController : ControllerBase
{
    private readonly ILibroRepository _libroRepository;

    public LibroController(ILibroRepository libroRepository)
    {
        _libroRepository = libroRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LibroDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct);
        if (libro is null) return NotFound();

        return Ok(Map(libro));
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<LibroDto>>> Buscar([FromQuery] string? titulo, [FromQuery] string? autor, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(titulo) && string.IsNullOrWhiteSpace(autor))
        {
            throw ValidationError(nameof(titulo), "Debe indicar un texto de búsqueda por título o autor.");
        }

        if (!string.IsNullOrWhiteSpace(titulo))
        {
            var resultadosTitulo = await _libroRepository.BuscarPorTituloAsync(titulo.Trim(), ct);
            return Ok(resultadosTitulo.Select(Map));
        }

        var resultadosAutor = await _libroRepository.BuscarPorAutorAsync(autor!.Trim(), ct);
        return Ok(resultadosAutor.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<LibroDto>> Crear([FromBody] CrearLibroRequest request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Isbn) &&
            await _libroRepository.IsbnExisteAsync(request.Isbn, null, ct))
        {
            throw new ConflictException("El ISBN indicado ya se encuentra registrado.");
        }

        try
        {
            var libro = Libro.Create(
                request.Titulo,
                request.Autor,
                request.EjemplaresTotales,
                request.Isbn,
                request.Ubicacion,
                request.FechaPublicacionUtc);

            await _libroRepository.AddAsync(libro, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = libro.Id }, Map(libro));
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Libro), ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LibroDto>> Actualizar(Guid id, [FromBody] ActualizarLibroRequest request, CancellationToken ct)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct);
        if (libro is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.Isbn) &&
            await _libroRepository.IsbnExisteAsync(request.Isbn, id, ct))
        {
            throw new ConflictException("El ISBN indicado ya se encuentra registrado.");
        }

        try
        {
            libro.ActualizarDatos(request.Titulo, request.Autor, request.Isbn, request.FechaPublicacionUtc);
            await _libroRepository.UpdateAsync(libro, ct);
            return Ok(Map(libro));
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Libro), ex.Message);
        }
    }

    [HttpPut("{id:guid}/ubicacion")]
    public async Task<ActionResult<LibroDto>> ActualizarUbicacion(Guid id, [FromBody] ActualizarUbicacionRequest request, CancellationToken ct)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct);
        if (libro is null) return NotFound();

        try
        {
            libro.ActualizarUbicacion(request.Ubicacion);
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Libro), ex.Message);
        }

        await _libroRepository.UpdateAsync(libro, ct);
        return Ok(Map(libro));
    }

    [HttpPost("{id:guid}/estado")]
    public async Task<ActionResult<LibroDto>> CambiarEstado(Guid id, [FromBody] CambiarEstadoLibroRequest request, CancellationToken ct)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct);
        if (libro is null) return NotFound();

        try
        {
            switch (request.Estado)
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
                        throw ValidationError(nameof(request.Estado), "No es posible marcar como disponible sin ejemplares prestados.");
                    }
                    break;
                default:
                    throw ValidationError(nameof(request.Estado), "Estado no soportado.");
            }
        }
        catch (InvalidOperationException ex)
        {
            throw ValidationError(nameof(request.Estado), ex.Message);
        }

        await _libroRepository.UpdateAsync(libro, ct);
        return Ok(Map(libro));

    }

    private static ValidationException ValidationError(string property, string message)
        => new(new[] { new ValidationFailure(property, message) });
}

    private static LibroDto Map(Libro libro)
        => new(
            libro.Id,
            libro.Titulo,
            libro.Autor,
            libro.Isbn,
            libro.Ubicacion,
            libro.EjemplaresTotales,
            libro.EjemplaresDisponibles,
            libro.Estado,
            libro.FechaPublicacion,
            libro.CreatedAtUtc,
            libro.UpdatedAtUtc);
}