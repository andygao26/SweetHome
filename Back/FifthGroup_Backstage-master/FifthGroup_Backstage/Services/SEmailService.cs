using FifthGroup_Backstage.Interfaces;
using FifthGroup_Backstage.ViewModel;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace FifthGroup_Backstage.Services
{
    public class SEmailService : IEmailService
    {
        private readonly CEmailSettings cemailSettings;
        public SEmailService(IOptions<CEmailSettings> options)
        {
            this.cemailSettings = options.Value;
        }
        public async Task SendEmailAsync(Mailrequest mailrequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(cemailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
            email.Subject = mailrequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailrequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(cemailSettings.Host, cemailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(cemailSettings.Email, cemailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
