namespace SIGEBI.Application.Prestamos.Commands;

public sealed record ExtenderPrestamoCommand(
    Guid PrestamoId,
    int Dias);
