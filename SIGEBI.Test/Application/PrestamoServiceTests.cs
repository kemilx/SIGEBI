using FluentValidation;
using FluentValidation.Results;
using Moq;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Prestamos.Commands;
using SIGEBI.Application.Prestamos.Services;
using SIGEBI.Application.Prestamos.Validators;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Test.Application;

public class PrestamoServiceTests
{
    private readonly Mock<ILibroRepository> _libroRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IPrestamoRepository> _prestamoRepository = new();
    private readonly Mock<IPenalizacionRepository> _penalizacionRepository = new();
    private readonly IPrestamoService _sut;

    public PrestamoServiceTests()
    {
        _sut = new PrestamoService(
            _libroRepository.Object,
            _usuarioRepository.Object,
            _prestamoRepository.Object,
            _penalizacionRepository.Object,
            new CrearPrestamoCommandValidator(),
            new ActivarPrestamoCommandValidator(),
            new RegistrarDevolucionCommandValidator(),
            new CancelarPrestamoCommandValidator(),
            new ExtenderPrestamoCommandValidator());
    }

    [Fact]
    public async Task CrearAsync_DebeFallar_SiLibroNoDisponible()
    {
        var libro = Libro.Create("El Quijote", "Cervantes", 1);
        libro.MarcarPrestado();

        var usuario = Usuario.Create(
            NombreCompleto.Create("Ana", "Pérez"),
            EmailAddress.Create("ana@example.com"),
            TipoUsuario.Estudiante);

        _libroRepository.Setup(r => r.GetByIdAsync(libro.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(libro);
        _usuarioRepository.Setup(r => r.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(usuario);
        _prestamoRepository.Setup(r => r.ExistePrestamoActivoOPendienteAsync(libro.Id, usuario.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

        var command = new CrearPrestamoCommand(libro.Id, usuario.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CrearAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CrearAsync_RegresaPrestamo_CuandoDatosValidos()
    {
        var libro = Libro.Create("Clean Code", "Robert C. Martin", 3);
        var usuario = Usuario.Create(
            NombreCompleto.Create("Luis", "Gómez"),
            EmailAddress.Create("luis@example.com"),
            TipoUsuario.Docente);

        _libroRepository.Setup(r => r.GetByIdAsync(libro.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(libro);
        _usuarioRepository.Setup(r => r.GetByIdAsync(usuario.Id, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(usuario);
        _prestamoRepository.Setup(r => r.ExistePrestamoActivoOPendienteAsync(libro.Id, usuario.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);
        _libroRepository.Setup(r => r.UpdateAsync(libro, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);
        _prestamoRepository.Setup(r => r.AddAsync(It.IsAny<Prestamo>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

        var command = new CrearPrestamoCommand(libro.Id, usuario.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        var prestamo = await _sut.CrearAsync(command, CancellationToken.None);

        Assert.Equal(libro.Id, prestamo.LibroId);
        Assert.Equal(usuario.Id, prestamo.UsuarioId);
        Assert.Equal(2, libro.EjemplaresDisponibles);
        _prestamoRepository.Verify(r => r.AddAsync(It.IsAny<Prestamo>(), It.IsAny<CancellationToken>()), Times.Once);
        _libroRepository.Verify(r => r.UpdateAsync(libro, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarDevolucionAsync_GeneraPenalizacion_CuandoHayRetraso()
    {
        var libro = Libro.Create("DDD", "Evans", 2);
        libro.MarcarPrestado();

        var periodo = PeriodoPrestamo.Create(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-2));
        var prestamo = Prestamo.Solicitar(libro.Id, Guid.NewGuid(), periodo);
        prestamo.Activar();

        _prestamoRepository.Setup(r => r.GetByIdAsync(prestamo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(prestamo);
        _libroRepository.Setup(r => r.GetByIdAsync(libro.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(libro);
        _prestamoRepository.Setup(r => r.UpdateAsync(prestamo, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);
        _libroRepository.Setup(r => r.UpdateAsync(libro, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        Penalizacion? penalizacionCreada = null;
        _penalizacionRepository.Setup(r => r.AddAsync(It.IsAny<Penalizacion>(), It.IsAny<CancellationToken>()))
                               .Callback<Penalizacion, CancellationToken>((p, _) => penalizacionCreada = p)
                               .Returns(Task.CompletedTask);

        var fechaEntrega = DateTime.UtcNow;
        var command = new RegistrarDevolucionCommand(prestamo.Id, fechaEntrega, null);

        await _sut.RegistrarDevolucionAsync(command, CancellationToken.None);

        Assert.NotNull(penalizacionCreada);
        Assert.Equal(prestamo.Id, penalizacionCreada!.PrestamoId);
        Assert.True(penalizacionCreada.Monto > 0);
        _penalizacionRepository.Verify(r => r.AddAsync(It.IsAny<Penalizacion>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelarAsync_RestauraDisponibles_CuandoPrestamoPendiente()
    {
        var libro = Libro.Create("Clean Architecture", "Robert C. Martin", 2);
        libro.ReservarEjemplar();

        var prestamo = Prestamo.Solicitar(libro.Id, Guid.NewGuid(),
            PeriodoPrestamo.Create(DateTime.UtcNow, DateTime.UtcNow.AddDays(3)));

        _prestamoRepository.Setup(r => r.GetByIdAsync(prestamo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(prestamo);
        _libroRepository.Setup(r => r.GetByIdAsync(libro.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(libro);
        _prestamoRepository.Setup(r => r.UpdateAsync(prestamo, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);
        _libroRepository.Setup(r => r.UpdateAsync(libro, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new CancelarPrestamoCommand(prestamo.Id, "Cancelado por el usuario");

        await _sut.CancelarAsync(command, CancellationToken.None);

        Assert.Equal(libro.EjemplaresTotales, libro.EjemplaresDisponibles);
        _libroRepository.Verify(r => r.UpdateAsync(libro, It.IsAny<CancellationToken>()), Times.Once);
        _prestamoRepository.Verify(r => r.UpdateAsync(prestamo, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelarAsync_RestauraDisponibles_CuandoPrestamoActivo()
    {
        var libro = Libro.Create("Refactoring", "Martin Fowler", 1);
        libro.ReservarEjemplar();
        libro.ConfirmarPrestamoReservado();

        var prestamo = Prestamo.Solicitar(libro.Id, Guid.NewGuid(),
            PeriodoPrestamo.Create(DateTime.UtcNow, DateTime.UtcNow.AddDays(4)));
        prestamo.Activar();

        _prestamoRepository.Setup(r => r.GetByIdAsync(prestamo.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(prestamo);
        _libroRepository.Setup(r => r.GetByIdAsync(libro.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(libro);
        _prestamoRepository.Setup(r => r.UpdateAsync(prestamo, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);
        _libroRepository.Setup(r => r.UpdateAsync(libro, It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        var command = new CancelarPrestamoCommand(prestamo.Id, "Cancelación administrativa");

        await _sut.CancelarAsync(command, CancellationToken.None);

        Assert.Equal(libro.EjemplaresTotales, libro.EjemplaresDisponibles);
        Assert.Equal(EstadoLibro.Disponible, libro.Estado);
        _libroRepository.Verify(r => r.UpdateAsync(libro, It.IsAny<CancellationToken>()), Times.Once);
        _prestamoRepository.Verify(r => r.UpdateAsync(prestamo, It.IsAny<CancellationToken>()), Times.Once);
    }
}
