using Pdc.Messaging;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;
using PlataformaPDCOnline.Internals.plataforma;
using System;
using System.Collections.Generic;

namespace PlataformaPDCOnline.Editable.Searchers
{
    public class SearchCreateWebAccessGroup : ISearcher
    {
        public Command RunSearcher(Dictionary<string, object> row, WebCommandsController controller) //obligatorio tanto por la interface como que es el metodo que se ejecutara para buscar y crear el command
        {
            CreateWebAccessGroup commands = null;

            if (row.GetValueOrDefault("accessgroupid").ToString().Equals(""))
            {
                string uid = Guid.NewGuid().ToString(); //generamos el guid del usuario

                //hay que indicar que campo de la tabla es la clave por la que realizaremos las busquedas
                if (ConsultasPreparadas.Singelton().UpdateTableForGUID(controller, row, uid, "accessgroupname") == 1) //la consulta ja se ocupa de actualizar-lo y nos devuelve el numero de filas actualizadas
                {
                    Console.WriteLine("Preparando el command CreateWebAccessGroup");
                    commands = new CreateWebAccessGroup(uid) { Accessgroupname = row.GetValueOrDefault("accessgroupname").ToString() };
                }
            }
            else
            {
                Boolean exist = false; //comprovamos en la base de datos del marc si existe

                if (!exist)
                {
                    Console.WriteLine("Re-enviando command CreateWebAccessGroup");
                    commands = new CreateWebAccessGroup(row.GetValueOrDefault("accessgroupid").ToString()) { Accessgroupname = row.GetValueOrDefault("accessgroupname").ToString() };
                }
            }

            return commands; //devuelvo el command
        }
    }
}
