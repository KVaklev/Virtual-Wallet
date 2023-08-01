using Business.Services.Additional;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Business.Services.Models
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration emailConfiguration;
        public EmailService(EmailConfiguration emailConfiguration)
        {
            this.emailConfiguration = emailConfiguration;
        }
        public Message BuildEmail(User user, string confirmationLink)
        {
            var message = new Message(new string[] { user.Email });
            message.Content += confirmationLink;

            return message;
        }
        public void SendEMail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(emailConfiguration.SMTPServer, emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(emailConfiguration.Username, emailConfiguration.Password);

                client.Send(mailMessage);
            }
            catch (Exception)
            {

                throw new InvalidOperationException("An error occurred while sending the email. Please try again later.");
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SpeedPay", emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

    }
}
