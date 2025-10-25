using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibroController : ControllerBase
{
    private readonly ILibroService _libroService;

    public LibroController(ILibroService libroService)
    {
        _libroService = libroService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LibroDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var libro = await _libroService.ObtenerPorIdAsync(id, ct);
        return Ok(Map(libro));
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<LibroDto>>> Buscar([FromQuery] string? titulo, [FromQuery] string? autor, CancellationToken ct)
    {
        var resultados = await _libroService.BuscarAsync(titulo, autor, ct);
        return Ok(resultados.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<LibroDto>> Crear([FromBody] CrearLibroRequest request, CancellationToken ct)
    {
        var libro = await _libroService.CrearAsync(
            request.Titulo,
            request.Autor,
            request.EjemplaresTotales,
            request.Isbn,
            request.Ubicacion,
            request.FechaPublicacionUtc,
            ct);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = libro.Id }, Map(libro));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LibroDto>> Actualizar(Guid id, [FromBody] ActualizarLibroRequest request, CancellationToken ct)
    {
        var libro = await _libroService.ActualizarAsync(id, request.Titulo, request.Autor, request.Isbn, request.FechaPublicacionUtc, ct);
        return Ok(Map(libro));
    }

    [HttpPut("{id:guid}/ubicacion")]
    public async Task<ActionResult<LibroDto>> ActualizarUbicacion(Guid id, [FromBody] ActualizarUbicacionRequest request, CancellationToken ct)
    {
        var libro = await _libroService.ActualizarUbicacionAsync(id, request.Ubicacion, ct);
        return Ok(Map(libro));
    }

    [HttpPost("{id:guid}/estado")]
    public async Task<ActionResult<LibroDto>> CambiarEstado(Guid id, [FromBody] CambiarEstadoLibroRequest request, CancellationToken ct)
    {
        var libro = await _libroService.CambiarEstadoAsync(id, request.Estado, ct);
        return Ok(Map(libro));
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
