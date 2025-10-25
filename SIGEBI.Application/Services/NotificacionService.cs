using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using SIGEBI.Application.Common.Exceptions;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Application.Services;

public sealed class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public NotificacionService(INotificacionRepository notificacionRepository, IUsuarioRepository usuarioRepository)
    {
        _notificacionRepository = notificacionRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Notificacion> CrearAsync(Guid usuarioId, string titulo, string mensaje, string tipo, CancellationToken ct = default)
    {
        _ = await _usuarioRepository.GetByIdAsync(usuarioId, ct) ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        try
        {
            var notificacion = Notificacion.Crear(usuarioId, titulo, mensaje, tipo);
            await _notificacionRepository.AddAsync(notificacion, ct);
            return notificacion;
        }
        catch (ArgumentException ex)
        {
            throw ValidationError(ex.ParamName ?? nameof(Notificacion), ex.Message);
        }
    }

    public async Task<IReadOnlyList<Notificacion>> ObtenerPendientesAsync(Guid usuarioId, CancellationToken ct = default)
        => await _notificacionRepository.ObtenerNoLeidasPorUsuarioAsync(usuarioId, ct);

    public async Task<int> ContarPendientesAsync(Guid usuarioId, CancellationToken ct = default)
        => await _notificacionRepository.ContarNoLeidasAsync(usuarioId, ct);

    public async Task<int> MarcarLeidasAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var pendientes = await _notificacionRepository.ObtenerNoLeidasPorUsuarioAsync(usuarioId, ct);
        if (pendientes.Count == 0)
        {
            return 0;
        }

        foreach (var notificacion in pendientes)
        {
            notificacion.MarcarComoLeida();
            await _notificacionRepository.UpdateAsync(notificacion, ct);
        }

        return pendientes.Count;
    }

    private static ValidationException ValidationError(string property, string message)
        => new(new[] { new ValidationFailure(property, message) });
}
