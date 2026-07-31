using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Local_cofig
{
    public static class Local_config
    {
        public static string CadenaConexion => "Server=localhost;" +
            "Database=AcueductoDB;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;" +
            "Encrypt=True;" +
            "MultipleActiveResultSets=True;";



    }
}
