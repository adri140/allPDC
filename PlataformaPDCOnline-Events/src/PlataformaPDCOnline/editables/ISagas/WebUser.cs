using Microsoft.Extensions.Logging;
using Pdc.Domain;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;
using PlataformaPDCOnline.Editable.pdcOnline.Events;
using System;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Editable.ClassTab
{
    public class WebUser : AggregateRoot, ISaga<WebUserCreated>, ISaga<WebUserDeleted>, ISaga<WebUserUpdated>
    {
        public WebUser(ILogger<AggregateRoot> logger) : base(logger)
        {

        }

        public string Username { get; private set; }
        public string Usercode { get; private set; }

        public async Task CreateWebUser(CreateWebUser command)
        {
            //Console.WriteLine("Creando evento WebUserCreated");
            await RaiseEventAsync(new WebUserCreated(command.AggregateId, command.Username, command.Usercode, command));
        }

        void ISaga<WebUserCreated>.Apply(WebUserCreated @event)
        {
            Id = @event.Id;
            Username = @event.Username;
            Usercode = @event.Usercode;
        }

        public async Task UpdateWebUser(UpdateWebUser command)
        {
            if (command.AggregateId != Id)
            {
                throw new InvalidOperationException("The command was not sended to this aggregate root.");
            }

            //Console.WriteLine("Updateando evento WebUserUpdated");
            await RaiseEventAsync(new WebUserUpdated(Id, command.Username, command));
        }

        void ISaga<WebUserUpdated>.Apply(WebUserUpdated @event)
        {
            Id = @event.Id;
            Username = @event.Username;
        }

        public async Task DeleteWebUser(DeleteWebUser command)
        {
            if (command.AggregateId != Id)
            {
                throw new InvalidOperationException("The command was not sended to this aggregate root.");
            }
            //Console.WriteLine("Eliminando evento WebUserDeleted");
            await RaiseEventAsync(new WebUserDeleted(Id, command));
        }

        void ISaga<WebUserDeleted>.Apply(WebUserDeleted @event)
        {
            Id = @event.Id;
        }
    }
}
