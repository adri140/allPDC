using Pdc.Messaging;
using Pdc.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pdc.Integration.BoundaryContext
{
    public class CreateCustomerHandler : ICommandHandler<CreateCustomer>
    {
        private readonly IUnitOfWork unitOfWork;

        public CreateCustomerHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(CreateCustomer message, CancellationToken cancellationToken = default)
        {
            var customer = await unitOfWork.CreateAsync<Customer>(c => c.CreateCustomer(message));
            await unitOfWork.CommitAsync();
        }
    }

    public class RenameCustomerHandler : ICommandHandler<RenameCustomer>
    {
        private readonly IUnitOfWork unitOfWork;

        public RenameCustomerHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(RenameCustomer message, CancellationToken cancellationToken = default)
        {
            var customer = await unitOfWork.FindAsync<Customer>(message.AggregateId);
            await customer.Rename(message);
            await unitOfWork.CommitAsync();
        }
    }

    public class MoveCustomerHandler : ICommandHandler<MoveCustomer>
    {
        private readonly IUnitOfWork unitOfWork;

        public MoveCustomerHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(MoveCustomer message, CancellationToken cancellationToken = default)
        {
            var customer = await unitOfWork.FindAsync<Customer>(message.AggregateId);
            await customer.Move(message);
            await unitOfWork.CommitAsync();
        }
    }
}
