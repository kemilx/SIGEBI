using System;
using System.Collections.Generic;
using System.Linq;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Entities
{
    public sealed class Usuario : AggregateRoot
    {
        private readonly List<Rol> _roles = new();
        private readonly List<Guid> _prestamosIds = new();

        private Usuario() { }

        public NombreCompleto Nombre { get; private set; } = null!;
        public EmailAddress Email { get; private set; } = null!;
        public TipoUsuario Tipo { get; private set; }
        public bool Activo { get; private set; } = true;

        public IReadOnlyCollection<Rol> Roles => _roles.AsReadOnly();
        public IReadOnlyCollection<Guid> PrestamosIds => _prestamosIds.AsReadOnly();

        public static Usuario Create(NombreCompleto nombre, EmailAddress email, TipoUsuario tipo)
        {
            return new Usuario
            {
                Nombre = nombre,
                Email = email,
                Tipo = tipo
            };
        }

        public void CambiarEmail(EmailAddress nuevo)
        {
            if (nuevo.Value.Equals(Email.Value, StringComparison.OrdinalIgnoreCase)) return;
            Email = nuevo;
            Touch();
        }

        public void CambiarNombre(NombreCompleto nuevo)
        {
            Nombre = nuevo;
            Touch();
        }

        public void AsignarRol(Rol rol)
        {
            if (_roles.Any(r => r.Nombre.Equals(rol.Nombre, StringComparison.OrdinalIgnoreCase)))
                return;
            _roles.Add(rol);
            Touch();
        }

        public void RevocarRol(string nombreRol)
        {
            var existente = _roles.FirstOrDefault(r => r.Nombre.Equals(nombreRol, StringComparison.OrdinalIgnoreCase));
            if (existente is null) return;
            _roles.Remove(existente);
            Touch();
        }

        public void RegistrarPrestamo(Guid prestamoId)
        {
            if (!_prestamosIds.Contains(prestamoId))
            {
                _prestamosIds.Add(prestamoId);
                Touch();
            }
        }

        public void Desactivar()
        {
            if (!Activo) return;
            Activo = false;
            Touch();
        }

        public void Reactivar()
        {
            if (Activo) return;
            Activo = true;
            Touch();
        }
    }
}