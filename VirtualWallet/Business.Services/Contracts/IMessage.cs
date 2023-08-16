using MimeKit;

namespace Business.Services.Contracts
{
    public interface IMessage
    {
        public List<MailboxAddress> To { get; set; }
       
        public string Subject { get; set; }
        
        public string Content { get; set; }
    }
}
