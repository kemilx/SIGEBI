using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Configurations
{
    public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
    {
        public void Configure(EntityTypeBuilder<Notificacion> builder)
        {
            builder.ToTable("Notificaciones");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.UsuarioId).IsRequired();

            builder.Property(n => n.Titulo)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(n => n.Mensaje)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.Property(n => n.Tipo)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(n => n.Leida).IsRequired();

            builder.Property(n => n.FechaLecturaUtc)
                   .IsRequired(false);

            builder.Property(n => n.CreatedAtUtc).IsRequired();
            builder.Property(n => n.UpdatedAtUtc);

            builder.HasIndex(n => n.UsuarioId);
            builder.HasIndex(n => n.Leida);
            builder.HasIndex(n => n.Tipo);
        }
    }
}