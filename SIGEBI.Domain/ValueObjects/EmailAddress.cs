using System;
using System.Text.RegularExpressions;

namespace SIGEBI.Domain.ValueObjects
{
    public sealed class EmailAddress
    {
        public const int MaxLength = 256;

        private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        public string Value { get; }

        private EmailAddress(string value)
        {
            Value = value;
        }

        public static EmailAddress Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El email no puede estar vacío.", nameof(value));

            value = value.Trim();

            if (value.Length > MaxLength)
                throw new ArgumentException($"El email no puede exceder {MaxLength} caracteres.", nameof(value));

            if (!Pattern.IsMatch(value))
                throw new ArgumentException("Formato de email inválido.", nameof(value));

            return new EmailAddress(value);
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
            => obj is EmailAddress other && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

        public static implicit operator string(EmailAddress email) => email.Value;
    }
}