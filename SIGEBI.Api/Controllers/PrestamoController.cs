using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly ILibroRepository _libroRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PrestamoController(
        IPrestamoRepository prestamoRepository,
        ILibroRepository libroRepository,
        IUsuarioRepository usuarioRepository)
    {
        _prestamoRepository = prestamoRepository;
        _libroRepository = libroRepository;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrestamoDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        return Ok(Map(prestamo));
    }

    [HttpGet("usuario/{usuarioId:guid}")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerPorUsuario(Guid usuarioId, CancellationToken ct)
    {
        var prestamos = await _prestamoRepository.ObtenerPorUsuarioAsync(usuarioId, ct);
        return Ok(prestamos.Select(Map));
    }

    [HttpGet("libro/{libroId:guid}")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerActivosPorLibro(Guid libroId, CancellationToken ct)
    {
        var prestamos = await _prestamoRepository.ObtenerActivosPorLibroAsync(libroId, ct);
        return Ok(prestamos.Select(Map));
    }

    [HttpGet("vencidos")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerVencidos([FromQuery] DateTime? referenciaUtc, CancellationToken ct)
    {
        var referencia = referenciaUtc ?? DateTime.UtcNow;
        var vencidos = await _prestamoRepository.ObtenerVencidosAsync(referencia, ct);
        return Ok(vencidos.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<PrestamoDto>> Crear([FromBody] CrearPrestamoRequest request, CancellationToken ct)
    {
        var libro = await _libroRepository.GetByIdAsync(request.LibroId, ct);
        if (libro is null) return NotFound(new { message = "El libro indicado no existe." });

        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, ct);
        if (usuario is null) return NotFound(new { message = "El usuario indicado no existe." });

        try
        {
            var periodo = PeriodoPrestamo.Create(request.FechaInicioUtc, request.FechaFinUtc);
            var prestamo = Prestamo.Solicitar(request.LibroId, request.UsuarioId, periodo);

            await _prestamoRepository.AddAsync(prestamo, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = prestamo.Id }, Map(prestamo));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/activar")]
    public async Task<ActionResult<PrestamoDto>> Activar(Guid id, CancellationToken ct)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        var libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct);
        if (libro is null) return NotFound(new { message = "El libro asociado al préstamo no existe." });

        var usuario = await _usuarioRepository.GetByIdAsync(prestamo.UsuarioId, ct);
        if (usuario is null) return NotFound(new { message = "El usuario asociado al préstamo no existe." });

        try
        {
            libro.MarcarPrestado();
            prestamo.Activar();
            usuario.RegistrarPrestamo(prestamo.Id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        await _libroRepository.UpdateAsync(libro, ct);
        await _prestamoRepository.UpdateAsync(prestamo, ct);
        await _usuarioRepository.UpdateAsync(usuario, ct);

        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/devolver")]
    public async Task<ActionResult<PrestamoDto>> RegistrarDevolucion(Guid id, [FromBody] RegistrarDevolucionRequest request, CancellationToken ct)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        var libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct);
        if (libro is null) return NotFound(new { message = "El libro asociado al préstamo no existe." });

        try
        {
            prestamo.MarcarDevuelto(request.FechaEntregaUtc, request.Observaciones);
            libro.MarcarDevuelto();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        await _libroRepository.UpdateAsync(libro, ct);

        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/cancelar")]
    public async Task<ActionResult<PrestamoDto>> Cancelar(Guid id, [FromBody] CancelarPrestamoRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            return BadRequest(new { message = "Debe indicar un motivo de cancelación." });
        }

        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        try
        {
            prestamo.Cancelar(request.Motivo.Trim());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/extender")]
    public async Task<ActionResult<PrestamoDto>> Extender(Guid id, [FromBody] ExtenderPrestamoRequest request, CancellationToken ct)
    {
        if (request.Dias <= 0)
        {
            return BadRequest(new { message = "Los días de extensión deben ser mayores a cero." });
        }

        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        try
        {
            prestamo.Extender(request.Dias);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);
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