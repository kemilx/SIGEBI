using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Configurations
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Nombre)
                   .HasMaxLength(80)
                   .IsRequired();

            builder.Property(r => r.Descripcion)
                   .HasMaxLength(250);

            builder.Property(r => r.CreatedAtUtc).IsRequired();
            builder.Property(r => r.UpdatedAtUtc);

            builder.HasIndex(r => r.Nombre).IsUnique();
        }
    }
}