using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlataformaPDCOnline.Internals.excepciones;
using PlataformaPDCOnline.Internals.pdcOnline.Sender;
using PlataformaPDCOnline.Internals.plataforma;

namespace PlataformaPDCOnline
{
    public class Program
    {
        public static object GetApplicationConfiguration(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        //public static Boolean end = false;
        private static int TotalCommandsEnviados = 0;

        public static void Main(string[] args)
        {
            Sender.Singelton(); //iniciamos el sender.

            StartFunction();

            try
            {
                WebCommandsController.EndSender();
            }
            catch(NullReferenceException ne)
            {
                //Sender.Singelton().TrackException(ne);
                Console.WriteLine(ne.Message);
            }
            catch(Exception e)
            {
                //Sender.Singelton().TrackException(e);
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Total commands enviados: " + TotalCommandsEnviados);
            Sender.Singelton().GetServices().GetRequiredService<ILogger<Program>>().LogInformation("Total commands enviados: " + TotalCommandsEnviados);


            Task.Delay(10000).Wait(); //espera 10 segundos, por si acaso
        }

        //inicia el programa, cargando todos los commands que hay en la base de datos informix
        private static void StartFunction()
        {
            List<Dictionary<string, object>> webCommandsTable = ConsultasPreparadas.Singelton().GetWebCommands();

            if (webCommandsTable.Count > 0) PrepareDetector(webCommandsTable); //si hay commands con los que trabajar, trabajamos
        }

        /// <summary>
        /// Metodo que extrae un command de la base de datos y busca por cada tabla que cambios son de este command
        /// </summary>
        /// <param name="commandsTable">Recibe una lista de diccionarios (string, object) donde string es la columna y el object es el contenido de la fila  en la base de datos</param>
        private static void PrepareDetector(List<Dictionary<string, object>> commandsTable)
        {
            foreach (Dictionary<string, object> row in commandsTable)
            {
                try
                {
                    WebCommandsController controller = new WebCommandsController(row); //generamos un webController a partir de la informacion de este controller
                    TotalCommandsEnviados += controller.RunDetector(); //lanzamos el controller y recivimos el numero de commands enviados
                }
                catch (MyNoImplementedException ni)
                {
                    Sender.Singelton().GetServices().GetRequiredService<ILogger<Program>>().LogError(ni.Message);
                    Console.WriteLine(ni.Message);
                }
                catch(NoCompletCommandSend cs)
                {
                    Sender.Singelton().GetServices().GetRequiredService<ILogger<Program>>().LogError(cs.Message);
                    Console.WriteLine(cs.Message);
                }
                catch(Exception e)
                {
                    Sender.Singelton().GetServices().GetRequiredService<ILogger<Program>>().LogError(e.Message);
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
