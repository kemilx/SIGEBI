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
    string Titulo,
    string Autor,
    int EjemplaresTotales,
    string? Isbn,
    string? Ubicacion,
    DateTime? FechaPublicacionUtc);

public record ActualizarLibroRequest(
    string? Titulo,
    string? Autor,
    string? Isbn,
    DateTime? FechaPublicacionUtc);

public record ActualizarUbicacionRequest(string? Ubicacion);

public record CambiarEstadoLibroRequest(EstadoLibro Estado);
