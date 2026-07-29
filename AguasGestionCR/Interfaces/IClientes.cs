using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Interfaces
{
    interface IClientes
    {
        public int RegistrarCliente(Clientes cliente);
        public Clientes ObtenerClientePorId(int id);
        public bool EditarCliente(Clientes cliente);
        public string EliminarCliente(int id);
    }
}
