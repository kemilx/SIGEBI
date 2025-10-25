using System;
using System.Collections.Generic;
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
public class NotificacionController : ControllerBase
{
    private readonly INotificacionService _notificacionService;

    public NotificacionController(INotificacionService notificacionService)
    {
        _notificacionService = notificacionService;
    }

    [HttpPost]
    public async Task<ActionResult<NotificacionDto>> Crear([FromBody] CrearNotificacionRequest request, CancellationToken ct)
    {
        var notificacion = await _notificacionService.CrearAsync(request.UsuarioId, request.Titulo, request.Mensaje, request.Tipo, ct);
        return CreatedAtAction(nameof(ObtenerPendientes), new { usuarioId = request.UsuarioId }, Map(notificacion));
    }

    [HttpGet("usuario/{usuarioId:guid}/pendientes")]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerPendientes(Guid usuarioId, CancellationToken ct)
    {
        var pendientes = await _notificacionService.ObtenerPendientesAsync(usuarioId, ct);
        return Ok(pendientes.Select(Map));
    }

    [HttpPost("usuario/{usuarioId:guid}/marcar-leidas")]
    public async Task<IActionResult> MarcarLeidas(Guid usuarioId, CancellationToken ct)
    {
        var totalMarcadas = await _notificacionService.MarcarLeidasAsync(usuarioId, ct);
        if (totalMarcadas == 0) return NoContent();

        return Ok(new { total = totalMarcadas });
    }

    [HttpGet("usuario/{usuarioId:guid}/contador")]
    public async Task<ActionResult<int>> ContarPendientes(Guid usuarioId, CancellationToken ct)
    {
        var total = await _notificacionService.ContarPendientesAsync(usuarioId, ct);
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
