using Pdc.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdc.Integration.BoundaryContext
{
    public class CustomerCreated : Event
    {
        public CustomerCreated(string aggregateId, string name, CreateCustomer previous)
            : base(typeof(Customer).Name, aggregateId, 0, previous)
        {
            Id = aggregateId;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class CustomerRenamed : Event
    {
        public CustomerRenamed(string aggregateId, string name, RenameCustomer previous)
            : base(typeof(Customer).Name, aggregateId, 0, previous)
        {
            Id = aggregateId;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class CustomerMoved : Event
    {
        public CustomerMoved(string aggregateId, string address, MoveCustomer previous)
            : base(typeof(Customer).Name, aggregateId, 0, previous)
        {
            Id = aggregateId;
            Address = address;
        }

        public string Id { get; set; }

        public string Address { get; set; }
    }
}
