using FifthGroup_Backstage.Interfaces;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_Backstage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailAPIController : ControllerBase
    {
        private readonly IEmailService emailService;

        public EmailAPIController(IEmailService service)
        {
            this.emailService = service;
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail()
        {
            try
            {
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = "darwin.hayes@ethereal.email";
                mailrequest.Subject = "歡迎來到無聲鄰陸社區";
                mailrequest.Body = GetHtmlcontent();

                await emailService.SendEmailAsync(mailrequest);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetHtmlcontent()
        {
            string response = "<h1>歡迎來到無聲鄰陸社區</h1>";
            response += "<h2>您已報名<桌球>活動~</h2>";
            response += "<h2>您報名的時間為<2023/9/15 晚上7:00~晚上8:00></h2>";
            response += "<h2>敬請踴躍參加</h2>";
            response += "<br>";
            response += "<h2>無聲鄰陸社區提醒您~</h2>";

            return response;
        }

    }
}
