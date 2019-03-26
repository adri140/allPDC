using Pdc.Messaging;
using PlataformaPDCOnline.Editable.ClassTab;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;

namespace PlataformaPDCOnline.Editable.pdcOnline.Events
{
    public class WebAccessGroupCreated : Event
    {
        public WebAccessGroupCreated(string aggregateId, string accessgroupname, CreateWebAccessGroup previous)
                   : base(typeof(WebAccessGroup).Name, aggregateId, 0, previous)
        {
            Id = aggregateId;
            this.Accessgroupname = accessgroupname;
        }

        public string Id { get; set; }
        public string Accessgroupname { set; get; }
    }
}
