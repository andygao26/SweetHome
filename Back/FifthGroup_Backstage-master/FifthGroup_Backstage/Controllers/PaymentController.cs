using FifthGroup_Backstage.Interfaces;
using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.Repositories;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Globalization;
using X.PagedList;

namespace FifthGroup_Backstage.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DbHouseContext dbHouseContext;
        private readonly IEmailService emailService2;

        public PaymentController(DbHouseContext dbHouseContext,EmailService emailService, IEmailService emailService2)
        {
            this.dbHouseContext = dbHouseContext;
            this.emailService2 = emailService2;
        }
        public IActionResult List(int? page, int? buildingId = null, DateTime? startDate = null, DateTime? endDate = null, bool? status =null)
        {
            // 管理者ID
            string userID = HttpContext.Session.GetString("UserID");
            
            using (DbHouseContext context = new DbHouseContext())
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                // 找管理者所在社區ID
                var communityId = context.Admins
                    .Where(a => a.UserId == userID)
                    .Select(a => a.CommunityId)
                    .FirstOrDefault();

                // 查询所有PaymentItems，但根据communityId筛选数据
                IQueryable<PaymentItem> pa = from p in context.PaymentItems
                                             join b in context.CommunityBuildings on p.CommunityBuildingId equals b.CommunityBuildingId
                                             where b.CommunityId == communityId
                                             orderby p.PaymentItemCode descending
                                             select p;

                // 如果提供了楼栋ID参数，则进行进一步筛选
                if (buildingId.HasValue)
                {
                    pa = pa.Where(p => p.CommunityBuildingId == buildingId);
                }

                // 如果提供了日期范围参数，则进行日期范围筛选
                if (startDate.HasValue && endDate.HasValue)
                {
                    DateTime parsedStartDate = startDate.Value;
                    DateTime parsedEndDate = endDate.Value;
               
                    ViewBag.StartDate = parsedStartDate.ToString("yyyy-MM-dd");
                    ViewBag.EndDate = parsedEndDate.ToString("yyyy-MM-dd");

                    pa = pa.Where(p => p.Date >= startDate && p.Date <= endDate);
                }

                // 如果提供了推送状态参数，则进行筛选
                if (status.HasValue)
                {
                    pa = pa.Where(p => p.Ispushed == status.Value);
                    ViewBag.Status = status.Value ? "true" : "false";
                }

                // 在这里设置ViewBag.BuildingId以便在视图中使用
                ViewBag.BuildingId = buildingId;
                //ViewBag.StartDate = startDate;
                //ViewBag.EndDate = endDate;
                

                // 只选择与当前用户关联的社区的建筑物
                var buildings = context.CommunityBuildings
                    .Where(b => b.CommunityId == communityId)
                    .Select(b => new SelectListItem
                    {
                        Value = b.CommunityBuildingId.ToString(),
                        Text = b.BuildingName
                    }).ToList();

                PaymentListViewModel viewModel = new PaymentListViewModel
                {
                    PagedPaymentItems = pa.ToPagedList(pageNumber, pageSize),
                    CommunityBuilding = context.CommunityBuildings.Where(b => b.CommunityId == communityId).ToList(),
                    Buildings = buildings
                };
                return View(viewModel);
            }
        }
        //public IActionResult Push(int? id)
        //{
        //    if (id == null)
        //        return RedirectToAction("List");


        //    DbHouseContext context = new DbHouseContext();
        //    PaymentItem pa = context.PaymentItems.FirstOrDefault(I => I.PaymentItemCode == id);
        //    PaymentViewModel viewModel = new PaymentViewModel
        //    {
        //        PaymentItem = pa,
        //        CommunityBuilding = context.CommunityBuildings,
        //        PaymentItemsName = context.PaymentItemsNames
        //    };
        //    return View(viewModel);
        //}

        //[HttpPost]
        //public IActionResult Push(PaymentItem pa)
        //{
        //    DbHouseContext context = new DbHouseContext();

        //    IEnumerable < Resident> residents = from r in context.Residents where r.CommunityBuildingId == pa.CommunityBuildingId select r;
        //    PaymentItem Payitem = (from r in context.PaymentItems where r.PaymentItemCode ==  pa.PaymentItemCode select r).FirstOrDefault();

        //    foreach (Resident ritem in residents)
        //    {
        //        Payment p = new Payment();
        //        p.PayDay = DateTime.Now;
        //        p.HouseholdCode = ritem.HouseholdCode;
        //        p.Amount=Payitem.Amount;
        //        p.Paid = false;
        //        context.Payments.Add(p);
        //        context.SaveChanges();
        //    }
        //    Payitem.Ispushed = true;
        //    context.SaveChanges();

        //    return RedirectToAction("List");
        //}
        public async Task<IActionResult> Push(int? id)
        {
            DbHouseContext context = new DbHouseContext();

            var CBid = (from r in context.PaymentItems where r.PaymentItemCode == id select r.CommunityBuildingId).FirstOrDefault();
            IEnumerable<Resident> residents = from r in context.Residents where r.CommunityBuildingId == CBid select r;
            PaymentItem Payitem = (from r in context.PaymentItems where r.PaymentItemCode == id select r).FirstOrDefault();
            if (residents != null && Payitem != null)
            {
                foreach (Resident ritem in residents)
                {
                    Payment p = new Payment();

                    p.HouseholdCode = ritem.HouseholdCode;
                    p.Amount = Payitem.Amount;
                    p.Paid = false;
                    p.PaymentItemCode = Payitem.PaymentItemCode;
                    context.Payments.Add(p);

                    string DefMail = $"無聲鄰陸社區繳費通知 {Payitem.Remark}已經開放繳費，請盡速前往繳費。 繳費期限到{Payitem.Date.ToString("yyyy-MM-dd")}止，若逾期可能會影響住戶權益。 無聲鄰陸社區提醒您~";
                    Email e = new Email()
                    {
                        HouseholdCode = ritem.HouseholdCode,
                        Subject = Payitem.PaymentName,
                        FromEmail = "qdbzdt2846@gmail.com",
                        ToEmail = ritem.Email,
                        Time = DateTime.Now,
                        Body = DefMail,

                    };
                    context.Emails.Add(e);
  

                    try
                    {
                        Mailrequest mailrequest = new Mailrequest();
                        mailrequest.ToEmail = ritem.Email;
                        mailrequest.Subject = Payitem.PaymentName;
                        mailrequest.Body = GetHtmlcontent(Payitem);

                        await emailService2.SendEmailAsync(mailrequest);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }






                    Payitem.Ispushed = true;
                    
                }

            }
                context.SaveChanges(); 
            return RedirectToAction("List");
          
        }
        private string GetHtmlcontent(PaymentItem p)
        {

            string response = "<h1>無聲鄰陸社區繳費通知</h1>";

            response += $"<h2>{p.Remark} 已經開放繳費，請盡速前往繳費。</h2>";
            response += $"<h2>繳費期限到{p.Date.ToString("yyyy-MM-dd")}止，若逾期可能會影響住戶權益。</h2>";
            response += "<br>";
            response += "<h2>無聲鄰陸社區提醒您~</h2>";

            return response;
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            DbHouseContext context = new DbHouseContext();

            PaymentItem paymentItem = context.PaymentItems.FirstOrDefault(t => t.PaymentItemCode == id);

            if (paymentItem != null)
            {
                context.PaymentItems.Remove(paymentItem);
                context.SaveChanges();
            }


            return RedirectToAction("List");
        }

        public IActionResult Create()
        {
            DbHouseContext context = new DbHouseContext();

            string userID = HttpContext.Session.GetString("UserID");

            var communityId = context.Admins
                   .Where(a => a.UserId == userID)
                   .Select(a => a.CommunityId)
                   .FirstOrDefault();

            PaymentViewModel viewModel = new PaymentViewModel
            {
                PaymentItemsName = context.PaymentItemsNames,
                PaymentItem = new PaymentItem(),
                CommunityBuilding = from i in context.CommunityBuildings where i.CommunityId == communityId select i
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Create(PaymentViewModel model)
        {

            DbHouseContext context = new DbHouseContext();


           
            if (model != null)
                {
                     model.PaymentItem.ItemClassificationCode = (from p in context.PaymentItemsNames where model.PaymentItem.PaymentName == p.Name select p.ItemClassificationCode).FirstOrDefault();
                     model.PaymentItem.Ispushed = false;
                     context.PaymentItems.Add(model.PaymentItem);
                     context.SaveChanges();  
                     return RedirectToAction("List"); // 或其他适当的操作
                
            }


            // 验证失败，返回视图并显示错误信息
            return View("List");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");


            DbHouseContext context = new DbHouseContext();
            PaymentItem pa = context.PaymentItems.FirstOrDefault(I => I.PaymentItemCode == id);

            PaymentViewModel viewModel = new PaymentViewModel
            {
                PaymentItemsName = context.PaymentItemsNames,
                PaymentItem = pa,
                CommunityBuilding = context.CommunityBuildings
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(PaymentViewModel vm)
        {

            DbHouseContext context = new DbHouseContext();
            PaymentItem pa = context.PaymentItems.FirstOrDefault(I => I.PaymentItemCode == vm.PaymentItem.PaymentItemCode);

            if (pa != null)
            {
                pa.PaymentName = vm.PaymentItem.PaymentName;
                
                pa.Remark = vm.PaymentItem.Remark;
                pa.Amount = vm.PaymentItem.Amount;
                pa.Date = vm.PaymentItem.Date;
                pa.CommunityBuildingId = vm.PaymentItem.CommunityBuildingId;
                pa.Ispushed = vm.PaymentItem.Ispushed;
                context.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
                return RedirectToAction("List");


            DbHouseContext context = new DbHouseContext();
            IEnumerable<Payment> pa = from i in context.Payments where i.PaymentItemCode == id select i;
            PaymentItem PI = (from i in context.PaymentItems where i.PaymentItemCode == id select i).FirstOrDefault();

         

            PaymentDetailViewModel viewModel = new PaymentDetailViewModel
            {
                Payment = pa,
                PaymentItem = context.PaymentItems.FirstOrDefault(I => I.PaymentItemCode == id),
                CommunityBuilding = context.CommunityBuildings.FirstOrDefault(I => I.CommunityBuildingId == PI.CommunityBuildingId),
            };
            return View(viewModel);
        }
        //[HttpPost]
        //public IActionResult Detail(PaymentItem vm)
        //{

        //    IamgayContext context = new IamgayContext();
        //    PaymentItem pa = context.PaymentItems.FirstOrDefault(I => I.PaymentItemCode == vm.PaymentItemCode);

        //    if (pa != null)
        //    {
        //        pa.PaymentName = vm.PaymentName;
        //        pa.PaymentItemCode = vm.PaymentItemCode;
        //        pa.Remark = vm.Remark;
        //        pa.Amount = vm.Amount;
        //        pa.Date = vm.Date;
        //        pa.CommunityBuildingId = vm.CommunityBuildingId;
        //        context.SaveChanges();
        //    }
        //    return RedirectToAction("List");
        //}



        //[System.Web.Http.HttpPost]
        //[System.Web.Http.Route("Payment/AddOrders")]
        //public string AddOrders(get_localStorage json)
        //{
        //    DbHouseContext db = new DbHouseContext();

        //    string num = "0";
        //    try
        //    {
        //        EcpayOrder Orders = new EcpayOrder();
        //        Orders.MemberId = json.MerchantID;
        //        Orders.MerchantTradeNo = json.MerchantTradeNo;
        //        Orders.RtnCode = 0; //未付款
        //        Orders.RtnMsg = "訂單成功尚未付款";
        //        Orders.TradeNo = json.MerchantID.ToString();
        //        Orders.TradeAmt = json.TotalAmount;
        //        Orders.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
        //        Orders.PaymentType = json.PaymentType;
        //        Orders.PaymentTypeChargeFee = "0";
        //        Orders.TradeDate = json.MerchantTradeDate;
        //        Orders.SimulatePaid = 0;
        //        db.EcpayOrders.Add(Orders);
        //        db.SaveChanges();
        //        num = "OK";
        //    }
        //    catch (Exception ex)
        //    {
        //        num = ex.ToString();
        //    }
        //    return num;
        //}



        //----------------------------*****催繳Start*****----------------------------//
        //顯示未付款住戶清單
        public IActionResult UnpaidResidentsList(int page = 1, int pageSize = 10, int? buildingId = null)
        {

            // 管理者ID
            string userID = HttpContext.Session.GetString("UserID");

            using (DbHouseContext context = new DbHouseContext())
            {

                // 找管理者所在社區ID
                var communityId = context.Admins
                    .Where(a => a.UserId == userID)
                    .Select(a => a.CommunityId)
                    .FirstOrDefault();

                //取得現在時間
                var currentDate = DateTime.Now;

                //查詢資料
                var unpaidResidents = from p in dbHouseContext.Payments
                                  join pi in dbHouseContext.PaymentItems on p.PaymentItemCode equals pi.PaymentItemCode
                                  join pin in dbHouseContext.PaymentItemsNames on pi.ItemClassificationCode equals pin.ItemClassificationCode
                                  where currentDate > pi.Date && p.Paid == false
                                  select new UnpaidResidentsViewModel
                                  {
                                      HouseholdCode = p.HouseholdCode,
                                      PaymentCategory = pin.Name,
                                      ExpiredDay = pi.Date.ToString("yyyy-MM-dd"),

                                      Amount = p.Amount.ToString("C0", new CultureInfo("zh-TW"))
                                  };

                // 使用PageList
                var pagedUnpaidResidents = unpaidResidents.ToPagedList(page, pageSize);

                return View(pagedUnpaidResidents);

            }


             

        }

        public string GetResidentEmail(string householdCode)
        {
            var resident = dbHouseContext.Residents.FirstOrDefault(r => r.HouseholdCode == householdCode);
            if (resident != null)
            {
                return resident.Email;
            }
            else
            {
                
                return null;
            }
        }

        //寄送催繳信件(單一住戶)
        public IActionResult SendPaymentReminder(string householdCode)
        {
            // 取得HouseholdCode的電子信箱
            string recipientEmail = GetResidentEmail(householdCode);

            if (recipientEmail != null)
            {
                // 創建郵件主體和正文
                string subject = "【無聲鄰睦帳款逾期通知】";
                string body = "尊敬的住戶，您有逾期的帳單尚未繳納，請盡快付款！以避免影響居住權益";

                // 使用 EmailService發送郵件
                EmailService emailService = new EmailService();
                emailService.SendMailByGmail(new List<string> { recipientEmail }, subject, body);

                // 返回催繳清單頁面

                return RedirectToAction("UnpaidResidentsList");
            }
            else
            {
                // 未找到電子信箱的情況
                return RedirectToAction("Error");
            }
        }


        //全體發送
        public IActionResult SendPaymentRemindersToAll()
        {
            var currentDate = DateTime.Now;

            var unpaidResidents = from p in dbHouseContext.Payments
                                  join pi in dbHouseContext.PaymentItems on p.PaymentItemCode equals pi.PaymentItemCode
                                  join pin in dbHouseContext.PaymentItemsNames on pi.ItemClassificationCode equals pin.ItemClassificationCode
                                  where currentDate > pi.Date && p.Paid == false
                                  select new UnpaidResidentsViewModel
                                  {
                                      HouseholdCode = p.HouseholdCode,
                                      PaymentCategory = pin.Name,
                                      ExpiredDay = pi.Date.ToString("yyyy-MM-dd"),
                                      Amount = p.Amount.ToString("C0", new CultureInfo("zh-TW"))
                                  };

            var unpaidResidentsList = unpaidResidents.ToList();

            foreach (var unpaidResident in unpaidResidentsList)
            {
                string recipientEmail = GetResidentEmail(unpaidResident.HouseholdCode);

                if (recipientEmail != null)
                {
                    string subject = "【無聲鄰睦帳款逾期通知】";
                    string body = "尊敬的住戶，您有尚未付款的帳單，請盡快付款！";

                    EmailService emailService = new EmailService();
                    emailService.SendMailByGmail(new List<string> { recipientEmail }, subject, body);
                }
            }

            return RedirectToAction("UnpaidResidentsList");
        }


        private Dictionary<string, bool> remindedResidents = new Dictionary<string, bool>();

        //----------------------------*****催繳End*****----------------------------//





        //----------------------------*****財務報表Start*****----------------------------//

        public IActionResult GenerateReport(int page = 1, int pageSize = 10, int? communityBuildingId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // 获取当前登录管理员的 ID
            string userID = HttpContext.Session.GetString("UserID");

            // 使用社区管理员的 ID 查找相关社区 ID
            var communityId = dbHouseContext.Admins
                .Where(a => a.UserId == userID)
                .Select(a => a.CommunityId)
                .FirstOrDefault();

            // 查询 PaymentItems 数据
            var query = dbHouseContext.PaymentItems
                .Where(pi => pi.Ispushed && pi.CommunityBuilding.Community.CommunityId == communityId);

            // 如果提供了Community Building ID参数，则进行进一步筛选
            if (communityBuildingId.HasValue)
            {
                query = query.Where(pi => pi.CommunityBuildingId == communityBuildingId);
            }

            // 如果提供了日期范围参数，则进行日期范围筛选
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(pi => pi.Date >= startDate && pi.Date <= endDate);
            }




            // 在这里获取筛选后的数据
            var financialReport = query
             .GroupBy(pi => new
             {
                 pi.PaymentItemCode,
                 pi.PaymentName,
                 pi.CommunityBuilding.BuildingName,
                 pi.Date // 包括PaymentItemDate
             })
             .Select(grouped => new
             {
                 PaymentItemCode = grouped.Key.PaymentItemCode,
                 PaymentName = grouped.Key.PaymentName,
                 CommunityBuildingName = grouped.Key.BuildingName,
                 TotalAmountToReceive = grouped.Sum(x => x.Amount),
                 Payments = grouped.Select(x => x.Payments).ToList(), // 收集 Payments
                 PaymentItemDate = grouped.Key.Date // 包括 PaymentItemDate
             })
             .ToList();

            // 计算 TotalAmountReceived 和设置 PaymentItemDate
            var finalFinancialReport = financialReport
         .Select(item => new FinancialReport
         {
             PaymentItemCode = item.PaymentItemCode,
             PaymentName = item.PaymentName,
             CommunityBuildingName = item.CommunityBuildingName,
             TotalAmountToReceive = item.TotalAmountToReceive,
             TotalAmountReceived = item.Payments.Sum(p => p.Where(subp => subp.Paid).Sum(subp => subp.Amount)),
             PaymentItemDate = item.PaymentItemDate // 设置PaymentItemDate
         })
         .ToList();



            var pagedFinancialReport = finalFinancialReport.ToPagedList(page, pageSize);

            return View(pagedFinancialReport);
        }

        //----------------------------*****財務報表End*****----------------------------//





    }
}
