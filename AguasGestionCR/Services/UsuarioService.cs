using AguasGestionCR.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioService(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public (bool Exito, string Mensaje) CrearUsuario(
            string nombreCompleto,
            string identificacion,
            string numeroMedidor,
            string correo,
            string nombreUsuario,
            string contrasenaPlana)
        {
            using (var db = new AcueductoDbContext())
            {
                // Verificar que la identificación y el medidor coincidan en la tabla Clientes
                bool clienteValido = db.Clientes.Any(c =>
                    c.Identificacion == identificacion &&
                    c.NumeroMedidor == numeroMedidor);

                if (!clienteValido)
                {
                    return (false, "El número de medidor no está asociado a la identificación en el registro de clientes.");
                }

                // Verificar que el nombre de usuario no esté repetido
                bool usuarioExiste = db.Usuarios.Any(u => u.NombreUsuario == nombreUsuario);
                if (usuarioExiste)
                {
                    return (false, "El nombre de usuario ya se encuentra registrado.");
                }

                // Generar hash BCrypt
                string hashClave = _passwordHasher.HashPassword(contrasenaPlana);

                var nuevoUsuario = new Usuario
                {
                    NombreCompleto = nombreCompleto,
                    Identificacion = identificacion,
                    NumeroMedidor = numeroMedidor,
                    CorreoElectronico = correo,
                    NombreUsuario = nombreUsuario,
                    ContrasenaHash = hashClave,
                    Rol = "Cliente",
                    Estado = true,
                    FechaCreacion = DateTime.Now
                };

                db.Usuarios.Add(nuevoUsuario);
                db.SaveChanges();

                return (true, "Usuario registrado exitosamente.");
            }
        }
        public Usuario? Autenticar(string nombreUsuario, string contrasenaPlana)
        {
            using (var db = new AcueductoDbContext())
            {
                var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Estado);

                if (usuario != null && _passwordHasher.VerifyPassword(contrasenaPlana, usuario.ContrasenaHash))
                {
                    return usuario;
                }

                return null;
            }
        }
    }
}
