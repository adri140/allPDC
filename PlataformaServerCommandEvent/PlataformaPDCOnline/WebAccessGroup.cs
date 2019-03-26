using Microsoft.Extensions.Logging;
using Pdc.Domain;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;
using PlataformaPDCOnline.Editable.pdcOnline.Events;
using System;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Editable.ClassTab
{
    public class WebAccessGroup : AggregateRoot, ISaga<WebAccessGroupCreated>
    {
        public WebAccessGroup(ILogger<AggregateRoot> logger) : base(logger)
        {

        }

        public string Accessgroupname { set; get; }

        public async Task CreateWebAccessGroup(CreateWebAccessGroup command)
        {
            Console.WriteLine("Eliminando evento WebUserDeleted");
            await RaiseEventAsync(new WebAccessGroupCreated(Id, command.Accessgroupname, command));
        }

        void ISaga<WebAccessGroupCreated>.Apply(WebAccessGroupCreated @event)
        {
            Id = @event.Id;
            Accessgroupname = @event.Accessgroupname;
        }
    }
}
