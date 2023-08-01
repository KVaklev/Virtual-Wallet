using Business.Services.Additional;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IEmailService
    {
        Message BuildEmail(User user, string confirmationLink);
        void SendEMail(Message message);
    }
}
