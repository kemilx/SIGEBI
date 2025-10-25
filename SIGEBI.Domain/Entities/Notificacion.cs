using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Notificacion : Entity
    {
        private const int MaxTituloLength = 150;
        private const int MaxMensajeLength = 1000;
        private const int MaxTipoLength = 50;

        private Notificacion() { }

        public Guid UsuarioId { get; private set; }
        public string Titulo { get; private set; } = null!;
        public string Mensaje { get; private set; } = null!;
        public string Tipo { get; private set; } = null!;
        public bool Leida { get; private set; }
        public DateTime? FechaLecturaUtc { get; private set; }

        public DateTime FechaCreacionUtc => CreatedAtUtc;

        public static Notificacion Crear(Guid usuarioId, string titulo, string mensaje, string tipo)
        {
            if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("Título requerido.", nameof(titulo));
            if (string.IsNullOrWhiteSpace(mensaje)) throw new ArgumentException("Mensaje requerido.", nameof(mensaje));
            if (string.IsNullOrWhiteSpace(tipo)) throw new ArgumentException("Tipo requerido.", nameof(tipo));

            var tituloLimpio = titulo.Trim();
            if (tituloLimpio.Length > MaxTituloLength)
                throw new ArgumentException($"El título no puede exceder {MaxTituloLength} caracteres.", nameof(titulo));

            var mensajeLimpio = mensaje.Trim();
            if (mensajeLimpio.Length > MaxMensajeLength)
                throw new ArgumentException($"El mensaje no puede exceder {MaxMensajeLength} caracteres.", nameof(mensaje));

            var tipoLimpio = tipo.Trim();
            if (tipoLimpio.Length > MaxTipoLength)
                throw new ArgumentException($"El tipo no puede exceder {MaxTipoLength} caracteres.", nameof(tipo));

            return new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = tituloLimpio,
                Mensaje = mensajeLimpio,
                Tipo = tipoLimpio,
                Leida = false
            };
        }

        public void MarcarComoLeida()
        {
            if (Leida) return;
            Leida = true;
            FechaLecturaUtc = DateTime.UtcNow;
            Touch();
        }
    }
}