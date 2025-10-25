using System;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Application.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Usuario> CrearAsync(string nombres, string apellidos, string email, TipoUsuario tipo, CancellationToken ct = default);
    Task<Usuario> ActualizarAsync(Guid id, string? nombres, string? apellidos, string? email, CancellationToken ct = default);
    Task<Usuario> DesactivarAsync(Guid id, CancellationToken ct = default);
    Task<Usuario> ReactivarAsync(Guid id, CancellationToken ct = default);
    Task<Usuario> AsignarRolAsync(Guid id, string nombreRol, string? descripcion, CancellationToken ct = default);
    Task<Usuario> RevocarRolAsync(Guid id, string nombreRol, CancellationToken ct = default);
}
