using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using SIGEBI.Application.Common.Exceptions;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Services;

public sealed class PenalizacionService : IPenalizacionService
{
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPrestamoRepository _prestamoRepository;

    public PenalizacionService(
        IPenalizacionRepository penalizacionRepository,
        IUsuarioRepository usuarioRepository,
        IPrestamoRepository prestamoRepository)
    {
        _penalizacionRepository = penalizacionRepository;
        _usuarioRepository = usuarioRepository;
        _prestamoRepository = prestamoRepository;
    }

    public async Task<IReadOnlyList<Penalizacion>> ObtenerActivasAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var penalizaciones = await _penalizacionRepository.ObtenerActivasPorUsuarioAsync(usuarioId, ct);
        if (penalizaciones.Count == 0)
        {
            return penalizaciones;
        }

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

        return penalizaciones.Where(p => p.Activa).ToList();
    }

    public async Task<Penalizacion> CrearAsync(
        Guid usuarioId,
        Guid? prestamoId,
        decimal monto,
        DateTime fechaInicioUtc,
        DateTime fechaFinUtc,
        string motivo,
        CancellationToken ct = default)
    {
        _ = await _usuarioRepository.GetByIdAsync(usuarioId, ct) ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        if (prestamoId.HasValue)
        {
            _ = await _prestamoRepository.GetByIdAsync(prestamoId.Value, ct)
                ?? throw new NotFoundException(nameof(Prestamo), prestamoId.Value);
        }

        try
        {
            var penalizacion = Penalizacion.Generar(usuarioId, prestamoId, monto, fechaInicioUtc, fechaFinUtc, motivo);
            await _penalizacionRepository.AddAsync(penalizacion, ct);
            return penalizacion;
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Penalizacion), ex.Message);
        }
    }

    public async Task<Penalizacion> CerrarAsync(Guid penalizacionId, string razon, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(razon))
        {
            throw ValidationError(nameof(razon), "Debe indicar la razón del cierre.");
        }

        var penalizacion = await _penalizacionRepository.GetByIdAsync(penalizacionId, ct)
                           ?? throw new NotFoundException(nameof(Penalizacion), penalizacionId);

        penalizacion.VerificarEstado(DateTime.UtcNow);

        if (!penalizacion.Activa)
        {
            await _penalizacionRepository.UpdateAsync(penalizacion, ct);
            throw ValidationError(nameof(Penalizacion), "La penalización ya se encuentra cerrada.");
        }

        try
        {
            penalizacion.CerrarAnticipadamente(razon.Trim());
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Penalizacion), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            throw ValidationError(nameof(Penalizacion), ex.Message);
        }

        await _penalizacionRepository.UpdateAsync(penalizacion, ct);
        return penalizacion;
    }

    private static ValidationException ValidationError(string property, string message)
        => new(new[] { new ValidationFailure(property, message) });
}
