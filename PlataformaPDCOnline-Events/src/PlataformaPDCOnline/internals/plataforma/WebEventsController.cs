using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaPDCOnline.internals.plataforma
{
    public class WebEventsController
    {
        private static List<WebEventsController> Controllers;

        public static List<WebEventsController> Singelton()
        {
            if(Controllers == null) RefreshAllControllers();

            return Controllers;
        }

        public static WebEventsController Singelton(string eventName)
        {
            if (Controllers == null) RefreshAllControllers();

            foreach(WebEventsController controller in Controllers)
            {
                if (controller.EventName.Equals(eventName)) return controller;
            }

            return null;
        }

        public static void RefreshAllControllers()
        {
            if (Controllers == null) Controllers = new List<WebEventsController>();
            else Controllers.Clear();

            foreach (Dictionary<string, object> row in ConsultasPreparadas.Singelton().GetWebEvents())
            {
                Controllers.Add(new WebEventsController(row));
            }
        }

        public string EventName { get; }
        public string TableName { get; }
        public string UidName { get; }

        private WebEventsController(Dictionary<string, object> row)
        {
            EventName = row.GetValueOrDefault("eventname").ToString();
            TableName = row.GetValueOrDefault("tablename").ToString();
            UidName = row.GetValueOrDefault("udiname").ToString();
        }
    }
}
