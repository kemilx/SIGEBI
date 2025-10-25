using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Application.Interfaces;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("resumen")]
    public async Task<ActionResult<ReporteDto>> ObtenerResumen(CancellationToken ct)
    {
        var resumen = await _adminService.ObtenerResumenAsync(ct);
        var dto = new ReporteDto(
            resumen.TotalUsuarios,
            resumen.UsuariosActivos,
            resumen.TotalLibros,
            resumen.LibrosDisponibles,
            resumen.PrestamosActivos,
            resumen.PrestamosVencidos,
            resumen.PenalizacionesActivas);

        return Ok(dto);
    }
}
