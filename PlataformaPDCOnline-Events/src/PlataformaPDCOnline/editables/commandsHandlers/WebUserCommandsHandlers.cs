using Pdc.Messaging;
using Pdc.UnitOfWork;
using PlataformaPDCOnline.Editable.ClassTab;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Editable.CommandsHandlers
{
    public class CreateWebAccessGroupHandler : ICommandHandler<CreateWebAccessGroup>
    {
        private readonly IUnitOfWork unitOfWork;

        public CreateWebAccessGroupHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(CreateWebAccessGroup message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Create Web User Command Recibido");
            var user = await unitOfWork.CreateAsync<WebAccessGroup>(c => c.CreateWebAccessGroup(message));
            await unitOfWork.CommitAsync();
        }
    }
}
