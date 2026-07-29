using AguasGestionCR.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Interfaces
{
    interface IClientes
    {
        public int RegistrarCliente(Cliente cliente);
        public Cliente ObtenerClientePorId(int id);
        public bool EditarCliente(Cliente cliente);
        public Cliente EliminarCliente(int id);
    }
}
