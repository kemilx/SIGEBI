using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SIGEBI.IOC;
using SIGEBI.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSIGEBIPersistence(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SIGEBIDbContext>();

    if (!context.Database.IsRelational())
    {
        return;
    }

    try
    {
        await context.Database.EnsureCreatedAsync();
    }
    catch (SqlException ex)
    {
        throw new InvalidOperationException(
            "No se pudo crear o abrir la base de datos SIGEBI. Verifica la cadena de conexión y los permisos del usuario actual.",
            ex);
    }
}
