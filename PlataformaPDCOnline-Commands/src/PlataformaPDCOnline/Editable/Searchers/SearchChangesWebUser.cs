using Pdc.Messaging;
using System;
using System.Collections.Generic;
using PlataformaPDCOnline.Internals.plataforma;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;

namespace PlataformaPDCOnline.Editable.Searchers
{
    public class SearchCreateWebUser : ISearcher
    {
        public Command RunSearcher(Dictionary<string, object> row, WebCommandsController controller) //obligatorio tanto por la interface como que es el metodo que se ejecutara para buscar y crear el command
        {
            CreateWebUser commands = null;

            if (row.GetValueOrDefault("userid").ToString().Equals(""))
            {
                string uid = Guid.NewGuid().ToString(); //generamos el guid del usuario

                //hay que indicar que campo de la tabla es la clave por la que realizaremos las busquedas
                if (ConsultasPreparadas.Singelton().UpdateTableForGUID(controller, row, uid, "usercode") == 1)
                {
                    Console.WriteLine("Preparando el command CreateWebUser");
                    commands = new CreateWebUser(uid) { Username = row.GetValueOrDefault("username").ToString(), Usercode = row.GetValueOrDefault("usercode").ToString() };
                }
            }
            else
            {
                //mirar en la base de datos del marc si existe, si existe no hacemos nada, si no existe volvemos a enviar el command
                Boolean exist = false;

                if (!exist)
                {
                    Console.WriteLine("Re-enviando command CreateWebUser");
                    commands = new CreateWebUser(row.GetValueOrDefault("userid").ToString()) { Username = row.GetValueOrDefault("username").ToString(), Usercode = row.GetValueOrDefault("usercode").ToString() };
                }
            }

            return commands; //devuelvo el command
        }
    }

    public class SearchUpdateWebUser : ISearcher
    {
        public Command RunSearcher(Dictionary<string, object> row, WebCommandsController controller)
        {
            UpdateWebUser commands = null;
            Console.WriteLine("Running searcher Update");

            commands = new UpdateWebUser(row.GetValueOrDefault("userid").ToString()) { Username = row.GetValueOrDefault("username").ToString() };
            /*
             * Passos a seguir:
             * buscamos en la otra base de datos el id de nuestra row, si no existe, creamos y enviamos un command de createWebUser.
             * si existe, comparamos parametro a parametro que cambio hay, una vez encontrado, instanciamos y devolvemos el command
             */ 
            return commands;
        }
    }

    public class SearchDeleteWebUser : ISearcher
    {
        public Command RunSearcher(Dictionary<string, object> row, WebCommandsController controller)
        {
            DeleteWebUser commands = null;

            //lo sullo seria comprovar que en la otra base de datos no se ha eliminado, si se ha eliminado se quitaria el flag de changevalue a -1
            
            Console.WriteLine("Preparando el command DeleteWebUser");
            commands = new DeleteWebUser(row.GetValueOrDefault("userid").ToString());
            

            return commands;
        }
    }
}
