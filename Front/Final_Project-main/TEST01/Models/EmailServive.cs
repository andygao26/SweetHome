//using Microsoft.Extensions.Configuration;
//using System.Net;
//using System.Net.Mail;
//public class EmailService
//{
//    private readonly IConfiguration _configuration;
//    public EmailService(IConfiguration configuration)
//    {
//        _configuration = configuration;
//    }
//    public void SendEmail(string to, string subject, string body)
//    {
//        var emailSettings = _configuration.GetSection("EmailSettings");
//        using (var client = new SmtpClient())
//        {
//            var credentials = new NetworkCredential
//            {
//                UserName = emailSettings["SmtpUsername"],
//                Password = emailSettings["SmtpPassword"]
//            };
//            client.Credentials = credentials;
//            client.Host = emailSettings["SmtpServer"];
//            client.Port = int.Parse(emailSettings["SmtpPort"]);
//            client.EnableSsl = bool.Parse(emailSettings["UseSsl"]);
//            using (var emailMessage = new MailMessage())
//            {
//                emailMessage.From = new
//               MailAddress(emailSettings["SmtpUsername"]);
//                emailMessage.To.Add(to);
//                emailMessage.Subject = subject;
//                emailMessage.Body = body;
//                emailMessage.IsBodyHtml = true;
//                client.Send(emailMessage);
//            }
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System;
using Microsoft.Extensions.Options;
using FifthGroup_front.ViewModels;

namespace FifthGroup_front.Models
{


    public class EmailService
    {
        private readonly BEmailViewModel _smtpSettings;

        public EmailService(IOptions<BEmailViewModel> p)
        {
            _smtpSettings = p.Value;

        }


        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {

            using (var client = new SmtpClient(_smtpSettings.Host))
            {
                client.Port = _smtpSettings.Port;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                client.EnableSsl = true;


                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Username),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);

            }
        }
    }
}
