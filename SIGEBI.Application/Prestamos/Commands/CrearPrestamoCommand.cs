namespace SIGEBI.Application.Prestamos.Commands;

public sealed record CrearPrestamoCommand(
    Guid LibroId,
    Guid UsuarioId,
    DateTime FechaInicioUtc,
    DateTime FechaFinUtc);
