namespace SIGEBI.Api.Dtos;

public record PenalizacionDto(
    Guid Id,
    Guid UsuarioId,
    Guid? PrestamoId,
    decimal Monto,
    DateTime FechaInicioUtc,
    DateTime FechaFinUtc,
    string Motivo,
    bool Activa,
    DateTime CreadoUtc,
    DateTime? ActualizadoUtc);

public record CrearPenalizacionRequest(
    Guid UsuarioId,
    Guid? PrestamoId,
    decimal Monto,
    DateTime FechaInicioUtc,
    DateTime FechaFinUtc,
    string Motivo);

public record CerrarPenalizacionRequest(string Razon);
