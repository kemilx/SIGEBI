using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence.Configurations
{
    public class LibroConfiguration : IEntityTypeConfiguration<Libro>
    {
        public void Configure(EntityTypeBuilder<Libro> builder)
        {
            builder.ToTable("Libros");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Titulo)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(l => l.Autor)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(l => l.Isbn)
                   .HasMaxLength(40);

            builder.Property(l => l.Ubicacion)
                   .HasMaxLength(100);

            builder.Property(l => l.EjemplaresTotales).IsRequired();
            builder.Property(l => l.EjemplaresDisponibles).IsRequired();

            builder.Property(l => l.Estado)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(l => l.FechaPublicacion);

            builder.Property(l => l.CreatedAtUtc).IsRequired();
            builder.Property(l => l.UpdatedAtUtc);

            builder.HasIndex(l => l.Titulo);
            builder.HasIndex(l => l.Autor);
            builder.HasIndex(l => l.Estado);
            builder.HasIndex(l => l.Isbn)
                   .IsUnique()
                   .HasFilter("[Isbn] IS NOT NULL");
        }
    }
}