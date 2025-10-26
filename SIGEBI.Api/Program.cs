using System.Threading.Tasks; 
using FluentValidation.AspNetCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SIGEBI.IOC;
using SIGEBI.Persistence;
using SIGEBI.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSIGEBIDependencies(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


await Program.EnsureDatabaseCreatedAsync(app.Services);

await app.RunAsync();

public partial class Program
{
    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SIGEBIDbContext>();

      
        if (!context.Database.IsRelational())
            return;

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
}
