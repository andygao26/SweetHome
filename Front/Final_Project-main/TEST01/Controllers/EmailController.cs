using FifthGroup_front.Models;
using FifthGroup_front.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using X.PagedList;

namespace FifthGroup_front.Controllers
{
    public class EmailController : Controller
    {
        DbHouseContext db = new DbHouseContext();

        private readonly DbHouseContext _context;
        //Session用的全域變數_httpContextAccessor
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailController(DbHouseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
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

            //取 住戶資料表的戶號) { }
            string userHouseholdCode = _httpContextAccessor.HttpContext.Session.GetString("UserHouseholdCode");

            // 如果用户的住戶資料表的戶號不为空，则筛选数据
            if (!string.IsNullOrEmpty(userHouseholdCode))
            {
                datas = datas.Where(t => t.Email.HouseholdCode == userHouseholdCode);
            }

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
            var varResultList = datas.OrderByDescending(x => x.Email.Time).ToPagedList(pageNumber, 10); // 每页显示10条记录
            ViewBag.EmailPage = varResultList;
            //\\此區為每10筆資料一頁的處理區controller
            if (Request.IsAjaxRequest())
            {
                return PartialView("EmailList", varResultList); // 返回部分視圖
            }

            return View(varResultList);
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
