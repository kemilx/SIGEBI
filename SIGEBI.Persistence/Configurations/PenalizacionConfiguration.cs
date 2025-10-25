using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Configurations
{
    public class PenalizacionConfiguration : IEntityTypeConfiguration<Penalizacion>
    {
        public void Configure(EntityTypeBuilder<Penalizacion> builder)
        {
            builder.ToTable("Penalizaciones");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UsuarioId).IsRequired();
            builder.Property(p => p.PrestamoId);

            builder.Property(p => p.Monto)
                   .HasColumnType("decimal(12,2)")
                   .IsRequired();

            builder.Property(p => p.FechaInicioUtc).IsRequired();
            builder.Property(p => p.FechaFinUtc).IsRequired();

            builder.Property(p => p.Motivo)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(p => p.Activa).IsRequired();

            builder.Property(p => p.CreatedAtUtc).IsRequired();
            builder.Property(p => p.UpdatedAtUtc);

            builder.HasIndex(p => p.UsuarioId);
            builder.HasIndex(p => p.Activa);
        }
    }
}