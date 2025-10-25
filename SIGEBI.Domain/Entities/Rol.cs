using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Rol : Entity
    {
        private const int MaxNombreLength = 80;
        private const int MaxDescripcionLength = 250;

        private Rol() { }

        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }

        public static Rol Create(string nombre, string? descripcion = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del rol es obligatorio.", nameof(nombre));

            var nombreLimpio = nombre.Trim();
            if (nombreLimpio.Length > MaxNombreLength)
                throw new ArgumentException($"El nombre del rol no puede exceder {MaxNombreLength} caracteres.", nameof(nombre));

            string? descripcionLimpia = null;
            if (!string.IsNullOrWhiteSpace(descripcion))
            {
                descripcionLimpia = descripcion.Trim();
                if (descripcionLimpia.Length > MaxDescripcionLength)
                    throw new ArgumentException($"La descripción del rol no puede exceder {MaxDescripcionLength} caracteres.", nameof(descripcion));
            }

            return new Rol
            {
                Nombre = nombreLimpio,
                Descripcion = descripcionLimpia
            };
        }

        public void ActualizarDescripcion(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                Descripcion = null;
            }
            else
            {
                var descripcionLimpia = descripcion.Trim();
                if (descripcionLimpia.Length > MaxDescripcionLength)
                    throw new ArgumentException($"La descripción del rol no puede exceder {MaxDescripcionLength} caracteres.", nameof(descripcion));

                Descripcion = descripcionLimpia;
            }
            Touch();
        }

        public override string ToString() => Nombre;
    }
}