using Microsoft.Extensions.Logging;
using Pdc.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pdc.Integration.BoundaryContext
{
    class Customer : AggregateRoot, ISaga<CustomerCreated>, ISaga<CustomerRenamed>, ISaga<CustomerMoved>
    {
        public Customer(ILogger<AggregateRoot> logger) : base(logger)
        {

        }

        public string Name { get; private set; }
        public string Address { get; private set; }

        public async Task CreateCustomer(CreateCustomer command)
        {
            await RaiseEventAsync(new CustomerCreated(command.AggregateId, command.Name, command));
        }

        void ISaga<CustomerCreated>.Apply(CustomerCreated @event)
        {
            Id = @event.Id;
            Name = @event.Name;
        }

        public async Task Rename(RenameCustomer command)
        {
            if (command.AggregateId != Id)
            {
                throw new InvalidOperationException("The command was not sended to this aggregate root.");
            }

            await RaiseEventAsync(new CustomerRenamed(Id, command.Name, command));
        }

        void ISaga<CustomerRenamed>.Apply(CustomerRenamed @event)
        {
            Name = @event.Name;
        }

        public async Task Move(MoveCustomer command)
        {
            if (command.AggregateId != Id)
            {
                throw new InvalidOperationException("The command was not sended to this aggregate root.");
            }

            await RaiseEventAsync(new CustomerMoved(Id, command.Address, command));
        }

        void ISaga<CustomerMoved>.Apply(CustomerMoved @event)
        {
            Address = @event.Address;
        }
    }
}
