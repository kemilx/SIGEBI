using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Api.Dtos;

public record PrestamoDto(
    Guid Id,
    Guid LibroId,
    Guid UsuarioId,
    EstadoPrestamo Estado,
    DateTime FechaInicioUtc,
    DateTime FechaFinCompromisoUtc,
    DateTime? FechaEntregaRealUtc,
    string? Observaciones,
    DateTime CreadoUtc,
    DateTime? ActualizadoUtc);

public record CrearPrestamoRequest(
    Guid LibroId,
    Guid UsuarioId,
    DateTime FechaInicioUtc,
    DateTime FechaFinUtc);

public record RegistrarDevolucionRequest(
    DateTime FechaEntregaUtc,
    string? Observaciones);

public record CancelarPrestamoRequest(string Motivo);

public record ExtenderPrestamoRequest(int Dias);
