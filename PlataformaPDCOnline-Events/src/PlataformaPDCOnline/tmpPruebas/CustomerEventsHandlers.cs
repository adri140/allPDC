using Pdc.Integration.BoundaryContext;
using Pdc.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pdc.Integration.Denormalization
{
    public class CustomerDenormalizer : IEventHandler<CustomerCreated>, IEventHandler<CustomerRenamed>
    {
        public async Task HandleAsync(CustomerCreated message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Customer Created");
        }

        public async Task HandleAsync(CustomerRenamed message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Customer Renamed");
        }
    }
}
