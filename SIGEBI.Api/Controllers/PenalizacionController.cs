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
public class PenalizacionController : ControllerBase
{
    private readonly IPenalizacionService _penalizacionService;

    public PenalizacionController(IPenalizacionService penalizacionService)
    {
        _penalizacionService = penalizacionService;
    }

    [HttpGet("usuario/{usuarioId:guid}")]
    public async Task<ActionResult<IEnumerable<PenalizacionDto>>> ObtenerActivas(Guid usuarioId, CancellationToken ct)
    {
        var penalizaciones = await _penalizacionService.ObtenerActivasAsync(usuarioId, ct);
        return Ok(penalizaciones.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<PenalizacionDto>> Crear([FromBody] CrearPenalizacionRequest request, CancellationToken ct)
    {
        var penalizacion = await _penalizacionService.CrearAsync(
            request.UsuarioId,
            request.PrestamoId,
            request.Monto,
            request.FechaInicioUtc,
            request.FechaFinUtc,
            request.Motivo,
            ct);

        return CreatedAtAction(nameof(ObtenerActivas), new { usuarioId = request.UsuarioId }, Map(penalizacion));
    }

    [HttpPost("{id:guid}/cerrar")]
    public async Task<ActionResult<PenalizacionDto>> Cerrar(Guid id, [FromBody] CerrarPenalizacionRequest request, CancellationToken ct)
    {
        var penalizacion = await _penalizacionService.CerrarAsync(id, request.Razon, ct);
        return Ok(Map(penalizacion));
    }

    private static PenalizacionDto Map(Penalizacion penalizacion)
        => new(
            penalizacion.Id,
            penalizacion.UsuarioId,
            penalizacion.PrestamoId,
            penalizacion.Monto,
            penalizacion.FechaInicioUtc,
            penalizacion.FechaFinUtc,
            penalizacion.Motivo,
            penalizacion.Activa,
            penalizacion.CreatedAtUtc,
            penalizacion.UpdatedAtUtc);
}
