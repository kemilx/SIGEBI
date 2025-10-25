namespace SIGEBI.Domain.ValueObjects
{
    // Estados relevantes al ciclo de vida descrito (disponibilidad y control).
    public enum EstadoLibro
    {
        Disponible = 1,
        Prestado = 2,
        Reservado = 3,
        Dañado = 4,
        Inactivo = 5
    }
}