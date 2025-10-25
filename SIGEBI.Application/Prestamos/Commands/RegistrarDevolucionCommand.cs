namespace SIGEBI.Application.Prestamos.Commands;

public sealed record RegistrarDevolucionCommand(
    Guid PrestamoId,
    DateTime FechaEntregaUtc,
    string? Observaciones);
