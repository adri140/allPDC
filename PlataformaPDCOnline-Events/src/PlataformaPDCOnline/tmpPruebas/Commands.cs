using Pdc.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdc.Integration.BoundaryContext
{
    public class CreateCustomer : Command
    {
        public CreateCustomer(string aggregateId) : base(aggregateId, null)
        {

        }

        public string Name { get; set; }
    }

    public class RenameCustomer : Command
    {
        public RenameCustomer(string aggregateId) : base(aggregateId, null)
        {

        }

        public string Name { get; set; }
    }

    public class MoveCustomer : Command
    {
        public MoveCustomer(string aggregateId) : base(aggregateId, null)
        {

        }

        public string Address { get; set; }
    }
}
