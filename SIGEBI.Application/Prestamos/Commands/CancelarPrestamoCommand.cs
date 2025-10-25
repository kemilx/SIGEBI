namespace SIGEBI.Application.Prestamos.Commands;

public sealed record CancelarPrestamoCommand(
    Guid PrestamoId,
    string Motivo);
