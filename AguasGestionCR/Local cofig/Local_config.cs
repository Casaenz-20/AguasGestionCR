using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Local_cofig
{
    public static class Local_config
    {
        public static string CadenaConexion => "Scaffold-DbContext \"Server=SEBASTIAN\\SQLEXPRESS;Database=AcueductoDB;Integrated Security=True;TrustServerCertificate=True;\" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models";
    }
}
