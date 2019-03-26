using Pdc.Messaging;
using PlataformaPDCOnline.Editable.ClassTab;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;

namespace PlataformaPDCOnline.Editable.pdcOnline.Events
{
    public class WebUserCreated : Event
    {
        public WebUserCreated(string aggregateId, string username, string usercode, CreateWebUser previous)
            : base(typeof(WebUser).Name, aggregateId, 0, previous)
        {
            Id = aggregateId;
            this.Username = username;
            this.Usercode = usercode;
        }

        public string Id { get; set; }
        public string Username { set; get; }
        public string Usercode { set; get; }
    }

    public class WebUserUpdated : Event
    {
        public WebUserUpdated(string aggregateId, string username, UpdateWebUser previous)
            : base(typeof(WebUser).Name, aggregateId, 0, previous)
        {
            Id = aggregateId;
            this.Username = username;
        }

        public string Id { get; set; }
        public string Username { set; get; }
    }

    public class WebUserDeleted : Event
    {
        public WebUserDeleted(string aggregateId, DeleteWebUser previous)
            : base(typeof(WebUser).Name, aggregateId, 0, previous)
        {
            Id = aggregateId;
        }

        public string Id { get; set; }
    }
}
