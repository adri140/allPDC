using Pdc.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaPDCOnline.Editable.pdcOnline.Commands
{
    public class CreateWebAccessGroup : Command
    {
        public CreateWebAccessGroup(string aggregateId) : base(aggregateId, null)
        {

        }

        public string Accessgroupname { get; set; }
    }
}
