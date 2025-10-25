using System;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Entities
{
    public sealed class Libro : AggregateRoot
    {
        private const int MaxTituloLength = 250;
        private const int MaxAutorLength = 200;
        private const int MaxIsbnLength = 40;
        private const int MaxUbicacionLength = 100;

        private Libro() { }

        public string Titulo { get; private set; } = null!;
        public string Autor { get; private set; } = null!;
        public string? Isbn { get; private set; }
        public string? Ubicacion { get; private set; }
        public int EjemplaresTotales { get; private set; }
        public int EjemplaresDisponibles { get; private set; }
        public EstadoLibro Estado { get; private set; } = EstadoLibro.Disponible;
        public DateTime? FechaPublicacion { get; private set; }

        public static Libro Create(string titulo, string autor, int ejemplares, string? isbn = null, string? ubicacion = null, DateTime? fechaPublicacion = null)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título es obligatorio.", nameof(titulo));
            if (string.IsNullOrWhiteSpace(autor))
                throw new ArgumentException("El autor es obligatorio.", nameof(autor));
            if (ejemplares <= 0)
                throw new ArgumentException("Debe haber al menos un ejemplar.", nameof(ejemplares));

            var tituloLimpio = titulo.Trim();
            if (tituloLimpio.Length > MaxTituloLength)
                throw new ArgumentException($"El título no puede exceder {MaxTituloLength} caracteres.", nameof(titulo));

            var autorLimpio = autor.Trim();
            if (autorLimpio.Length > MaxAutorLength)
                throw new ArgumentException($"El autor no puede exceder {MaxAutorLength} caracteres.", nameof(autor));

            string? isbnLimpio = null;
            if (!string.IsNullOrWhiteSpace(isbn))
            {
                isbnLimpio = isbn.Trim();
                if (isbnLimpio.Length > MaxIsbnLength)
                    throw new ArgumentException($"El ISBN no puede exceder {MaxIsbnLength} caracteres.", nameof(isbn));
            }

            string? ubicacionLimpia = null;
            if (!string.IsNullOrWhiteSpace(ubicacion))
            {
                ubicacionLimpia = ubicacion.Trim();
                if (ubicacionLimpia.Length > MaxUbicacionLength)
                    throw new ArgumentException($"La ubicación no puede exceder {MaxUbicacionLength} caracteres.", nameof(ubicacion));
            }

            return new Libro
            {
                Titulo = tituloLimpio,
                Autor = autorLimpio,
                Isbn = isbnLimpio,
                Ubicacion = ubicacionLimpia,
                EjemplaresTotales = ejemplares,
                EjemplaresDisponibles = ejemplares,
                FechaPublicacion = fechaPublicacion
            };
        }

        public bool DisponibleParaPrestamo() =>
            Estado == EstadoLibro.Disponible && EjemplaresDisponibles > 0;

        public void ReservarEjemplar()
        {
            if (!DisponibleParaPrestamo())
                throw new InvalidOperationException("El libro no está disponible para reserva.");

            EjemplaresDisponibles--;
            if (EjemplaresDisponibles == 0)
                Estado = EstadoLibro.Reservado;

            Touch();
        }

        public void MarcarPrestado()
        {
            if (EjemplaresDisponibles == 0)
            {
                if (Estado == EstadoLibro.Reservado)
                {
                    Estado = EstadoLibro.Prestado;
                    Touch();
                    return;
                }

                throw new InvalidOperationException("El libro no está disponible para préstamo.");
            }

            if (!DisponibleParaPrestamo())
                throw new InvalidOperationException("El libro no está disponible para préstamo.");

            EjemplaresDisponibles--;
            if (EjemplaresDisponibles == 0)
                Estado = EstadoLibro.Prestado;

            Touch();
        }

        public void ConfirmarPrestamoReservado()
        {
            if (Estado == EstadoLibro.Reservado && EjemplaresDisponibles == 0)
            {
                Estado = EstadoLibro.Prestado;
                Touch();
            }
        }

        public void MarcarDevuelto()
        {
            if (EjemplaresDisponibles >= EjemplaresTotales)
                throw new InvalidOperationException("No hay ejemplares prestados para devolver.");

            EjemplaresDisponibles++;
            if (Estado == EstadoLibro.Prestado || Estado == EstadoLibro.Reservado)
                Estado = EstadoLibro.Disponible;

            Touch();
        }

        public void MarcarReservado()
        {
            if (Estado != EstadoLibro.Disponible)
                throw new InvalidOperationException("Solo se puede reservar un libro disponible.");
            Estado = EstadoLibro.Reservado;
            Touch();
        }

        public void MarcarDañado()
        {
            Estado = EstadoLibro.Dañado;
            Touch();
        }

        public void MarcarInactivo()
        {
            Estado = EstadoLibro.Inactivo;
            Touch();
        }

        public void ActualizarUbicacion(string? nueva)
        {
            if (string.IsNullOrWhiteSpace(nueva))
            {
                Ubicacion = null;
            }
            else
            {
                var ubicacionLimpia = nueva.Trim();
                if (ubicacionLimpia.Length > MaxUbicacionLength)
                    throw new ArgumentException($"La ubicación no puede exceder {MaxUbicacionLength} caracteres.", nameof(nueva));

                Ubicacion = ubicacionLimpia;
            }

            Touch();
        }

        public void ActualizarDatos(string? titulo = null, string? autor = null, string? isbn = null, DateTime? fechaPublicacion = null)
        {
            if (!string.IsNullOrWhiteSpace(titulo))
            {
                var tituloLimpio = titulo.Trim();
                if (tituloLimpio.Length > MaxTituloLength)
                    throw new ArgumentException($"El título no puede exceder {MaxTituloLength} caracteres.", nameof(titulo));
                Titulo = tituloLimpio;
            }

            if (!string.IsNullOrWhiteSpace(autor))
            {
                var autorLimpio = autor.Trim();
                if (autorLimpio.Length > MaxAutorLength)
                    throw new ArgumentException($"El autor no puede exceder {MaxAutorLength} caracteres.", nameof(autor));
                Autor = autorLimpio;
            }

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                var isbnLimpio = isbn.Trim();
                if (isbnLimpio.Length > MaxIsbnLength)
                    throw new ArgumentException($"El ISBN no puede exceder {MaxIsbnLength} caracteres.", nameof(isbn));
                Isbn = isbnLimpio;
            }

            if (fechaPublicacion.HasValue)
            {
                FechaPublicacion = fechaPublicacion;
            }

            Touch();
        }
    }
}