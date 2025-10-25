using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PenalizacionController : ControllerBase
{
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPrestamoRepository _prestamoRepository;

    public PenalizacionController(
        IPenalizacionRepository penalizacionRepository,
        IUsuarioRepository usuarioRepository,
        IPrestamoRepository prestamoRepository)
    {
        _penalizacionRepository = penalizacionRepository;
        _usuarioRepository = usuarioRepository;
        _prestamoRepository = prestamoRepository;
    }

    [HttpGet("usuario/{usuarioId:guid}")]
    public async Task<ActionResult<IEnumerable<PenalizacionDto>>> ObtenerActivas(Guid usuarioId, CancellationToken ct)
    {
        var penalizaciones = await _penalizacionRepository.ObtenerActivasPorUsuarioAsync(usuarioId, ct);
        if (penalizaciones.Count == 0) return Ok(Array.Empty<PenalizacionDto>());

        var ahora = DateTime.UtcNow;
        var modificadas = new List<Penalizacion>();

        foreach (var penalizacion in penalizaciones)
        {
            var seguiaActiva = penalizacion.Activa;
            penalizacion.VerificarEstado(ahora);

            if (seguiaActiva && !penalizacion.Activa)
            {
                modificadas.Add(penalizacion);
            }
        }

        foreach (var penalizacion in modificadas)
        {
            await _penalizacionRepository.UpdateAsync(penalizacion, ct);
        }

        var activas = penalizaciones.Where(p => p.Activa).Select(Map).ToArray();
        return Ok(activas);
    }

    [HttpPost]
    public async Task<ActionResult<PenalizacionDto>> Crear([FromBody] CrearPenalizacionRequest request, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, ct);
        if (usuario is null) return NotFound(new { message = "El usuario indicado no existe." });

        try
        {
            if (request.PrestamoId.HasValue)
            {
                var prestamo = await _prestamoRepository.GetByIdAsync(request.PrestamoId.Value, ct);
                if (prestamo is null)
                {
                    return NotFound(new { message = "El préstamo indicado no existe." });
                }
            }

            var penalizacion = Penalizacion.Generar(
                request.UsuarioId,
                request.PrestamoId,
                request.Monto,
                request.FechaInicioUtc,
                request.FechaFinUtc,
                request.Motivo);

            await _penalizacionRepository.AddAsync(penalizacion, ct);
            return CreatedAtAction(nameof(ObtenerActivas), new { usuarioId = request.UsuarioId }, Map(penalizacion));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/cerrar")]
    public async Task<ActionResult<PenalizacionDto>> Cerrar(Guid id, [FromBody] CerrarPenalizacionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Razon))
        {
            return BadRequest(new { message = "Debe indicar la razón del cierre." });
        }

        var penalizacion = await _penalizacionRepository.GetByIdAsync(id, ct);
        if (penalizacion is null)
        {
            return NotFound();
        }

        penalizacion.VerificarEstado(DateTime.UtcNow);

        if (!penalizacion.Activa)
        {
            await _penalizacionRepository.UpdateAsync(penalizacion, ct);
            return BadRequest(new { message = "La penalización ya se encuentra cerrada." });
        }

        try
        {
            penalizacion.CerrarAnticipadamente(request.Razon.Trim());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        await _penalizacionRepository.UpdateAsync(penalizacion, ct);
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