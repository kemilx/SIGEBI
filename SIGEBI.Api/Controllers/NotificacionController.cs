using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionController : ControllerBase
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public NotificacionController(INotificacionRepository notificacionRepository, IUsuarioRepository usuarioRepository)
    {
        _notificacionRepository = notificacionRepository;
        _usuarioRepository = usuarioRepository;
    }

    [HttpPost]
    public async Task<ActionResult<NotificacionDto>> Crear([FromBody] CrearNotificacionRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, ct);
        if (usuario is null) return NotFound(new { message = "El usuario indicado no existe." });

        try
        {
            var notificacion = Notificacion.Crear(request.UsuarioId, request.Titulo, request.Mensaje, request.Tipo);
            await _notificacionRepository.AddAsync(notificacion, ct);
            return CreatedAtAction(nameof(ObtenerPendientes), new { usuarioId = request.UsuarioId }, Map(notificacion));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("usuario/{usuarioId:guid}/pendientes")]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerPendientes(Guid usuarioId, CancellationToken ct)
    {
        var pendientes = await _notificacionRepository.ObtenerNoLeidasPorUsuarioAsync(usuarioId, ct);
        return Ok(pendientes.Select(Map));
    }

    [HttpPost("usuario/{usuarioId:guid}/marcar-leidas")]
    public async Task<IActionResult> MarcarLeidas(Guid usuarioId, CancellationToken ct)
    {
        var pendientes = await _notificacionRepository.ObtenerNoLeidasPorUsuarioAsync(usuarioId, ct);
        if (!pendientes.Any()) return NoContent();

        foreach (var notificacion in pendientes)
        {
            notificacion.MarcarComoLeida();
            await _notificacionRepository.UpdateAsync(notificacion, ct);
        }

        return Ok(new { message = "Notificaciones marcadas como leídas." });
    }

    [HttpGet("usuario/{usuarioId:guid}/contador")]
    public async Task<ActionResult<int>> ContarPendientes(Guid usuarioId, CancellationToken ct)
    {
        var total = await _notificacionRepository.ContarNoLeidasAsync(usuarioId, ct);
        return Ok(total);
    }

    private static NotificacionDto Map(Notificacion notificacion)
        => new(
            notificacion.Id,
            notificacion.UsuarioId,
            notificacion.Titulo,
            notificacion.Mensaje,
            notificacion.Tipo,
            notificacion.Leida,
            notificacion.CreatedAtUtc,
            notificacion.FechaLecturaUtc,
            notificacion.UpdatedAtUtc);
}