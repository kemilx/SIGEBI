using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence;
using SIGEBI.Persistence.Repositories;

namespace SIGEBI.IOC;

public static class DependencyInjection
{
    public static IServiceCollection AddSIGEBIPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<SIGEBIDbContext>(options => options.UseInMemoryDatabase("SIGEBI"));
        }
        else
        {
            services.AddDbContext<SIGEBIDbContext>(options => options.UseSqlServer(connectionString));
        }

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ILibroRepository, LibroRepository>();
        services.AddScoped<IPrestamoRepository, PrestamoRepository>();
        services.AddScoped<INotificacionRepository, NotificacionRepository>();
        services.AddScoped<IPenalizacionRepository, PenalizacionRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<RolRepository>();

        return services;
    }
}
