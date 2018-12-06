using System.Net;
using System.Net.Mail;

namespace WindowsStartupTool.Lib
{
    public class EmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly MailMessage _mailMessage;

        const string _sender = "notify.cubicles@gmail.com";
        const string _senderPassword = "Notify@cubicles";

        public EmailSender()
        {
            _smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_sender, _senderPassword)
            };
            _mailMessage = new MailMessage { IsBodyHtml = true };
            _mailMessage.From = new MailAddress(_sender);
        }

        public void Send(string body, string to)
        {
            _mailMessage.Subject = "Weekly report, Windows startup apps from Cubicles domain";
            _mailMessage.Body = body;
            _mailMessage.To.Add(new MailAddress(to));
            _smtpClient.Send(_mailMessage);
        }
    }
}
