using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using SIGEBI.Application.Common.Exceptions;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Prestamos.Commands;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Application.Prestamos.Services;

public sealed class PrestamoService : IPrestamoService
{
    private const decimal MontoPenalizacionPorDia = 5m;
    private const int DiasPenalizacion = 7;

    private readonly ILibroRepository _libroRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;
    private readonly IValidator<CrearPrestamoCommand> _crearValidator;
    private readonly IValidator<ActivarPrestamoCommand> _activarValidator;
    private readonly IValidator<RegistrarDevolucionCommand> _devolucionValidator;
    private readonly IValidator<CancelarPrestamoCommand> _cancelarValidator;
    private readonly IValidator<ExtenderPrestamoCommand> _extenderValidator;

    public PrestamoService(
        ILibroRepository libroRepository,
        IUsuarioRepository usuarioRepository,
        IPrestamoRepository prestamoRepository,
        IPenalizacionRepository penalizacionRepository,
        IValidator<CrearPrestamoCommand> crearValidator,
        IValidator<ActivarPrestamoCommand> activarValidator,
        IValidator<RegistrarDevolucionCommand> devolucionValidator,
        IValidator<CancelarPrestamoCommand> cancelarValidator,
        IValidator<ExtenderPrestamoCommand> extenderValidator)
    {
        _libroRepository = libroRepository;
        _usuarioRepository = usuarioRepository;
        _prestamoRepository = prestamoRepository;
        _penalizacionRepository = penalizacionRepository;
        _crearValidator = crearValidator;
        _activarValidator = activarValidator;
        _devolucionValidator = devolucionValidator;
        _cancelarValidator = cancelarValidator;
        _extenderValidator = extenderValidator;
    }

