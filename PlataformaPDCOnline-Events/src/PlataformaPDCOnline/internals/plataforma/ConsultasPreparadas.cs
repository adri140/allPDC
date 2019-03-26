using OdbcDatabase;
using OdbcDatabase.database;
using OdbcDatabase.excepciones;
using Pdc.Messaging;
using PlataformaPDCOnline.internals.exceptions;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.internals.plataforma
{
    public class ConsultasPreparadas
    {
        private static ConsultasPreparadas Consultas;

        private readonly InformixOdbcDao Infx;

        private ConsultasPreparadas()
        {
            Infx = new InformixOdbcDao();
        }

        public static ConsultasPreparadas Singelton()
        {
            if (Consultas == null) Consultas = new ConsultasPreparadas();

            return Consultas;
        }

        public List<Dictionary<string, object>> GetWebEvents()
        {
            string sql = "SELECT eventname, tablename, uidname FROM webevents WHERE active = ?";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "active", 1 }
            };

            Dictionary<string, OdbcType> types = new Dictionary<string, OdbcType>
            {
                { "active", OdbcType.Int }
            };

            OdbcCommand commandOdbc = new OdbcCommand(sql, Infx.Database.Connection);

            DatabaseTools.InsertParameters(parameters, types, commandOdbc);
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            try
            {
                Infx.Database.Connection.Open();
                result = Infx.ExecuteSelectCommand(commandOdbc);
                Infx.Database.Connection.Close();
            }
            catch (MyOdbcException e)
            {
                if (Infx.Database.Connection.State == System.Data.ConnectionState.Open) Infx.Database.Connection.Close();
                ErrorDBLog.Write(e.Message);
            }
            return result;
        }

        public async Task ExecuteCommitCommit(Event eventReceived)
        {
            WebEventsController controller = WebEventsController.Singelton(eventReceived.GetType().Name);

            int updateadas = 0;

            if (controller != null)
            {
                //datos de la select inicial
                string sqlS = "SELECT eventcommit, changevalue FROM " + controller.TableName + " WHERE " + controller.UidName + " = ?";

                Dictionary<string, object> whereDataS = new Dictionary<string, object>()
                {
                    { controller.UidName, eventReceived.AggregateId }
                };

                Dictionary<string, OdbcType> whereTypeS = new Dictionary<string, OdbcType>()
                {
                    { controller.UidName, OdbcType.VarChar }
                };

                OdbcCommand selectCommand = new OdbcCommand(sqlS);
                DatabaseTools.InsertParameters(whereDataS, whereTypeS, selectCommand);

                //datos de la update
                string sqlU = "UPDATE " + controller.TableName + " SET eventcommit = ?, changevalue = ? WHERE " + controller.UidName + " = ?";
                
                Dictionary<string, OdbcType> typeU = new Dictionary<string, OdbcType>()
                {
                    { "eventcommit", OdbcType.Int },
                    { "changevalue", OdbcType.Int },
                    { controller.UidName, OdbcType.VarChar }
                };

                OdbcCommand updateCommand = new OdbcCommand(sqlU);

                OdbcTransaction transaction = null;

                try
                {
                    Infx.Database.Connection.Open();
                    transaction = Infx.Database.Connection.BeginTransaction();

                    selectCommand.Transaction = transaction;

                    List<Dictionary<string, object>> result = Infx.ExecuteSelectCommandWithTransaction(selectCommand, transaction);

                    if (result.Count == 1)
                    {
                        Dictionary<string, object> dataU = new Dictionary<string, object>()
                        {
                            { "eventcommit", ((int) (result.ToArray()[0].GetValueOrDefault("eventcommit")) + 1) },
                            { "changevalue", ((int) (result.ToArray()[0].GetValueOrDefault("changevalue")) - 1) },
                            { controller.UidName, eventReceived.AggregateId }
                        };

                        DatabaseTools.InsertParameters(dataU, typeU, updateCommand);

                        updateCommand.Transaction = transaction;

                        updateadas = Infx.ExecuteUpdateCommandWithTransaction(updateCommand, transaction);

                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch (MyOdbcException e)
                {
                    if (transaction != null) transaction.Rollback();
                    ErrorDBLog.Write(e.Message);
                }
                finally
                {
                    if (Infx.Database.Connection.State == System.Data.ConnectionState.Open) Infx.Database.Connection.Close();
                }
            }
            else throw new MyNoImplementedException("No se ha encontrado ningun controlador para el evento " + eventReceived.GetType().Name);

            await Task.CompletedTask;
        }
    }
}
