
using Microsoft.Data.SqlClient;

namespace SistemaInventario.Datos
{
    public class ConexionDB
    {
        private readonly string cadenaConexion =
            @"Server=(localdb)\MSSQLLocalDB;Database=InventarioDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}
