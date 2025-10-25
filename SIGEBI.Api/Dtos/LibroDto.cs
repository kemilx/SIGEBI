using System.ComponentModel.DataAnnotations;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Api.Dtos;

public record LibroDto(
    Guid Id,
    string Titulo,
    string Autor,
    string? Isbn,
    string? Ubicacion,
    int EjemplaresTotales,
    int EjemplaresDisponibles,
    EstadoLibro Estado,
    DateTime? FechaPublicacion,
    DateTime CreadoUtc,
    DateTime? ActualizadoUtc);

public record CrearLibroRequest(
    [property: Required, StringLength(250)] string Titulo,
    [property: Required, StringLength(200)] string Autor,
    [property: Range(1, int.MaxValue)] int EjemplaresTotales,
    [property: StringLength(40)] string? Isbn,
    [property: StringLength(100)] string? Ubicacion,
    DateTime? FechaPublicacionUtc);

public record ActualizarLibroRequest(
    [property: StringLength(250)] string? Titulo,
    [property: StringLength(200)] string? Autor,
    [property: StringLength(40)] string? Isbn,
    DateTime? FechaPublicacionUtc);

public record ActualizarUbicacionRequest([property: StringLength(100)] string? Ubicacion);

public record CambiarEstadoLibroRequest([property: Required] EstadoLibro Estado);
