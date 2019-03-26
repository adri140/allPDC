using Pdc.Messaging;
using Pdc.UnitOfWork;
using PlataformaPDCOnline.Editable.ClassTab;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Editable.CommandsHandlers
{
    public class CreateWebUserHandler : ICommandHandler<CreateWebUser>
    {
        private readonly IUnitOfWork unitOfWork;

        public CreateWebUserHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(CreateWebUser message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Create Web User Command Recibido");
            var user = await unitOfWork.CreateAsync<WebUser>(c => c.CreateWebUser(message));
            await unitOfWork.CommitAsync();
        }
    }

    public class UpdateWebUserHandler : ICommandHandler<UpdateWebUser>
    {
        private readonly IUnitOfWork unitOfWork;

        public UpdateWebUserHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(UpdateWebUser message, CancellationToken cancellationToken = default)
        {
            var user = await unitOfWork.FindAsync<WebUser>(message.AggregateId);
            await user.UpdateWebUser(message);
            await unitOfWork.CommitAsync();
        }
    }

    public class DeleteWebUserHandler : ICommandHandler<DeleteWebUser>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteWebUserHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(DeleteWebUser message, CancellationToken cancellationToken = default)
        {
            var user = await unitOfWork.FindAsync<WebUser>(message.AggregateId);
            await user.DeleteWebUser(message);
            await unitOfWork.CommitAsync();
        }
    }
}
