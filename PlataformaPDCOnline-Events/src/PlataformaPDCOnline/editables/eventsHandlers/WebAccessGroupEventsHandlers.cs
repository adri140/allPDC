using Pdc.Messaging;
using PlataformaPDCOnline.Editable.pdcOnline.Events;
using PlataformaPDCOnline.internals.plataforma;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Editable.EventsHandlers
{
    public class WebAccessGroupCreatedHandler : IEventHandler<WebAccessGroupCreated>
    {
        public async Task HandleAsync(WebAccessGroupCreated message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Recivido - WebAccessGroupCreated");
            await ConsultasPreparadas.Singelton().ExecuteCommitCommit(message);
        }
    }
}
