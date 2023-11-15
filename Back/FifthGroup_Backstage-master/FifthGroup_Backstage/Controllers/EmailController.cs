using FifthGroup_Backstage.Interfaces;
using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.Repositories;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace FifthGroup_Backstage.Controllers
{
    public class EmailController : Controller
    {
        DbHouseContext db = new DbHouseContext();

        private readonly DbHouseContext _context;
        private readonly IEmailService emailService;

        public EmailController(DbHouseContext context, IEmailService emailService)
        {
            _context = context;
            this.emailService = emailService;
        }



        //Email寄信歷史紀錄
        public IActionResult EmailList(CKeywordViewModel vm, int? Page)
        {
            DbHouseContext db = new DbHouseContext();
            IQueryable<CEmails> datas = db.Emails.Join(
                db.Residents,
                email => email.HouseholdCode,
                resident => resident.HouseholdCode,
                (email, resident) => new CEmails
                {
                    Email = email,
                    ResidentName = resident.Name
                    // 將其他需要的屬性添加到這裡
                }
            );

            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                // Try to parse the input keyword as an integer
                if (int.TryParse(vm.txtKeyword, out int keywordAsInt))
                {
                    datas = datas.Where(t => t.Email.EmailCode == keywordAsInt);
                }
                else
                {
                    datas = datas.Where(t =>
                        t.Email.HouseholdCode.Contains(vm.txtKeyword) ||
                        t.Email.Subject.Contains(vm.txtKeyword) ||
                        t.Email.FromEmail.Contains(vm.txtKeyword) ||
                        t.Email.ToEmail.Contains(vm.txtKeyword) ||
                        t.Email.Body.Contains(vm.txtKeyword) ||
                        t.ResidentName.Contains(vm.txtKeyword)
                    );
                }
            }

            //此區為每10筆資料一頁的處理區controller
            int pageNumber = Page.HasValue ? Page.Value : 1; // 设置默认值为1，以防 pageNumber 为 null
            var varResultList = datas.OrderByDescending(x => x.Email.EmailCode).ToPagedList(pageNumber, 10); // 每页显示10条记录
            ViewBag.EmailPage = varResultList;
            //\\此區為每10筆資料一頁的處理區controller
            if (Request.IsAjaxRequest())
            {
                return PartialView("EmailList", varResultList); // 返回部分視圖
            }

            return View(varResultList);
        }

        //撰寫Email
        public IActionResult Create()
        {
          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CEmails cEmails)
        {
            if (cEmails != null)
            {
                if (string.IsNullOrWhiteSpace(cEmails.ResidentName) || cEmails.ResidentName == "0" ||
                    string.IsNullOrWhiteSpace(cEmails.HouseholdCode) || cEmails.HouseholdCode == "0" ||
                    string.IsNullOrWhiteSpace(cEmails.Subject) || cEmails.Subject == "0" ||
                    string.IsNullOrWhiteSpace(cEmails.FromEmail) || cEmails.FromEmail == "0" ||
                    string.IsNullOrWhiteSpace(cEmails.ToEmail) || cEmails.ToEmail == "0"
                    )
                {
                    // 检查多个属性是否为零或 null，返回验证错误
                    if (string.IsNullOrWhiteSpace(cEmails.ResidentName) || cEmails.ResidentName == "0")
                    {
                        ModelState.AddModelError("ResidentName", "住戶姓名不可為空白或零，請填寫!");
                    }

                    if (string.IsNullOrWhiteSpace(cEmails.HouseholdCode) || cEmails.HouseholdCode == "0")
                    {
                        ModelState.AddModelError("HouseholdCode", "戶號不可為空白或零，請填寫住家戶號!");
                    }

                    if (string.IsNullOrWhiteSpace(cEmails.Subject) || cEmails.Subject == "0")
                    {
                        ModelState.AddModelError("Subject", "主題不可為空白或零，請填寫!");
                    }

                    if (string.IsNullOrWhiteSpace(cEmails.FromEmail) || cEmails.FromEmail == "0")
                    {
                        ModelState.AddModelError("FromEmail", "寄件人郵箱不可為空白或零，請填寫!");
                    }

                    if (string.IsNullOrWhiteSpace(cEmails.ToEmail) || cEmails.ToEmail == "0")
                    {
                        ModelState.AddModelError("ToEmail", "收件人郵箱不可為空白或零，請填寫!");
                    }

                    

                    return View(cEmails);
                }

                // 这里继续处理邮件发送和数据库保存的逻辑
                cEmails.Time = DateTime.Now;

                // 处理 Body 字段为空的情况
                if (string.IsNullOrWhiteSpace(cEmails.Body))
                {
                    // 设置一个默认值或者为空字符串，以防止 GetHtmlcontent 方法抛出异常
                    cEmails.Body = "";
                }

                Email e = new Email()
                {
                    HouseholdCode = cEmails.HouseholdCode,
                    Subject = cEmails.Subject,
                    FromEmail = cEmails.FromEmail,
                    ToEmail = cEmails.ToEmail,
                    Time = cEmails.Time,
                    Body = cEmails.Body,
                    HouseholdCodeNavigation = cEmails.HouseholdCodeNavigation
                };
                _context.Emails.Add(e);
                await _context.SaveChangesAsync();

                try
                {
                    Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = cEmails.ToEmail;
                mailrequest.Subject = cEmails.Subject;
                mailrequest.Body = GetHtmlcontent(cEmails.Body);

                await emailService.SendEmailAsync(mailrequest);
                }
                catch (Exception ex)
                {
                    throw;
                }

                return RedirectToAction("EmailList");
            }

            return View(cEmails);
        }

        //專門抓householdCode回傳表單for Create
        [HttpGet]
        public JsonResult GetResidentDetails(string residentName)
        {
            var resident = _context.Residents.FirstOrDefault(r => r.Name == residentName);
            if (resident != null)
            {
                return Json(new { householdCode = resident.HouseholdCode, email = resident.Email });
            }
            return Json(new { householdCode = "", email = "" });
        }


        //專門驗證input並回傳給blur事件的方法for Create
        [HttpGet]
        public IActionResult ValidateInput(string inputId, string inputValue)
        {
            bool isValid = true;
            string errorMessage = "";

            // 根據 inputId 來驗證 inputValue
            switch (inputId)
            {
                case "ResidentName":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "住戶姓名不可為空白或零，請填寫!";
                    }
                    break;
                case "HouseholdCode":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "戶號不可為空白或零，請填寫住家戶號!";
                    }
                    break;
                case "Subject":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "主題不可為空白或零，請填寫!";
                    }
                    break;
                case "FromEmail":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "寄件人郵箱不可為空白或零，請填寫!";
                    }
                    break;
                case "ToEmail":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "收件人郵箱不可為空白或零，請填寫!";
                    }
                    break;
                    // 你可以根據需要添加更多的 case
            }

            // 返回驗證結果
            return Json(new { isValid = isValid, errorMessage = errorMessage });
        }



        private string GetHtmlcontent(string bodyContent)
        {
            var lines = bodyContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string response = "<h1>歡迎來到無聲鄰陸社區</h1>";
            foreach (var line in lines)
            {
                response += $"<h2>{line}</h2>";
            }
            response += "<br>";
            response += "<h2>無聲鄰陸社區提醒您~</h2>";

            return response;
        }

        //查看信件內容
        public IActionResult CheckEmailContent(int? id)
        {
            if (id == null)
                return RedirectToAction("EmailList");
            DbHouseContext db = new DbHouseContext();
            Email email = db.Emails.FirstOrDefault(t => t.EmailCode == id);
            if (email == null)
                return RedirectToAction("EmailList");

            // 使用HouseholdCode查找Resident
            Resident resident = db.Residents.FirstOrDefault(r => r.HouseholdCode == email.HouseholdCode);

            // 將 IsRead 欄位設置為 true
            email.IsRead = true;

            // 將更改保存回數據庫
            db.SaveChanges();


            // 創建視圖模型並設置其屬性
            var model = new CEmails
            {
                Email = email,
                ResidentName = resident?.Name
            };

            return View(model);
        }

        [HttpGet]
        public JsonResult GetResidentDetailsForCheckEmailContent(string residentName)
        {
            var resident = _context.Residents.FirstOrDefault(r => r.Name == residentName);
            if (resident != null)
            {
                return Json(new { householdCode = resident.HouseholdCode, email = resident.Email });
            }
            return Json(new { householdCode = "", email = "" });
        }


        //刪除信件
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("EmailList");
            DbHouseContext db = new DbHouseContext();
            Email email = db.Emails.FirstOrDefault(t => t.EmailCode == id);
            if (email != null)
            {
                db.Emails.Remove(email);
                db.SaveChanges();
            }
            return RedirectToAction("EmailList");
        }

        public IActionResult UnreadEmailCount()
        {
            // 從數據庫中獲取所有未讀的電子郵件
            var unreadEmails = _context.Emails.Where(email => !email.IsRead);

            // 計算未讀電子郵件的數量
            var count = unreadEmails.Count();

            // 返回未讀電子郵件的數量
            return Json(new { count = count });
        }

        public IActionResult UnreadEmails()
        {
            // 從數據庫中獲取所有未讀的電子郵件
            var unreadEmails = _context.Emails.Where(email => !email.IsRead);

            // 將未讀電子郵件轉換為適合顯示在下拉菜單中的格式
            var emails = unreadEmails.Select(email => new {
                id = email.EmailCode,
                subject = email.Subject
            }).ToList();

            // 返回未讀電子郵件的列表
            return Json(emails);
        }

        [HttpGet]
        public IActionResult GetUnreadEmails()
        {
            DbHouseContext db = new DbHouseContext();
            var unreadEmails = db.Emails.Where(e => e.IsRead == false)
                                        .Select(e => new { e.EmailCode, e.Subject })
                                        .ToList();
            return Json(unreadEmails);
        }

    }
}

