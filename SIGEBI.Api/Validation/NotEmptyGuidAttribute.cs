using System;
using System.ComponentModel.DataAnnotations;

namespace SIGEBI.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute() : base("El valor '{0}' no puede estar vacío.")
    {
    }

    public override bool IsValid(object? value)
        => value is Guid guid && guid != Guid.Empty;
}
