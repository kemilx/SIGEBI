using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence
{
    public class SIGEBIDbContext : DbContext
    {
        public SIGEBIDbContext(DbContextOptions<SIGEBIDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Libro> Libros => Set<Libro>();
        public DbSet<Prestamo> Prestamos => Set<Prestamo>();
        public DbSet<Penalizacion> Penalizaciones => Set<Penalizacion>();
        public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
        public DbSet<Configuracion> Configuraciones => Set<Configuracion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RolConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.LibroConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PrestamoConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PenalizacionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.NotificacionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ConfiguracionConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}