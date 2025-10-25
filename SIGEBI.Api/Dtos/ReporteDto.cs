namespace SIGEBI.Api.Dtos;

public record ReporteDto(
    int TotalUsuarios,
    int UsuariosActivos,
    int TotalLibros,
    int LibrosDisponibles,
    int PrestamosActivos,
    int PrestamosVencidos,
    int PenalizacionesActivas);
