using AguasGestionCR.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Services
{
    internal class ClienteServices //: IClientes
    {
        //public int RegistrarCliente(Clientes cliente)
        //{
        //    using (var db = newAcueductoDbContext())
        //    {
        //        var conn = db.Database.GetDbConnection();
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "RegistrarCliente";
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        }

        //    }


        //    }
        public Clientes ObtenerClientePorId(int id)
        {
            throw new NotImplementedException();
        }
        public bool EditarCliente(Clientes cliente)
        {
            throw new NotImplementedException();
        }
        public string EliminarCliente(int id)
        {
            throw new NotImplementedException();
        }
    }
}
