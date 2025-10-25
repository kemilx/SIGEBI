using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using SIGEBI.Api.Dtos;
using SIGEBI.Application.Common.Exceptions;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;
using SIGEBI.Persistence.Repositories;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly RolRepository _rolRepository;

    public UsuarioController(IUsuarioRepository usuarioRepository, RolRepository rolRepository)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> GetById(Guid id, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return NotFound();

        return Ok(Map(usuario));
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear([FromBody] CrearUsuarioRequest request, CancellationToken ct)
    {
        try
        {
            var emailVo = EmailAddress.Create(request.Email);
            var existente = await _usuarioRepository.GetByEmailAsync(emailVo.Value, ct);
            if (existente is not null)
            {
                throw new ConflictException("El correo electrónico ya está registrado.");
            }

            var usuario = Usuario.Create(
                NombreCompleto.Create(request.Nombres, request.Apellidos),
                emailVo,
                request.Tipo);

            await _usuarioRepository.AddAsync(usuario, ct);

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, Map(usuario));
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Usuario), ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(Guid id, [FromBody] ActualizarUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return NotFound();

        try
        {
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailVo = EmailAddress.Create(request.Email);
                if (!emailVo.Value.Equals(usuario.Email.Value, StringComparison.OrdinalIgnoreCase))
                {
                    var existente = await _usuarioRepository.GetByEmailAsync(emailVo.Value, ct);
                    if (existente is not null && existente.Id != usuario.Id)
                    {
                        throw new ConflictException("El correo electrónico ya está en uso.");
                    }

                    usuario.CambiarEmail(emailVo);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Nombres) && !string.IsNullOrWhiteSpace(request.Apellidos))
            {
                usuario.CambiarNombre(NombreCompleto.Create(request.Nombres, request.Apellidos));
            }
            else if (!string.IsNullOrWhiteSpace(request.Nombres) || !string.IsNullOrWhiteSpace(request.Apellidos))
            {
                throw ValidationError(nameof(request.Nombres), "Debe indicar nombres y apellidos para actualizar el nombre completo.");
            }

            await _usuarioRepository.UpdateAsync(usuario, ct);
            return Ok(Map(usuario));
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Usuario), ex.Message);
        }
    }

    [HttpPost("{id:guid}/desactivar")]
    public async Task<ActionResult<UsuarioDto>> Desactivar(Guid id, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return NotFound();

        usuario.Desactivar();
        await _usuarioRepository.UpdateAsync(usuario, ct);
        return Ok(Map(usuario));
    }

    [HttpPost("{id:guid}/reactivar")]
    public async Task<ActionResult<UsuarioDto>> Reactivar(Guid id, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return NotFound();

        usuario.Reactivar();
        await _usuarioRepository.UpdateAsync(usuario, ct);
        return Ok(Map(usuario));
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<ActionResult<UsuarioDto>> AsignarRol(Guid id, [FromBody] AsignarRolRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return NotFound();

        var rolNombre = request.Nombre.Trim();
        Rol? rol;

        try
        {
            rol = await _rolRepository.GetByNombreAsync(rolNombre, ct);
            if (rol is null)
            {
                rol = Rol.Create(rolNombre, request.Descripcion);
                await _rolRepository.AddAsync(rol, ct);
            }
            else if (!string.IsNullOrWhiteSpace(request.Descripcion))
            {
                rol.ActualizarDescripcion(request.Descripcion);
                await _rolRepository.UpdateAsync(rol, ct);
            }
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Rol), ex.Message);
        }

        usuario.AsignarRol(rol);
        await _usuarioRepository.UpdateAsync(usuario, ct);

        return Ok(Map(usuario));
    }

    [HttpDelete("{id:guid}/roles/{nombre}")]
    public async Task<ActionResult<UsuarioDto>> RevocarRol(Guid id, string nombre, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw ValidationError(nameof(nombre), "Debe indicar el nombre del rol a revocar.");
        }

        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario is null) return NotFound();

        usuario.RevocarRol(nombre.Trim());
        await _usuarioRepository.UpdateAsync(usuario, ct);

        return Ok(Map(usuario));
    }

    private static ValidationException ValidationError(string property, string message)
        => new(new[] { new ValidationFailure(property, message) });

    private static UsuarioDto Map(Usuario usuario)
        => new(
            usuario.Id,
            usuario.Nombre.Nombres,
            usuario.Nombre.Apellidos,
            usuario.Nombre.Completo,
            usuario.Email.Value,
            usuario.Tipo,
            usuario.Activo,
            usuario.Roles.Select(r => r.Nombre).ToArray(),
            usuario.PrestamosIds.ToArray(),
            usuario.CreatedAtUtc,
            usuario.UpdatedAtUtc);
}