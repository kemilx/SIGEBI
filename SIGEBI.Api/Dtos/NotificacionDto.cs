namespace SIGEBI.Api.Dtos;

public record NotificacionDto(
    Guid Id,
    Guid UsuarioId,
    string Titulo,
    string Mensaje,
    string Tipo,
    bool Leida,
    DateTime CreadaUtc,
    DateTime? FechaLecturaUtc,
    DateTime? ActualizadoUtc);

public record CrearNotificacionRequest(
    Guid UsuarioId,
    string Titulo,
    string Mensaje,
    string Tipo);
