using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SIGEBIDbContext _context;

        public UsuarioRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Usuarios
                             .Include(u => u.Roles) // mejor que el string "_roles"
                             .FirstOrDefaultAsync(u => u.Id == id, ct);

        // Opción A (recomendada): comparar VO con VO (ya tienes HasConversion)
        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var emailVO = EmailAddress.Create(email);
            return await _context.Usuarios
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.Email == emailVO, ct);
        }

        public async Task<bool> EmailExisteAsync(string email, CancellationToken ct = default)
        {
            var emailVO = EmailAddress.Create(email);
            return await _context.Usuarios
                                 .AsNoTracking()
                                 .AnyAsync(u => u.Email == emailVO, ct);
        }

        public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
        {
            await _context.Usuarios.AddAsync(usuario, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<int> ContarActivosAsync(CancellationToken ct = default)
            => await _context.Usuarios.CountAsync(u => u.Activo, ct);
    }
}
