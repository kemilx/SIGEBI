using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Models;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILibroRepository _libroRepository;

    public AdminService(
        IAdminRepository adminRepository,
        IUsuarioRepository usuarioRepository,
        ILibroRepository libroRepository)
    {
        _adminRepository = adminRepository;
        _usuarioRepository = usuarioRepository;
        _libroRepository = libroRepository;
    }

    public async Task<AdminSummary> ObtenerResumenAsync(CancellationToken ct = default)
    {
        var totalUsuarios = await _adminRepository.ContarUsuariosAsync(ct);
        var usuariosActivos = await _usuarioRepository.ContarActivosAsync(ct);
        var totalLibros = await _adminRepository.ContarLibrosAsync(ct);
        var librosDisponibles = await _libroRepository.ContarDisponiblesAsync(ct);
        var prestamosActivos = await _adminRepository.ContarPrestamosActivosAsync(ct);
        var prestamosVencidos = await _adminRepository.ContarPrestamosVencidosAsync(ct);
        var penalizacionesActivas = await _adminRepository.ContarPenalizacionesActivasAsync(ct);

        return new AdminSummary(
            totalUsuarios,
            usuariosActivos,
            totalLibros,
            librosDisponibles,
            prestamosActivos,
            prestamosVencidos,
            penalizacionesActivas);
    }
}
