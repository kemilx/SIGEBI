using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILibroRepository _libroRepository;

    public AdminController(
        IAdminRepository adminRepository,
        IUsuarioRepository usuarioRepository,
        ILibroRepository libroRepository)
    {
        _adminRepository = adminRepository;
        _usuarioRepository = usuarioRepository;
        _libroRepository = libroRepository;
    }

    [HttpGet("resumen")]
    public async Task<ActionResult<ReporteDto>> ObtenerResumen(CancellationToken ct)
    {
        var totalUsuarios = await _adminRepository.ContarUsuariosAsync(ct);
        var usuariosActivos = await _usuarioRepository.ContarActivosAsync(ct);
        var totalLibros = await _adminRepository.ContarLibrosAsync(ct);
        var librosDisponibles = await _libroRepository.ContarDisponiblesAsync(ct);
        var prestamosActivos = await _adminRepository.ContarPrestamosActivosAsync(ct);
        var prestamosVencidos = await _adminRepository.ContarPrestamosVencidosAsync(ct);
        var penalizacionesActivas = await _adminRepository.ContarPenalizacionesActivasAsync(ct);

        var dto = new ReporteDto(
            totalUsuarios,
            usuariosActivos,
            totalLibros,
            librosDisponibles,
            prestamosActivos,
            prestamosVencidos,
            penalizacionesActivas);

        return Ok(dto);
    }
}