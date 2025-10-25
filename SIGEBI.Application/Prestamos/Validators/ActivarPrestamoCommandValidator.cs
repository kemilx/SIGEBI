using FluentValidation;
using SIGEBI.Application.Prestamos.Commands;

namespace SIGEBI.Application.Prestamos.Validators;

public sealed class ActivarPrestamoCommandValidator : AbstractValidator<ActivarPrestamoCommand>
{
    public ActivarPrestamoCommandValidator()
    {
        RuleFor(c => c.PrestamoId)
            .NotEmpty().WithMessage("El identificador del préstamo es obligatorio.");
    }
}
