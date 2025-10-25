using System;

namespace SIGEBI.Domain.ValueObjects
{
    public sealed class NombreCompleto
    {
        public const int MaxNombresLength = 100;
        public const int MaxApellidosLength = 120;

        public string Nombres { get; }
        public string Apellidos { get; }

        private NombreCompleto(string nombres, string apellidos)
        {
            Nombres = nombres;
            Apellidos = apellidos;
        }

        public static NombreCompleto Create(string nombres, string apellidos)
        {
            if (string.IsNullOrWhiteSpace(nombres))
                throw new ArgumentException("Nombres requeridos.", nameof(nombres));
            if (string.IsNullOrWhiteSpace(apellidos))
                throw new ArgumentException("Apellidos requeridos.", nameof(apellidos));

            var nombresLimpios = nombres.Trim();
            var apellidosLimpios = apellidos.Trim();

            if (nombresLimpios.Length > MaxNombresLength)
                throw new ArgumentException($"Los nombres no pueden exceder {MaxNombresLength} caracteres.", nameof(nombres));
            if (apellidosLimpios.Length > MaxApellidosLength)
                throw new ArgumentException($"Los apellidos no pueden exceder {MaxApellidosLength} caracteres.", nameof(apellidos));

            return new NombreCompleto(nombresLimpios, apellidosLimpios);
        }

        public string Completo => $"{Nombres} {Apellidos}";

        public override string ToString() => Completo;

        public override bool Equals(object? obj)
        {
            if (obj is not NombreCompleto other) return false;
            return Nombres.Equals(other.Nombres, StringComparison.OrdinalIgnoreCase)
                   && Apellidos.Equals(other.Apellidos, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
            => HashCode.Combine(Nombres.ToLowerInvariant(), Apellidos.ToLowerInvariant());
    }
}