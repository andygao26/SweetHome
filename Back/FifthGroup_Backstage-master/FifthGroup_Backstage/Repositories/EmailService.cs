using System.Net.Mail;

namespace FifthGroup_Backstage.Repositories
{
    public class EmailService
    {
        public void SendMailByGmail(List<string> MailList, string Subject, string Body)
        {
            MailMessage msg = new MailMessage();
            //收件者，以逗號分隔不同收件者
            msg.To.Add(string.Join(",", MailList.ToArray()));
            msg.From = new MailAddress("suzuka89036@gmail.com", "【無聲鄰睦】系統通知信", System.Text.Encoding.UTF8);
            //郵件標題 
            msg.Subject = Subject;
            //郵件標題編碼 
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            //郵件內容  
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
            msg.Priority = MailPriority.Normal;//郵件優先級 

            //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

            //使用應用程式密碼  
            MySmtp.Credentials = new System.Net.NetworkCredential("suzuka89036@gmail.com", "dfvc vhri vwfz hyhe");

            //Gmial 的 smtp 使用 SSL  
            MySmtp.EnableSsl = true;

            MySmtp.Send(msg);
        }
    }
}
