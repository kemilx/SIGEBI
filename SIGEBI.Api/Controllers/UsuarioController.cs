using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> GetById(Guid id, CancellationToken ct)
    {
        var usuario = await _usuarioService.ObtenerPorIdAsync(id, ct);
        return Ok(Map(usuario));
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear([FromBody] CrearUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioService.CrearAsync(request.Nombres, request.Apellidos, request.Email, request.Tipo, ct);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, Map(usuario));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(Guid id, [FromBody] ActualizarUsuarioRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioService.ActualizarAsync(id, request.Nombres, request.Apellidos, request.Email, ct);
        return Ok(Map(usuario));
    }

    [HttpPost("{id:guid}/desactivar")]
    public async Task<ActionResult<UsuarioDto>> Desactivar(Guid id, CancellationToken ct)
    {
        var usuario = await _usuarioService.DesactivarAsync(id, ct);
        return Ok(Map(usuario));
    }

    [HttpPost("{id:guid}/reactivar")]
    public async Task<ActionResult<UsuarioDto>> Reactivar(Guid id, CancellationToken ct)
    {
        var usuario = await _usuarioService.ReactivarAsync(id, ct);
        return Ok(Map(usuario));
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<ActionResult<UsuarioDto>> AsignarRol(Guid id, [FromBody] AsignarRolRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioService.AsignarRolAsync(id, request.Nombre, request.Descripcion, ct);
        return Ok(Map(usuario));
    }

    [HttpDelete("{id:guid}/roles/{nombre}")]
    public async Task<ActionResult<UsuarioDto>> RevocarRol(Guid id, string nombre, CancellationToken ct)
    {
        var usuario = await _usuarioService.RevocarRolAsync(id, nombre, ct);
        return Ok(Map(usuario));
    }

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
