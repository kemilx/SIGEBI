using FluentValidation;
using SIGEBI.Application.Prestamos.Commands;

namespace SIGEBI.Application.Prestamos.Validators;

public sealed class ExtenderPrestamoCommandValidator : AbstractValidator<ExtenderPrestamoCommand>
{
    public ExtenderPrestamoCommandValidator()
    {
        RuleFor(c => c.PrestamoId)
            .NotEmpty().WithMessage("El identificador del préstamo es obligatorio.");

        RuleFor(c => c.Dias)
            .GreaterThan(0)
            .WithMessage("Los días de extensión deben ser mayores a cero.");
    }
}
