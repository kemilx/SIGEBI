using System.ComponentModel.DataAnnotations;
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
    [property: Required, StringLength(100)] string Nombres,
    [property: Required, StringLength(120)] string Apellidos,
    [property: Required, EmailAddress, StringLength(256)] string Email,
    [property: Required] TipoUsuario Tipo);

public record ActualizarUsuarioRequest(
    [property: StringLength(100)] string? Nombres,
    [property: StringLength(120)] string? Apellidos,
    [property: EmailAddress, StringLength(256)] string? Email);

public record AsignarRolRequest(
    [property: Required, StringLength(80)] string Nombre,
    [property: StringLength(250)] string? Descripcion);
