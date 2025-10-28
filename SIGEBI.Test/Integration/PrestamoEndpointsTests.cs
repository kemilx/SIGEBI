using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Api;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.ValueObjects;
using SIGEBI.Persistence;

namespace SIGEBI.Test.Integration;

public class PrestamoEndpointsTests : IClassFixture<PrestamoApiFactory>
{
    private readonly HttpClient _client;
    private readonly PrestamoApiFactory _factory;

    public PrestamoEndpointsTests(PrestamoApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CrearPrestamo_RetornaProblemDetails_CuandoFechasInvalidas()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SIGEBIDbContext>();

        var libro = Libro.Create("Test", "Autor", 2);
        var usuario = Usuario.Create(
            NombreCompleto.Create("Carla", "Suarez"),
            EmailAddress.Create("carla@example.com"),
            TipoUsuario.Estudiante);

        context.Libros.Add(libro);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var request = new
        {
            LibroId = libro.Id,
            UsuarioId = usuario.Id,
            FechaInicioUtc = DateTime.UtcNow,
            FechaFinUtc = DateTime.UtcNow
        };

        var response = await _client.PostAsJsonAsync("/api/Prestamo", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(nameof(request.FechaFinUtc));
    }
}

public sealed class PrestamoApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SIGEBIDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<SIGEBIDbContext>(options =>
                options.UseInMemoryDatabase($"SIGEBI_Test_{Guid.NewGuid()}"));
        });
    }
}
