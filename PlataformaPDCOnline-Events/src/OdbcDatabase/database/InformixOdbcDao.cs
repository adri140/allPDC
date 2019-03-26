using OdbcDatabase.excepciones;
using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace OdbcDatabase.database
{
    public class InformixOdbcDao
    {
        public InformixOdbcDatabase Database { get; private set; }

        public InformixOdbcDao()
        {
            try
            {
                if (Database == null) { Database = new InformixOdbcDatabase(DatabaseTools.GetConnectionString("[OdbcInformixServer]")); }
            }
            catch(Exception e)
            {
                ErrorDBLog.Write(e.Message);
                Database = null;
            }
        }

        public List<Dictionary<string, object>> ExecuteSelectCommand(OdbcCommand command)
        {
            List<Dictionary<string, object>> tablaResult = new List<Dictionary<string, object>>();
            try
            { 
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Dictionary<string, object> rowResult = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rowResult.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    tablaResult.Add(rowResult);
                }

            }
            catch (MyOdbcException e)
            {
                throw new MyOdbcException("Error Informix OdbcDao: " + e.ToString());
            }
            return tablaResult;
        }

        public List<Dictionary<string, object>> ExecuteSelectCommandWithTransaction(OdbcCommand command, OdbcTransaction transaction)
        {
            List<Dictionary<string, object>> tablaResult = new List<Dictionary<string, object>>();
            try
            {
                if (command.Transaction == null) command.Transaction = transaction;

                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Dictionary<string, object> rowResult = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rowResult.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    tablaResult.Add(rowResult);
                }

            }
            catch (MyOdbcException e)
            {
                throw new MyOdbcException("Error Informix OdbcDao: " + e.ToString());
            }
            return tablaResult;
        }

        public int ExecuteUpdateCommand(OdbcCommand command)
        {
            int updates = 0;
            try
            {
                updates = command.ExecuteNonQuery();
            }
            catch (MyOdbcException e)
            {
                throw new MyOdbcException("Error Informix OdbcDao: " + e.ToString());
            }
            return updates;
        }

        public int ExecuteUpdateCommandWithTransaction(OdbcCommand command, OdbcTransaction transaction)
        {
            int updates = 0;
            try
            {
                if (command.Transaction == null) command.Transaction = transaction;

                updates = command.ExecuteNonQuery();
            }
            catch (MyOdbcException e)
            {
                throw new MyOdbcException("Error Informix OdbcDao: " + e.ToString());
            }
            return updates;
        }
    }
}
