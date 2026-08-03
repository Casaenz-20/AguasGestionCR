using AguasGestionCR.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Interfaces
{
    public interface IClientes
    {
        public int RegistrarCliente(Cliente cliente);
        public Cliente ObtenerClientePorId(int id);
        public bool EditarCliente(Cliente cliente);
        public string EliminarCliente(int id);
        // metodo para listar y filtrar clientes
        IEnumerable<Cliente> ObtenerClientes(string nombre = null, string identificacion = null, string medidor = null, string estado = "Todos");
    }
}
