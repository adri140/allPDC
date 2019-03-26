using Pdc.Messaging;
using System.Collections.Generic;

namespace PlataformaPDCOnline.Internals.plataforma
{
    /// <summary>
    /// Interface que obliga a implementar el method RunSearcher, el cual realizara la busqueda de cambios en ambas base de datos.
    /// </summary>
    interface ISearcher//<T> where T : Command
    {
        /// <summary>
        /// Ejecuta la busqueda de cambios en las dos base de datos, segun el command seleccionado, el resultado de este es un command, el qual sera enviado posteriormente.
        /// </summary>
        /// <param name="row">Fila de la base de datos informix, la cual comprovaremos si tiene cambios</param>
        /// <param name="controller">El controllador del command que se esta ejecutando en este momento</param>
        /// <returns>Devuelve un command que generara su padre</returns>
        Command RunSearcher(Dictionary<string, object> row, WebCommandsController controller);
    }
}
