using System.ComponentModel.DataAnnotations;
using SIGEBI.Api.Validation;
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
    [property: NotEmptyGuid] Guid LibroId,
    [property: NotEmptyGuid] Guid UsuarioId,
    [property: Required] DateTime FechaInicioUtc,
    [property: Required] DateTime FechaFinUtc);

public record RegistrarDevolucionRequest(
    [property: Required] DateTime FechaEntregaUtc,
    [property: StringLength(500)] string? Observaciones);

public record CancelarPrestamoRequest([property: Required, StringLength(500)] string Motivo);

public record ExtenderPrestamoRequest([property: Range(1, int.MaxValue)] int Dias);
