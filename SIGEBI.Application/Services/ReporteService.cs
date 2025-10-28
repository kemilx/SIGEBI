using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Application.Services;

public sealed class ReporteService : IReporteService
{
    private readonly ILibroRepository _libroRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ReporteService(
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

    public async Task<IReadOnlyDictionary<string, int>> ObtenerLibrosPorEstadoAsync(CancellationToken ct = default)
    {
        var result = new Dictionary<string, int>();
        foreach (var estado in Enum.GetValues<EstadoLibro>())
        {
            var cantidad = await _libroRepository.ContarPorEstadoAsync(estado, ct);
            result[estado.ToString()] = cantidad;
        }

        return result;
    }

    public async Task<IReadOnlyDictionary<string, int>> ObtenerPrestamosPorEstadoAsync(CancellationToken ct = default)
    {
        var result = new Dictionary<string, int>();
        foreach (var estado in Enum.GetValues<EstadoPrestamo>())
        {
            var cantidad = await _prestamoRepository.ContarPorEstadoAsync(estado, ct);
            result[estado.ToString()] = cantidad;
        }

        return result;
    }

    public async Task<int> ContarPenalizacionesActivasAsync(CancellationToken ct = default)
        => await _penalizacionRepository.ContarActivasAsync(ct);

    public async Task<int> ContarUsuariosActivosAsync(CancellationToken ct = default)
        => await _usuarioRepository.ContarActivosAsync(ct);
}
