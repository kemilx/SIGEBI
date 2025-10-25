using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly SIGEBIDbContext _context;

    public AdminRepository(SIGEBIDbContext context)
    {
        _context = context;
    }

    public async Task<int> ContarUsuariosAsync(CancellationToken ct = default)
        => await _context.Usuarios.CountAsync(ct);

    public async Task<int> ContarLibrosAsync(CancellationToken ct = default)
        => await _context.Libros.CountAsync(ct);

    public async Task<int> ContarPrestamosActivosAsync(CancellationToken ct = default)
        => await _context.Prestamos.CountAsync(p => p.Estado == EstadoPrestamo.Activo, ct);

    public async Task<int> ContarPrestamosVencidosAsync(CancellationToken ct = default)
        => await _context.Prestamos.CountAsync(p => p.Estado == EstadoPrestamo.Vencido, ct);

    public async Task<int> ContarPenalizacionesActivasAsync(CancellationToken ct = default)
        => await _context.Penalizaciones.CountAsync(p => p.Activa, ct);
}
