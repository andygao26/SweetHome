using FifthGroup_front.Models;
using FifthGroup_front.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using System;
using System.IO;
using System.Linq;
using FifthGroup_front.Interfaces;

namespace FifthGroup_front.Controllers
{
    public class RepairController : Controller
    {
        //Session用的全域變數_httpContextAccessor
        private readonly IHttpContextAccessor _httpContextAccessor;

        //圖片用的全域變數_enviro
        private readonly IWebHostEnvironment _enviro;

        private readonly DbHouseContext _context;

        private readonly IEmailService emailService;


        public RepairController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment p, DbHouseContext context, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _enviro = p;
            _context = context;
            this.emailService = emailService;
        }



        public IActionResult RepairInquire(CKeywordViewModel vm, string txtKeywordItems,string txtKeywordState,int? Page)
        {
            DbHouseContext db = new DbHouseContext();
            IQueryable<Repair> datas = db.Repairs; // Starting with all repairs

            //取 住戶資料表的戶號) { }
            string userHouseholdCode = _httpContextAccessor.HttpContext.Session.GetString("UserHouseholdCode");

            // 如果用户的住戶資料表的戶號不为空，则筛选数据
            if (!string.IsNullOrEmpty(userHouseholdCode))
            {
                datas = datas.Where(t => t.HouseholdCode == userHouseholdCode);
            }

            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                // Try to parse the input keyword as an integer
                if (int.TryParse(vm.txtKeyword, out int keywordAsInt))
                {
                    datas = datas.Where(t => t.RepairCode == keywordAsInt);
                }
                else
                {
                    datas = datas.Where(t =>
                        t.HouseholdCode.Contains(vm.txtKeyword) ||
                        t.Name.Contains(vm.txtKeyword) ||
                        t.Phone.Contains(vm.txtKeyword) ||
                        t.Type.Contains(vm.txtKeyword) ||
                        t.ProcessingStatus.Contains(vm.txtKeyword) ||
                        t.ManufacturerCode.Contains(vm.txtKeyword)
                    );
                }
            }

            if (!string.IsNullOrEmpty(txtKeywordItems))
            {
                datas = datas.Where(t => t.Type == txtKeywordItems);
            }

            if (!string.IsNullOrEmpty(txtKeywordState))
            {
                datas = datas.Where(t => t.ProcessingStatus == txtKeywordState);
            }

            if (DateTime.TryParse(vm.txtKeywordDate, out DateTime keywordAsDateTime))
            {
                datas = datas.Where(t => t.Time.HasValue && t.Time.Value.Date == keywordAsDateTime.Date);
            }

            ViewBag.RepairItems = txtKeywordItems;
            ViewBag.RepairStatus = txtKeywordState;


            //此區為每10筆資料一頁的處理區controller
            int pageNumber = Page.HasValue ? Page.Value : 1; // 设置默认值为1，以防 pageNumber 为 null

            // 將 "未處理"、"處理中" 和 "已完成" 的狀態轉換為可以排序的數字
            Func<string, int> statusToOrder = status => status switch
            {
                "未處理" => 0,
                "處理中" => 1,
                "已完成" => 2,
                _ => 3
            };

            var varResultList = datas
                .AsEnumerable() // 先將資料全部取出
                .OrderBy(x => statusToOrder(x.ProcessingStatus)) // 先按照 ProcessingStatus 排序
                .ThenByDescending(x => x.Time) // 再按照 Time 排序
                .ToPagedList(pageNumber, 10); // 每页显示10条记录

            ViewBag.RepairPage = varResultList;
            //\\此區為每10筆資料一頁的處理區controller

            //原先資料
            //此區為每10筆資料一頁的處理區controller
            //int pageNumber = Page.HasValue ? Page.Value : 1; // 设置默认值为1，以防 pageNumber 为 null
            //var varResultList = datas.OrderBy(x => x.RepairCode).ToPagedList(pageNumber, 10); // 每页显示10条记录
            //ViewBag.RepairPage = varResultList;
            //\\此區為每10筆資料一頁的處理區controller

