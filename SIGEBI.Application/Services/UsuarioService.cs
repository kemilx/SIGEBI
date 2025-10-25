using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using SIGEBI.Application.Common.Exceptions;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Application.Services;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository, IRolRepository rolRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
    }

    public async Task<Usuario> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => await _usuarioRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Usuario), id);

    public async Task<Usuario> CrearAsync(string nombres, string apellidos, string email, TipoUsuario tipo, CancellationToken ct = default)
    {
        try
        {
            var emailVo = EmailAddress.Create(email);
            if (await _usuarioRepository.EmailExisteAsync(emailVo.Value, ct))
            {
                throw new ConflictException("El correo electrónico ya está registrado.");
            }

            var usuario = Usuario.Create(NombreCompleto.Create(nombres, apellidos), emailVo, tipo);
            await _usuarioRepository.AddAsync(usuario, ct);
            return usuario;
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Usuario), ex.Message);
        }
    }

    public async Task<Usuario> ActualizarAsync(Guid id, string? nombres, string? apellidos, string? email, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Usuario), id);

        try
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailVo = EmailAddress.Create(email);
                if (!emailVo.Value.Equals(usuario.Email.Value, StringComparison.OrdinalIgnoreCase) &&
                    await _usuarioRepository.EmailExisteAsync(emailVo.Value, ct))
                {
                    throw new ConflictException("El correo electrónico ya está en uso.");
                }

                usuario.CambiarEmail(emailVo);
            }

            var nombresVacios = string.IsNullOrWhiteSpace(nombres);
            var apellidosVacios = string.IsNullOrWhiteSpace(apellidos);
            if (!nombresVacios || !apellidosVacios)
            {
                if (nombresVacios || apellidosVacios)
                {
                    throw ValidationError(nameof(nombres), "Debe indicar nombres y apellidos para actualizar el nombre completo.");
                }

                usuario.CambiarNombre(NombreCompleto.Create(nombres!, apellidos!));
            }
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Usuario), ex.Message);
        }

        await _usuarioRepository.UpdateAsync(usuario, ct);
        return usuario;
    }

    public async Task<Usuario> DesactivarAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Usuario), id);
        usuario.Desactivar();
        await _usuarioRepository.UpdateAsync(usuario, ct);
        return usuario;
    }

    public async Task<Usuario> ReactivarAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Usuario), id);
        usuario.Reactivar();
        await _usuarioRepository.UpdateAsync(usuario, ct);
        return usuario;
    }

    public async Task<Usuario> AsignarRolAsync(Guid id, string nombreRol, string? descripcion, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nombreRol))
        {
            throw ValidationError(nameof(nombreRol), "Debe indicar el nombre del rol a asignar.");
        }

        var usuario = await _usuarioRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Usuario), id);

        try
        {
            var rol = await _rolRepository.GetByNombreAsync(nombreRol.Trim(), ct);
            if (rol is null)
            {
                rol = Rol.Create(nombreRol, descripcion);
                await _rolRepository.AddAsync(rol, ct);
            }
            else if (!string.IsNullOrWhiteSpace(descripcion))
            {
                rol.ActualizarDescripcion(descripcion);
                await _rolRepository.UpdateAsync(rol, ct);
            }

            usuario.AsignarRol(rol);
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Rol), ex.Message);
        }

        await _usuarioRepository.UpdateAsync(usuario, ct);
        return usuario;
    }

    public async Task<Usuario> RevocarRolAsync(Guid id, string nombreRol, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nombreRol))
        {
            throw ValidationError(nameof(nombreRol), "Debe indicar el nombre del rol a revocar.");
        }

        var usuario = await _usuarioRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Usuario), id);
        usuario.RevocarRol(nombreRol.Trim());
        await _usuarioRepository.UpdateAsync(usuario, ct);
        return usuario;
    }

    private static ValidationException ValidationError(string property, string message)
        => new(new[] { new ValidationFailure(property, message) });
}
