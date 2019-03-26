using Pdc.Messaging;

namespace PlataformaPDCOnline.Editable.pdcOnline.Commands
{
    public class CreateWebUser : Command
    {
        public CreateWebUser(string aggregateId) : base(aggregateId, null)
        {

        }

        public string Username { get; set; }
        public string Usercode { set; get; }
    }

    public class UpdateWebUser : Command
    {
        public UpdateWebUser(string aggregateId) : base(aggregateId, null)
        {

        }

        public string Username { set; get; }
    }

    public class DeleteWebUser : Command
    {
        public DeleteWebUser(string aggregateId) : base(aggregateId, null)
        {

        }
    }
}
