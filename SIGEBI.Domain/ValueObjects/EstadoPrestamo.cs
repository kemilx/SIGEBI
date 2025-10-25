namespace SIGEBI.Domain.ValueObjects
{
    public enum EstadoPrestamo
    {
        Pendiente = 1, // Solicitud registrada pero no entregada físicamente
        Activo = 2,    // Libro entregado al usuario
        Devuelto = 3,
        Vencido = 4,
        Cancelado = 5
    }
}