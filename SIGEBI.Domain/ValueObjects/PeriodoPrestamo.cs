using System;

namespace SIGEBI.Domain.ValueObjects
{
    public sealed class PeriodoPrestamo
    {
        public DateTime FechaInicioUtc { get; private set; }
        public DateTime FechaFinCompromisoUtc { get; private set; }

        // Constructor parameterless requerido por EF Core para materializar el owned type.
        private PeriodoPrestamo()
        {
        }

        private PeriodoPrestamo(DateTime fechaInicioUtc, DateTime fechaFinCompromisoUtc)
        {
            FechaInicioUtc = fechaInicioUtc;
            FechaFinCompromisoUtc = fechaFinCompromisoUtc;
        }

        public static PeriodoPrestamo Create(DateTime fechaInicioUtc, DateTime fechaFinCompromisoUtc)
        {
            if (fechaFinCompromisoUtc <= fechaInicioUtc)
                throw new ArgumentException("La fecha fin debe ser posterior a la fecha de inicio.");
            return new PeriodoPrestamo(fechaInicioUtc, fechaFinCompromisoUtc);
        }

        public bool EstaVencido(DateTime referenciaUtc) => referenciaUtc > FechaFinCompromisoUtc;

        public PeriodoPrestamo ExtenderDias(int dias)
        {
            if (dias <= 0) throw new ArgumentException("Los días de extensión deben ser positivos.", nameof(dias));
            return new PeriodoPrestamo(FechaInicioUtc, FechaFinCompromisoUtc.AddDays(dias));
        }

        public override bool Equals(object? obj)
            => obj is PeriodoPrestamo other &&
               FechaInicioUtc == other.FechaInicioUtc &&
               FechaFinCompromisoUtc == other.FechaFinCompromisoUtc;

        public override int GetHashCode() => HashCode.Combine(FechaInicioUtc, FechaFinCompromisoUtc);
    }
}