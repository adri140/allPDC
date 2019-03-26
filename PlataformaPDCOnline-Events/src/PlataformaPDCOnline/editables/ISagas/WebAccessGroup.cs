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

        public async Task CreateWebAccessGroup(CreateWebAccessGroup command)
        {
            if (command.AggregateId != Id)
            {
                throw new InvalidOperationException("The command was not sended to this aggregate root.");
            }
            //Console.WriteLine("Eliminando evento WebUserDeleted");
            await RaiseEventAsync(new WebAccessGroupCreated(Id, command.Accessgroupname, command));
        }

        public void Apply(WebAccessGroupCreated @event)
        {
            Id = @event.Id;
        }
    }
}