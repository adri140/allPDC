using Pdc.Messaging;
using PlataformaPDCOnline.Editable.pdcOnline.Events;
using PlataformaPDCOnline.internals.plataforma;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Editable.EventsHandlers
{
    public class WebUserCreatedHandler : IEventHandler<WebUserCreated>
    {
        public async Task HandleAsync(WebUserCreated message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Recivido - WebUserCreated");
            await ConsultasPreparadas.Singelton().ExecuteCommitCommit(message);
        }
    }

    public class WebUserUpdatedHandler : IEventHandler<WebUserUpdated>
    {
        public async Task HandleAsync(WebUserUpdated message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Recivido - WebUserUpdated");
            await ConsultasPreparadas.Singelton().ExecuteCommitCommit(message);
        }
    }

    public class WebUserDeletedHandler : IEventHandler<WebUserDeleted>
    {
        public async Task HandleAsync(WebUserDeleted message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Recivido - WebUserDeleted");
            await ConsultasPreparadas.Singelton().ExecuteCommitCommit(message);
        }
    }
}
