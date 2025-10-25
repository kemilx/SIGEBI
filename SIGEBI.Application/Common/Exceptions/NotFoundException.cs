using System;

namespace SIGEBI.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"No se encontró la entidad '{name}' con identificador '{key}'.")
    {
    }
}
