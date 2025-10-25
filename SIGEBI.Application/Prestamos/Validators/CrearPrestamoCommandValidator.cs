using FluentValidation;
using SIGEBI.Application.Prestamos.Commands;

namespace SIGEBI.Application.Prestamos.Validators;

public sealed class CrearPrestamoCommandValidator : AbstractValidator<CrearPrestamoCommand>
{
    public CrearPrestamoCommandValidator()
    {
        RuleFor(c => c.LibroId)
            .NotEmpty().WithMessage("El identificador del libro es obligatorio.");

        RuleFor(c => c.UsuarioId)
            .NotEmpty().WithMessage("El identificador del usuario es obligatorio.");

        RuleFor(c => c.FechaInicioUtc)
            .LessThan(c => c.FechaFinUtc)
            .WithMessage("La fecha de inicio debe ser anterior a la fecha de fin.");

        RuleFor(c => c.FechaInicioUtc)
            .NotEqual(default(DateTime))
            .WithMessage("La fecha de inicio es obligatoria.");

        RuleFor(c => c.FechaFinUtc)
            .NotEqual(default(DateTime))
            .WithMessage("La fecha de fin es obligatoria.");
    }
}
