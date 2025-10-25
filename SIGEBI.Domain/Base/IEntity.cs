using System;

namespace SIGEBI.Domain.Base
{
    // Conserva el nombre del archivo observado (IEntiy.cs). 
    // La interfaz expone únicamente el Id para entidades/agregados.
    public interface IEntity
    {
        Guid Id { get; }
    }
}