using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Prestamos.Commands;
using SIGEBI.Application.Prestamos.Services;
using SIGEBI.Application.Prestamos.Validators;
using SIGEBI.Application.Services;

namespace SIGEBI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSIGEBIApplication(this IServiceCollection services)
    {
        services.AddScoped<IPrestamoService, PrestamoService>();
        services.AddScoped<ILibroService, LibroService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<INotificacionService, NotificacionService>();
        services.AddScoped<IPenalizacionService, PenalizacionService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IAdminService, AdminService>();

        services.AddScoped<IValidator<CrearPrestamoCommand>, CrearPrestamoCommandValidator>();
        services.AddScoped<IValidator<ActivarPrestamoCommand>, ActivarPrestamoCommandValidator>();
        services.AddScoped<IValidator<RegistrarDevolucionCommand>, RegistrarDevolucionCommandValidator>();
        services.AddScoped<IValidator<CancelarPrestamoCommand>, CancelarPrestamoCommandValidator>();
        services.AddScoped<IValidator<ExtenderPrestamoCommand>, ExtenderPrestamoCommandValidator>();

        return services;
    }
}
