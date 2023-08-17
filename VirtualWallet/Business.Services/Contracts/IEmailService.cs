using Business.Services.Additional;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IEmailService
    {
        Task<Message> BuildEmailAsync(User user, string confirmationLink);
        
        Task SendEMailAsync(Message message);
    }
}
