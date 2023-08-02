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
        public Task<Message> BuildEmailAsync(User user, string confirmationLink)
        {
            var message = new Message(new string[] { user.Email });
            message.Content += confirmationLink;

            return Task.FromResult(message);
        }
        public async Task SendEMailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(emailConfiguration.SMTPServer, emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(emailConfiguration.Username, emailConfiguration.Password);

                await client.SendAsync(mailMessage);
            }
            catch (Exception)
            {

                throw new InvalidOperationException("An error occurred while sending the email. Please try again later.");
            }
            finally
            {
                await client.DisconnectAsync(true);
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
