using System.Linq;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using SIGEBI.Application.Common.Exceptions;

namespace SIGEBI.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ProblemDetailsFactory problemDetailsFactory,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _problemDetailsFactory = problemDetailsFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException notFoundException)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, notFoundException.Message);
        }
        catch (ConflictException conflictException)
        {
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, conflictException.Message);
        }
        catch (ValidationException validationException)
        {
            await HandleValidationExceptionAsync(context, validationException);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo un error inesperado procesando la solicitud");
            await WriteProblemAsync(context, (int)HttpStatusCode.InternalServerError, "Error interno del servidor.");
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new HttpValidationProblemDetails(errors)
        {
            Title = "La solicitud contiene datos inválidos.",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        await WriteProblemAsync(context, problem.Status!.Value, problem);
    }

    private async Task WriteProblemAsync(HttpContext context, int statusCode, string detail)
    {
        var problem = _problemDetailsFactory.CreateProblemDetails(
            context,
            statusCode: statusCode,
            detail: detail,
            instance: context.Request.Path);

        await WriteProblemAsync(context, statusCode, problem);
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, ProblemDetails problem)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
