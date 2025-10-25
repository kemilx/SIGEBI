using FluentValidation;
using SIGEBI.Application.Prestamos.Commands;

namespace SIGEBI.Application.Prestamos.Validators;

public sealed class RegistrarDevolucionCommandValidator : AbstractValidator<RegistrarDevolucionCommand>
{
    public RegistrarDevolucionCommandValidator()
    {
        RuleFor(c => c.PrestamoId)
            .NotEmpty().WithMessage("El identificador del préstamo es obligatorio.");

        RuleFor(c => c.FechaEntregaUtc)
            .NotEqual(default(DateTime))
            .WithMessage("La fecha de entrega es obligatoria.");

        RuleFor(c => c.Observaciones)
            .MaximumLength(500)
            .WithMessage("Las observaciones no pueden superar 500 caracteres.");
    }
}
