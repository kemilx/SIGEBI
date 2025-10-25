using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence.Configurations
{
    public class PrestamoConfiguration : IEntityTypeConfiguration<Prestamo>
    {
        public void Configure(EntityTypeBuilder<Prestamo> builder)
        {
            builder.ToTable("Prestamos");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.LibroId).IsRequired();
            builder.Property(p => p.UsuarioId).IsRequired();

            builder.Property(p => p.Estado)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(p => p.Observaciones)
                   .HasMaxLength(500);

            builder.Property(p => p.FechaEntregaRealUtc);

            builder.Property(p => p.CreatedAtUtc).IsRequired();
            builder.Property(p => p.UpdatedAtUtc);

            // Owned Value Object: PeriodoPrestamo
            builder.OwnsOne(p => p.Periodo, pb =>
            {
                pb.Property(x => x.FechaInicioUtc)
                  .HasColumnName("FechaInicioUtc")
                  .IsRequired();

                pb.Property(x => x.FechaFinCompromisoUtc)
                  .HasColumnName("FechaFinCompromisoUtc")
                  .IsRequired();
            });

            builder.HasIndex(p => p.UsuarioId);
            builder.HasIndex(p => p.LibroId);
            builder.HasIndex(p => p.Estado);
        }
    }
}