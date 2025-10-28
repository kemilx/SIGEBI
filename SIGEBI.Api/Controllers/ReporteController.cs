using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReporteController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReporteController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet("libros-por-estado")]
    public async Task<ActionResult<IReadOnlyDictionary<string, int>>> LibrosPorEstado(CancellationToken ct)
    {
        var result = await _reporteService.ObtenerLibrosPorEstadoAsync(ct);
        return Ok(result);
    }

    [HttpGet("prestamos-por-estado")]
    public async Task<ActionResult<IReadOnlyDictionary<string, int>>> PrestamosPorEstado(CancellationToken ct)
    {
        var result = await _reporteService.ObtenerPrestamosPorEstadoAsync(ct);
        return Ok(result);
    }

    [HttpGet("penalizaciones-activas")]
    public async Task<ActionResult<int>> PenalizacionesActivas(CancellationToken ct)
    {
        var total = await _reporteService.ContarPenalizacionesActivasAsync(ct);
        return Ok(total);
    }

    [HttpGet("usuarios-activos")]
    public async Task<ActionResult<int>> UsuariosActivos(CancellationToken ct)
    {
        var total = await _reporteService.ContarUsuariosActivosAsync(ct);
        return Ok(total);
    }
}
