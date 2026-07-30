using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace AguasGestionCR.Helpers
{
    internal class DbHelper
    {
        public static DbParameter CrearParametro(DbCommand cmd, string nombre, object valor)
        {
            var parametro = cmd.CreateParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor ?? DBNull.Value;
            return parametro;
        }
    }
}
