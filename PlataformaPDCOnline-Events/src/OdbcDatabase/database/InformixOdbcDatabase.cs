using System;
using System.Data.Odbc;

namespace OdbcDatabase.database
{
    public class InformixOdbcDatabase
    {
        public OdbcConnection Connection { get; private set; }

        public InformixOdbcDatabase(string connectionString)
        {
            if (connectionString.Length == 0) throw new Exception("no es una cadena de conexion valida.");
            Connection = new OdbcConnection(connectionString);
        }
    }
}
