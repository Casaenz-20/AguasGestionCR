using AguasGestionCR.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Services
{
    public interface IUsuarioService
    {
        (bool Exito, string Mensaje) CrearUsuario(
            string nombreCompleto,
            string identificacion,
            string numeroMedidor,
            string correo,
            string nombreUsuario,
            string contrasenaPlana);

        Usuario? Autenticar(string nombreUsuario, string contrasenaPlana);
    }
}
