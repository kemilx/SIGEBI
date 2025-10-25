using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Configurations
{
    public class ConfiguracionConfiguration : IEntityTypeConfiguration<Configuracion>
    {
        public void Configure(EntityTypeBuilder<Configuracion> builder)
        {
            builder.ToTable("Configuraciones");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Clave)
                   .HasMaxLength(120)
                   .IsRequired();

            builder.Property(c => c.Valor)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(c => c.Descripcion)
                   .HasMaxLength(500);

            builder.Property(c => c.Activo).IsRequired();

            builder.Property(c => c.CreatedAtUtc).IsRequired();
            builder.Property(c => c.UpdatedAtUtc);

            builder.HasIndex(c => c.Clave).IsUnique();
            builder.HasIndex(c => c.Activo);
        }
    }
}