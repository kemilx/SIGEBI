using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Configuracion : Entity
    {
        private const int MaxClaveLength = 120;
        private const int MaxValorLength = 500;
        private const int MaxDescripcionLength = 500;

        private Configuracion() { }

        public string Clave { get; private set; } = null!;
        public string Valor { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public bool Activo { get; private set; }

        public static Configuracion Crear(string clave, string valor, string? descripcion = null, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Clave requerida.", nameof(clave));
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("Valor requerido.", nameof(valor));

            var claveLimpia = clave.Trim();
            if (claveLimpia.Length > MaxClaveLength)
                throw new ArgumentException($"La clave no puede exceder {MaxClaveLength} caracteres.", nameof(clave));

            var valorLimpio = valor.Trim();
            if (valorLimpio.Length > MaxValorLength)
                throw new ArgumentException($"El valor no puede exceder {MaxValorLength} caracteres.", nameof(valor));

            string? descripcionLimpia = null;
            if (!string.IsNullOrWhiteSpace(descripcion))
            {
                descripcionLimpia = descripcion.Trim();
                if (descripcionLimpia.Length > MaxDescripcionLength)
                    throw new ArgumentException($"La descripción no puede exceder {MaxDescripcionLength} caracteres.", nameof(descripcion));
            }

            return new Configuracion
            {
                Clave = claveLimpia,
                Valor = valorLimpio,
                Descripcion = descripcionLimpia,
                Activo = activo
            };
        }

        public void ActualizarValor(string nuevoValor)
        {
            if (string.IsNullOrWhiteSpace(nuevoValor))
                throw new ArgumentException("Valor requerido.", nameof(nuevoValor));

            var valorLimpio = nuevoValor.Trim();
            if (valorLimpio.Length > MaxValorLength)
                throw new ArgumentException($"El valor no puede exceder {MaxValorLength} caracteres.", nameof(nuevoValor));

            Valor = valorLimpio;
            Touch();
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
                    throw new ArgumentException($"La descripción no puede exceder {MaxDescripcionLength} caracteres.", nameof(descripcion));

                Descripcion = descripcionLimpia;
            }
            Touch();
        }

        public void CambiarEstado(bool activo)
        {
            if (Activo == activo) return;
            Activo = activo;
            Touch();
        }
    }
}