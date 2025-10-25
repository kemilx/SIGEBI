using FluentValidation;
using SIGEBI.Application.Prestamos.Commands;

namespace SIGEBI.Application.Prestamos.Validators;

public sealed class CancelarPrestamoCommandValidator : AbstractValidator<CancelarPrestamoCommand>
{
    public CancelarPrestamoCommandValidator()
    {
        RuleFor(c => c.PrestamoId)
            .NotEmpty().WithMessage("El identificador del préstamo es obligatorio.");

        RuleFor(c => c.Motivo)
            .NotEmpty().WithMessage("El motivo es obligatorio.")
            .MaximumLength(500)
            .WithMessage("El motivo no puede superar 500 caracteres.");
    }
}
