namespace SIGEBI.Application.Models;

public sealed record AdminSummary(
    int TotalUsuarios,
    int UsuariosActivos,
    int TotalLibros,
    int LibrosDisponibles,
    int PrestamosActivos,
    int PrestamosVencidos,
    int PenalizacionesActivas);
