using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Api.Dtos;

public record UsuarioDto(
    Guid Id,
    string Nombres,
    string Apellidos,
    string NombreCompleto,
    string Email,
    TipoUsuario Tipo,
    bool Activo,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<Guid> PrestamosIds,
    DateTime CreadoUtc,
    DateTime? ActualizadoUtc);

public record CrearUsuarioRequest(
    string Nombres,
    string Apellidos,
    string Email,
    TipoUsuario Tipo);

public record ActualizarUsuarioRequest(
    string? Nombres,
    string? Apellidos,
    string? Email);

public record AsignarRolRequest(
    string Nombre,
    string? Descripcion);
