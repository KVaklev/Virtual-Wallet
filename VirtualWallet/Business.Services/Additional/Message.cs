using Business.Services.Contracts;
using MimeKit;

namespace Business.Services.Additional
{
    public class Message : IMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; } = "Email confirmation";
        public string Content { get; set; } =
            "Welcome to Speed Pay!" +
            "\r\n\r\nCongratulations! You're just steps away from experiencing the convenience of Speed Pay's Virtual Wallet and seamless money transfers. " +
            "With Speed Pay, you can securely manage your finances, make instant transactions, and enjoy a range of benefits within our vibrant community. " +
            "Say goodbye to traditional hassles and embrace the future of fast and reliable money management." +
             "To get started, simply follow the link below to validate your email address." +
            "\r\n\r\nThank you for choosing Speed Pay. We look forward to providing you with an exceptional financial experience!" +
            "\r\n\r\nBest regards," +
            "\r\nThe Speed Pay Team" +
            "\r\n\r\n";

        public Message(IEnumerable<string> to)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x=>new MailboxAddress("SpeedPay", x)));
        }
    }
}
