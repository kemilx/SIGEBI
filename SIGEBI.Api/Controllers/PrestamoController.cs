using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Prestamos.Commands;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly IPrestamoService _prestamoService;

    public PrestamoController(IPrestamoService prestamoService)
    {
        _prestamoService = prestamoService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrestamoDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var prestamo = await _prestamoService.ObtenerPorIdAsync(id, ct);
        return Ok(Map(prestamo));
    }

    [HttpGet("usuario/{usuarioId:guid}")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerPorUsuario(Guid usuarioId, CancellationToken ct)
    {
        var prestamos = await _prestamoService.ObtenerPorUsuarioAsync(usuarioId, ct);
        return Ok(prestamos.Select(Map));
    }

    [HttpGet("libro/{libroId:guid}")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerActivosPorLibro(Guid libroId, CancellationToken ct)
    {
        var prestamos = await _prestamoService.ObtenerActivosPorLibroAsync(libroId, ct);
        return Ok(prestamos.Select(Map));
    }

    [HttpGet("vencidos")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerVencidos([FromQuery] DateTime? referenciaUtc, CancellationToken ct)
    {
        var prestamos = await _prestamoService.ObtenerVencidosAsync(referenciaUtc ?? DateTime.UtcNow, ct);
        return Ok(prestamos.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<PrestamoDto>> Crear([FromBody] CrearPrestamoRequest request, CancellationToken ct)
    {
        var prestamo = await _prestamoService.CrearAsync(
            new CrearPrestamoCommand(request.LibroId, request.UsuarioId, request.FechaInicioUtc, request.FechaFinUtc),
            ct);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = prestamo.Id }, Map(prestamo));
    }

    [HttpPost("{id:guid}/activar")]
    public async Task<ActionResult<PrestamoDto>> Activar(Guid id, CancellationToken ct)
    {
        var prestamo = await _prestamoService.ActivarAsync(new ActivarPrestamoCommand(id), ct);
        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/devolver")]
    public async Task<ActionResult<PrestamoDto>> RegistrarDevolucion(Guid id, [FromBody] RegistrarDevolucionRequest request, CancellationToken ct)
    {
        var prestamo = await _prestamoService.RegistrarDevolucionAsync(
            new RegistrarDevolucionCommand(id, request.FechaEntregaUtc, request.Observaciones),
            ct);

        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/cancelar")]
    public async Task<ActionResult<PrestamoDto>> Cancelar(Guid id, [FromBody] CancelarPrestamoRequest request, CancellationToken ct)
    {
        var prestamo = await _prestamoService.CancelarAsync(
            new CancelarPrestamoCommand(id, request.Motivo),
            ct);

        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/extender")]
    public async Task<ActionResult<PrestamoDto>> Extender(Guid id, [FromBody] ExtenderPrestamoRequest request, CancellationToken ct)
    {
        var prestamo = await _prestamoService.ExtenderAsync(new ExtenderPrestamoCommand(id, request.Dias), ct);
        return Ok(Map(prestamo));
    }

    private static PrestamoDto Map(Prestamo prestamo)
        => new(
            prestamo.Id,
            prestamo.LibroId,
            prestamo.UsuarioId,
            prestamo.Estado,
            prestamo.Periodo.FechaInicioUtc,
            prestamo.Periodo.FechaFinCompromisoUtc,
            prestamo.FechaEntregaRealUtc,
            prestamo.Observaciones,
            prestamo.CreatedAtUtc,
            prestamo.UpdatedAtUtc);
}
