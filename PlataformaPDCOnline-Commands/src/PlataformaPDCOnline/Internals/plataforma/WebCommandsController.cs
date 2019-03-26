using OdbcDatabase.excepciones;
using Pdc.Messaging;
using PlataformaPDCOnline.Internals.excepciones;
using PlataformaPDCOnline.Internals.pdcOnline.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Internals.plataforma
{
    public class WebCommandsController
    {
        public readonly string CommandName;
        public readonly List<string> CommandParameters;
        public readonly string TableName;
        public readonly string UidTableName;
        public readonly string SqlCommand;

        private int CommandsSended = 0;

        /// <summary>
        /// A partir de una fila de la tabla commands, genero este controller, mediante reflexion.
        /// </summary>
        /// <param name="controller">Diccionario (string, object) donde string es la columna y object es el dato en la base de datos</param>
        public WebCommandsController(Dictionary<string, object> controller)
        {
            this.TableName = controller.GetValueOrDefault("tablename").ToString();
            this.CommandName = controller.GetValueOrDefault("commandname").ToString();
            this.UidTableName = controller.GetValueOrDefault("uidtablename").ToString();
            this.SqlCommand = controller.GetValueOrDefault("sqlcommand").ToString();
            this.CommandParameters = new List<string>(controller.GetValueOrDefault("commandparameters").ToString().Split(",", StringSplitOptions.RemoveEmptyEntries));
        }

        //temporal
        public async static void EndSender()
        {
            try
            {
                await Sender.Singelton().EndJobAsync();
            }
            catch(NullReferenceException ne)
            {
                throw new Exception(ne.ToString());
            }
        }

        public override string ToString()
        {
            string result = "CommandName: " + this.CommandName + ", TableName: " + this.TableName + ", UidTableName: " + this.UidTableName + ", Parameters: [";
            if (this.CommandParameters != null)
            {
                foreach (string parameter in this.CommandParameters)
                {
                    result = result + parameter + ", ";
                }
            }
            return result + "], SqlCommand: " + this.SqlCommand;
        }

        //ejecuta el search para cada fila recuperada de la base de datos, antes de esto, debemos encontrar el search correspondiente para el command que toca, para eso usamos reflexion
        public int RunDetector()
        {
            try
            {
                Type searcherT = Type.GetType("PlataformaPDCOnline.Editable.Searchers.Search" + this.CommandName); //buscamos el tipo                                                                                         //Type commandT = Type.GetType("PlataformaPDCOnline.Editable.pdcOnline.Commands." + this.CommandName);

                if (searcherT.GetInterfaces().Contains(typeof(ISearcher))) //si la instancia implementa ISearcher y SearcherChangesController
                {
                    object search = searcherT == null ? throw new NullReferenceException("No se ha encontrado el typo.") : Activator.CreateInstance(searcherT); //creamos una instancia de esta clase

                    List<Dictionary<string, object>> table = ConsultasPreparadas.Singelton().GetRowData(this.SqlCommand);

                    MethodInfo method = search.GetType().GetMethod("RunSearcher");

                    foreach (Dictionary<string, object> row in table)
                    {
                        Command commandSend = (Command) method.Invoke(search, new object[] { row, this }); //invocamos el methodo con la instancia searcher y le pasamos los parametros
                        Task taskk = Sender.Singelton().SendCommand(commandSend);
                        taskk.Wait();
                        if (taskk.IsCompletedSuccessfully) CommandsSended++;
                        else throw new NoCompletCommandSend("No se a podido enviar el command.");
                    }
                }
                else throw new MyNoImplementedException("Se ha encontrado la clase " + searcherT.Name + ", pero no implementa ISearcher."); //ok
            }
            catch (NullReferenceException ne)
            {
                throw new Exception(ne.ToString());
            }
            return CommandsSended;
        }
    }
}