    public async Task<Prestamo> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => await _prestamoRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Prestamo), id);

    public async Task<IReadOnlyList<Prestamo>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken ct = default)
        => await _prestamoRepository.ObtenerPorUsuarioAsync(usuarioId, ct);

    public async Task<IReadOnlyList<Prestamo>> ObtenerActivosPorLibroAsync(Guid libroId, CancellationToken ct = default)
        => await _prestamoRepository.ObtenerActivosPorLibroAsync(libroId, ct);

    public async Task<IReadOnlyList<Prestamo>> ObtenerVencidosAsync(DateTime referenciaUtc, CancellationToken ct = default)
        => await _prestamoRepository.ObtenerVencidosAsync(referenciaUtc, ct);

    public async Task<Prestamo> CrearAsync(CrearPrestamoCommand command, CancellationToken ct = default)
    {
        await _crearValidator.ValidateAndThrowAsync(command, ct);

        var libro = await _libroRepository.GetByIdAsync(command.LibroId, ct)
                    ?? throw new NotFoundException(nameof(Libro), command.LibroId);

        var usuario = await _usuarioRepository.GetByIdAsync(command.UsuarioId, ct)
                      ?? throw new NotFoundException(nameof(Usuario), command.UsuarioId);

        if (!usuario.Activo)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(command.UsuarioId), "El usuario debe estar activo para solicitar préstamos.") });
        }

        var existePrestamoPendiente = await _prestamoRepository.ExistePrestamoActivoOPendienteAsync(command.LibroId, command.UsuarioId, ct);
        if (existePrestamoPendiente)
        {
            throw new ValidationException(new[] { new ValidationFailure("Prestamo", "El usuario ya tiene un préstamo pendiente o activo para este libro.") });
        }

        try
        {
            libro.ReservarEjemplar();
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(command.LibroId), ex.Message) });
        }

        var periodo = PeriodoPrestamo.Create(command.FechaInicioUtc, command.FechaFinUtc);
        var prestamo = Prestamo.Solicitar(command.LibroId, command.UsuarioId, periodo);

        await _libroRepository.UpdateAsync(libro, ct);
        await _prestamoRepository.AddAsync(prestamo, ct);
        return prestamo;
    }

    public async Task<Prestamo> ActivarAsync(ActivarPrestamoCommand command, CancellationToken ct = default)
    {
        await _activarValidator.ValidateAndThrowAsync(command, ct);

        var prestamo = await _prestamoRepository.GetByIdAsync(command.PrestamoId, ct)
                       ?? throw new NotFoundException(nameof(Prestamo), command.PrestamoId);

        var libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct)
                    ?? throw new NotFoundException(nameof(Libro), prestamo.LibroId);

        var usuario = await _usuarioRepository.GetByIdAsync(prestamo.UsuarioId, ct)
                      ?? throw new NotFoundException(nameof(Usuario), prestamo.UsuarioId);

        if (!usuario.Activo)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(prestamo.UsuarioId), "El usuario debe estar activo para activar el préstamo.") });
        }

        try
        {
            var estadoOriginal = libro.Estado;
            libro.ConfirmarPrestamoReservado();
            prestamo.Activar();
            usuario.RegistrarPrestamo(prestamo.Id);

            if (libro.Estado != estadoOriginal)
            {
                await _libroRepository.UpdateAsync(libro, ct);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(Prestamo.Estado), ex.Message) });
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        await _usuarioRepository.UpdateAsync(usuario, ct);

        return prestamo;
    }

    public async Task<Prestamo> RegistrarDevolucionAsync(RegistrarDevolucionCommand command, CancellationToken ct = default)
    {
        await _devolucionValidator.ValidateAndThrowAsync(command, ct);

        var prestamo = await _prestamoRepository.GetByIdAsync(command.PrestamoId, ct)
                       ?? throw new NotFoundException(nameof(Prestamo), command.PrestamoId);

        var libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct)
                    ?? throw new NotFoundException(nameof(Libro), prestamo.LibroId);

        try
        {
            prestamo.MarcarDevuelto(command.FechaEntregaUtc, command.Observaciones);
            libro.MarcarDevuelto();
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(ex.ParamName ?? nameof(Prestamo), ex.Message) });
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(Prestamo.Estado), ex.Message) });
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        await _libroRepository.UpdateAsync(libro, ct);

        await GenerarPenalizacionSiCorrespondeAsync(prestamo, command.FechaEntregaUtc, ct);

        return prestamo;
    }

    public async Task<Prestamo> CancelarAsync(CancelarPrestamoCommand command, CancellationToken ct = default)
    {
        await _cancelarValidator.ValidateAndThrowAsync(command, ct);

        var prestamo = await _prestamoRepository.GetByIdAsync(command.PrestamoId, ct)
                       ?? throw new NotFoundException(nameof(Prestamo), command.PrestamoId);

        var libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct)
                    ?? throw new NotFoundException(nameof(Libro), prestamo.LibroId);
        var estadoPrestamoAnterior = prestamo.Estado;
        var estadoLibroAnterior = libro.Estado;
        var ejemplaresDisponiblesAntes = libro.EjemplaresDisponibles;

        try
        {
            prestamo.Cancelar(command.Motivo);
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(ex.ParamName ?? nameof(Prestamo), ex.Message) });
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(Prestamo.Estado), ex.Message) });
        }

        if (estadoPrestamoAnterior is EstadoPrestamo.Pendiente or EstadoPrestamo.Activo)
        {
            try
            {
                libro.MarcarDevuelto();
            }
            catch (InvalidOperationException ex)
            {
                throw new ValidationException(new[] { new ValidationFailure(nameof(prestamo.LibroId), ex.Message) });
            }
        }

        if (estadoPrestamoAnterior is EstadoPrestamo.Pendiente or EstadoPrestamo.Activo)
        {
            if (libro.Estado != estadoLibroAnterior || libro.EjemplaresDisponibles != ejemplaresDisponiblesAntes)
            {
                await _libroRepository.UpdateAsync(libro, ct);
            }
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        return prestamo;
    }

    public async Task<Prestamo> ExtenderAsync(ExtenderPrestamoCommand command, CancellationToken ct = default)
    {
        await _extenderValidator.ValidateAndThrowAsync(command, ct);

        var prestamo = await _prestamoRepository.GetByIdAsync(command.PrestamoId, ct)
                       ?? throw new NotFoundException(nameof(Prestamo), command.PrestamoId);

        try
        {
            prestamo.Extender(command.Dias);
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(ex.ParamName ?? nameof(Prestamo), ex.Message) });
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(Prestamo.Estado), ex.Message) });
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        return prestamo;
    }

    private async Task GenerarPenalizacionSiCorrespondeAsync(Prestamo prestamo, DateTime fechaEntregaUtc, CancellationToken ct)
    {
        if (prestamo.Periodo.FechaFinCompromisoUtc >= fechaEntregaUtc)
        {
            return;
        }

        var diasRetraso = (int)Math.Ceiling((fechaEntregaUtc - prestamo.Periodo.FechaFinCompromisoUtc).TotalDays);
        if (diasRetraso <= 0)
        {
            return;
        }

        var monto = diasRetraso * MontoPenalizacionPorDia;
        var inicio = DateTime.UtcNow;
        var fin = inicio.AddDays(DiasPenalizacion);
        var motivo = $"Retraso de {diasRetraso} día(s) en la devolución del préstamo.";

        var penalizacion = Penalizacion.Generar(prestamo.UsuarioId, prestamo.Id, monto, inicio, fin, motivo);
        await _penalizacionRepository.AddAsync(penalizacion, ct);
    }
}
