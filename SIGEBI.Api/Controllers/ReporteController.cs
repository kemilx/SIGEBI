using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReporteController : ControllerBase
{
    private readonly ILibroRepository _libroRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ReporteController(
        ILibroRepository libroRepository,
        IPrestamoRepository prestamoRepository,
        IPenalizacionRepository penalizacionRepository,
        IUsuarioRepository usuarioRepository)
    {
        _libroRepository = libroRepository;
        _prestamoRepository = prestamoRepository;
        _penalizacionRepository = penalizacionRepository;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet("libros-por-estado")]
    public async Task<ActionResult<IDictionary<string, int>>> LibrosPorEstado(CancellationToken ct)
    {
        var result = new Dictionary<string, int>();
        foreach (var estado in Enum.GetValues<EstadoLibro>())
        {
            var cantidad = await _libroRepository.ContarPorEstadoAsync(estado, ct);
            result[estado.ToString()] = cantidad;
        }

        return Ok(result);
    }

    [HttpGet("prestamos-por-estado")]
    public async Task<ActionResult<IDictionary<string, int>>> PrestamosPorEstado(CancellationToken ct)
    {
        var result = new Dictionary<string, int>();
        foreach (var estado in Enum.GetValues<EstadoPrestamo>())
        {
            var cantidad = await _prestamoRepository.ContarPorEstadoAsync(estado, ct);
            result[estado.ToString()] = cantidad;
        }

        return Ok(result);
    }

    [HttpGet("penalizaciones-activas")]
    public async Task<ActionResult<int>> PenalizacionesActivas(CancellationToken ct)
    {
        var total = await _penalizacionRepository.ContarActivasAsync(ct);
        return Ok(total);
    }

    [HttpGet("usuarios-activos")]
    public async Task<ActionResult<int>> UsuariosActivos(CancellationToken ct)
    {
        var total = await _usuarioRepository.ContarActivosAsync(ct);
        return Ok(total);
    }
}