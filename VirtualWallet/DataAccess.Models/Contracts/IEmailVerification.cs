using DataAccess.Models.Models;

namespace DataAccess.Models.Contracts
{
    public interface IEmailVerification
    {
        public string VerificationCode { get; set; }
        bool IsVerified { get; set; }
        void SendVerificationEmail(User user, string verificationCode);
        bool VerifyEmail(User user, string verificationCode);
        void SendEmail(string toEmail, string subject, string body);
    }
}