            return View(varResultList);
            
        }


        public IActionResult CheckTheDetails(int? id)
        {
            if (id == null)
                return RedirectToAction("RepairInquire");
            DbHouseContext db = new DbHouseContext();
            Repair repair = db.Repairs.FirstOrDefault(t => t.RepairCode == id);
            if (repair == null)
                return RedirectToAction("RepairInquire");
            //CProductWrap prodWp = new CProductWrap();
            //prodWp.product = repair;
            return View(repair);
        }


        public IActionResult Create()
        {
            return View(new CEmails());
        }


        [HttpPost]
        public async Task<IActionResult> Create(CEmails model)
        {
            if (string.IsNullOrWhiteSpace(model.Repair.HouseholdCode) || model.Repair.HouseholdCode == "0" ||
                string.IsNullOrWhiteSpace(model.Repair.Name) || model.Repair.Name == "0" ||
                string.IsNullOrWhiteSpace(model.Repair.Phone) || model.Repair.Phone == "0" ||
                string.IsNullOrWhiteSpace(model.Repair.Type) || model.Repair.Type == "0" ||
                string.IsNullOrWhiteSpace(model.Repair.ProcessingStatus) || model.Repair.ProcessingStatus == "0" ||
                !model.Repair.Time.HasValue)
            {
                // 检查多个属性是否为零或 null，返回验证错误
                if (string.IsNullOrWhiteSpace(model.Repair.HouseholdCode) || model.Repair.HouseholdCode == "0")
                {
                    ModelState.AddModelError("HouseholdCode", "戶號不可為空白或零，請填寫住家戶號!");
                }

                if (string.IsNullOrWhiteSpace(model.Repair.Name) || model.Repair.Name == "0")
                {
                    ModelState.AddModelError("Name", "姓名不可為空白或零，請填寫!");
                }

                if (string.IsNullOrWhiteSpace(model.Repair.Phone) || model.Repair.Phone == "0")
                {
                    ModelState.AddModelError("Phone", "電話不可為空白或零，請填寫(參考格式:0972468159)");
                }

                if (string.IsNullOrWhiteSpace(model.Repair.Type) || model.Repair.Type == "0")
                {
                    ModelState.AddModelError("Type", "請選擇報修項目，此欄不能空白!");
                }

                if (!model.Repair.Time.HasValue)
                {
                    ModelState.AddModelError("Time", "請選擇時間，此欄不能空白!");
                }

                if (string.IsNullOrWhiteSpace(model.Repair.ProcessingStatus) || model.Repair.ProcessingStatus == "0")
                {
                    ModelState.AddModelError("ProcessingStatus", "請選擇處理狀態，此欄不能空白!");
                }

                if (!string.IsNullOrWhiteSpace(model.ResidentEmail) && model.ResidentEmail != "0")
                {
                    var atIndex = model.ResidentEmail.IndexOf('@');
                    if (atIndex <= 0 || atIndex >= model.ResidentEmail.Length - 1)
                    {
                        ModelState.AddModelError("ResidentEmail", "收件者郵箱(Email)格式不正確，請重新輸入!(參考格式:qdbzdt2846@gmail.com)");
                    }
                }
                else if (string.IsNullOrWhiteSpace(model.ResidentEmail) || model.ResidentEmail != "0")
                {
                    ModelState.AddModelError("ResidentEmail", "收件者郵箱(Email)不可為空白，請填寫!");
                }

                return View(model);
            }

            //假如有上傳圖檔，會存到img資料夾
            if (model.Repair.photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string path = _enviro.WebRootPath + "/img/" + photoName;
                model.Repair.photo.CopyTo(new FileStream(path, FileMode.Create));
                model.Repair.Pic = photoName;
            }
            //\\假如有上傳圖檔，會存到img資料夾

            // 处理 Detail 字段为空的情况
            if (string.IsNullOrWhiteSpace(model.Repair.Detail))
            {
                // 设置一个默认值或者为空字符串，以防止 GetHtmlcontent 方法抛出异常
                model.Repair.Detail = "";
            }


            DbHouseContext db = new DbHouseContext();
            db.Repairs.Add(model.Repair);
            db.SaveChanges();

            // 这里继续处理邮件发送和数据库保存的逻辑
            model.Time = DateTime.Now;

            //reapir存完換存email
            Email e = new Email()
            {
                HouseholdCode = model.Repair.HouseholdCode,
                Subject = model.Repair.Type,
                FromEmail = model.FromEmail,
                ToEmail = model.ResidentEmail,
                Time = model.Time,
                Body = model.Repair.Detail,
                HouseholdCodeNavigation = model.HouseholdCodeNavigation
            };
            _context.Emails.Add(e);
            await _context.SaveChangesAsync();

            try
            {
                //最後發送mail
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = model.ResidentEmail;
                mailrequest.Subject = model.Repair.Type;
                mailrequest.Body = GetHtmlcontent(model.Repair.Detail);

                await emailService.SendEmailAsync(mailrequest);
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("RepairInquire");
        }

        //寄信用格式for create
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

        //專門驗證input並回傳給blur事件的方法for Create
        [HttpGet]
        public IActionResult ValidateInput(CEmails model, string inputId, string inputValue)
        {
            bool isValid = true;
            string errorMessage = "";

            // 根據 inputId 來驗證 inputValue
            switch (inputId)
            {
                case "Name":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "姓名不可為空白或零，請填寫!";
                    }
                    break;
                case "HouseholdCode":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "戶號不可為空白或零，請填寫住家戶號!";
                    }
                    break;
                case "Phone":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "電話不可為空白或零，請填寫(參考格式:0972468159)";
                    }
                    break;
                case "Type":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "請選擇報修項目，此欄不能空白!";
                    }
                    break;
                case "Time":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "請選擇時間，此欄不能空白!";
                    }
                    break;
                case "ProcessingStatus":
                    if (string.IsNullOrWhiteSpace(inputValue) || inputValue == "0")
                    {
                        isValid = false;
                        errorMessage = "請選擇處理狀態，此欄不能空白!";
                    }
                    break;
                case "ResidentEmail":
                    if (string.IsNullOrWhiteSpace(inputValue))
                    {
                        isValid = false;
                        errorMessage = "收件者郵箱(Email)不可為空白，請填寫!";
                    }
                    else
                    {
                        // 如果電子郵件不為空，則進一步檢查是否包含 '@'，並且 '@' 前後都有字符
                        var atIndex = inputValue.IndexOf('@');
                        if (atIndex <= 0 || atIndex >= inputValue.Length - 1)
                        {
                            isValid = false;
                            errorMessage = "收件者郵箱(Email)格式不正確，請重新輸入!(參考格式:qdbzdt2846@gmail.com)";
                        }
                    }
                    break;
                    // 你可以根據需要添加更多的 case
            }

            // 返回驗證結果
            return Json(new { isValid = isValid, errorMessage = errorMessage });
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("RepairInquire");
            DbHouseContext db = new DbHouseContext();
            Repair repair = db.Repairs.FirstOrDefault(t => t.RepairCode == id);
            if (repair != null)
            {
                db.Repairs.Remove(repair);
                db.SaveChanges();
            }
            return RedirectToAction("RepairInquire");
        }

        //跨2個Model合併使用參考範例

        public IActionResult List()
        {
            DbHouseContext db = new DbHouseContext();
            IEnumerable<Repair> pa = from p in db.Repairs select p;

            RepairMerge viewModel = new RepairMerge
            {
                Repair = pa,
                Resident = db.Residents
            };
            return View(viewModel);
        }

        //\跨2個Model合併使用參考範例


       

        
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("RepairInquire");
            DbHouseContext db = new DbHouseContext();

            Repair repair = db.Repairs.FirstOrDefault(t => t.RepairCode == id);
            if (repair == null)
                return RedirectToAction("RepairInquire");

            //CProductWrap prodWp = new CProductWrap();
            //prodWp.product = repair;
            return View(repair);
        }
        [HttpPost]
        public IActionResult Edit(Repair repairIn)
        {
            DbHouseContext db = new DbHouseContext();
            Repair repairDb = db.Repairs.FirstOrDefault(t => t.RepairCode == repairIn.RepairCode);

            if (repairDb != null)
            {

                //假如有修改圖檔，會存到img資料夾
                if (repairIn.photo != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    string path = _enviro.WebRootPath + "/img/" + photoName;
                    repairIn.photo.CopyTo(new FileStream(path, FileMode.Create));
                    repairDb.Pic = photoName;
                }
                //\\假如有修改圖檔，會存到img資料夾

                repairDb.HouseholdCode = repairIn.HouseholdCode;
                repairDb.Name = repairIn.Name;
                repairDb.Phone = repairIn.Phone;
                repairDb.Type = repairIn.Type;
                repairDb.Time = repairIn.Time;
                repairDb.ProcessingStatus = repairIn.ProcessingStatus;
                repairDb.ManufacturerCode = repairIn.ManufacturerCode;
                repairDb.Detail = repairIn.Detail;
                repairDb.ProcessingStatusDetail = repairIn.ProcessingStatusDetail;

                db.SaveChanges();
            }
            return RedirectToAction("RepairInquire");
        }
    }
}
