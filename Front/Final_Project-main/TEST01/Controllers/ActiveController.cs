using CloudinaryDotNet;
using FifthGroup_front.Interfaces;
using FifthGroup_front.Models;
using FifthGroup_front.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.AccessControl;
using XAct;
using Application = FifthGroup_front.Models.Application;

namespace FifthGroup_front.Controllers
{

    public class ActiveController : Controller
    {

        private readonly IEmailService emailService;


        private IWebHostEnvironment _enviro = null;
        public ActiveController(IWebHostEnvironment img, IEmailService emailService)
        {
            _enviro = img;
            this.emailService = emailService;
        }

        /*查看所有活動 與 用戶的報名及查看*/
        public IActionResult ActiveList(int page = 1, int pageSize = 8, string searchType = "", DateTime? startDate = null, DateTime? endDate = null, DateTime? searchDate = null)
        {
            DbHouseContext db = new DbHouseContext();
            var query = db.Applications.Where(a => a.State == 2);
            //搜索
            if (searchDate == null)
            {
                query = db.Applications.Where(a =>a.DateEnd > DateTime.Today);
                if (!string.IsNullOrEmpty(searchType))
                {
                    query = query.Where(a => a.Activities == searchType);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(a => a.DateStart >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(a => a.DateEnd <= endDate.Value);
                }
            }
            else
            {
                query = db.Applications.Where(a => a.DateEnd >= searchDate && a.DateStart <= searchDate);
            }
            
            // 計算跳過項數
            int skip = (page - 1) * pageSize;
            IEnumerable<CActive> activities = query.OrderBy(a => a.DateStart).Select(a => new CActive
            {
                Applications = a,
                Amount = (from p in db.PaymentItems where p.PaymentItemCode == a.PaymentItemCode select p.Amount).FirstOrDefault()
            }).ToList();
            // 抓取當前數據
            

            ViewBag.TotalCount = query.Count(); // 總數量
            ViewBag.PageSize = pageSize; // 每頁數量
            ViewBag.searchType = searchType;
            ViewBag.searchDateStart = startDate;
            ViewBag.searchDateEnd = endDate;

            activities = activities.Skip(skip).Take(pageSize).ToList();
            return View(activities);
        }
        public IActionResult ActiveApply(int? id)
        {
            string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");
            if (string.IsNullOrEmpty(userHouseholdCode))
            {
                return RedirectToAction("UserLogin", "Resident"); // 查無此戶，回到登入
            }
            DbHouseContext db = new DbHouseContext();
            Application activedb = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
            if (activedb == null)
            {
                return RedirectToAction("ActiveList");
            }

            //匹配用戶有無報名過
            Registration existingRegistration = db.Registrations.FirstOrDefault(r => r.HouseholdCode == userHouseholdCode && r.ApplyCode == id);

            //場地資料代入
            var activePlaceName = (from rp in db.ReservationPlaces
                                   join pd in db.PublicSpaceDetails on rp.PlaceCode equals pd.PlaceCode
                                   where rp.ReserveCode == activedb.ReserveCode
                                   select pd.PlaceName).FirstOrDefault();
            var activeTime = (from rp in db.ReservationPlaces
                              join p in db.Periodoftimes on rp.PeriodoftimeCode equals p.PeriodoftimeCode
                              where rp.ReserveCode == activedb.ReserveCode
                              select p.Periodoftime1).FirstOrDefault();
            Payment pay = db.Payments.FirstOrDefault(p => p.PaymentItemCode == activedb.PaymentItemCode && p.HouseholdCode == userHouseholdCode);
            var ispaid = "";
            if (pay != null)
            {
                if (pay.Paid)
                {
                    ispaid = "已繳費";
                }
            }
            var activemoney = (from money in db.PaymentItems where money.PaymentItemCode == activedb.PaymentItemCode select money.Amount).FirstOrDefault();
            if (activemoney <= 0)
            {
                ViewBag.activemoney = "免費參加";

            }
            else
            {
                ViewBag.activemoney = activemoney + "/人";

            }
            var viewModel = new ActiveApplyViewModel
            {
                Application = activedb,
                Registration = existingRegistration ?? new Registration(),
                PlaceName = activePlaceName,
                Periodoftime1 = activeTime,
                isEditMode = (existingRegistration != null),
                isPaid = ispaid
        };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ActiveApply(ActiveApplyViewModel active_in)
        {
            if (active_in.ApplyCode == null)
            {
                return RedirectToAction("ActiveList");
            }

            DbHouseContext db = new DbHouseContext();
            Application activedb = db.Applications.FirstOrDefault(i => i.ApplyCode == active_in.ApplyCode);
            string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");
            Registration existingRegistration = db.Registrations.FirstOrDefault(r => r.HouseholdCode == userHouseholdCode && r.ApplyCode == active_in.ApplyCode);
            
            if (active_in.isEditMode)
            {
                //編輯模式，修改人數，先減掉已有再增加修改後
                if (activedb.Applicants - existingRegistration.NumberOfApplicants + active_in.NumberOfApplicants <= activedb.MaxApplicants)
                {
                    activedb.Applicants += active_in.NumberOfApplicants;
                    activedb.Applicants -= existingRegistration.NumberOfApplicants;

                    existingRegistration.NumberOfApplicants = active_in.NumberOfApplicants;
                    //更改人數要更改繳費金額
                    Payment p = db.Payments.FirstOrDefault(i => i.HouseholdCode == userHouseholdCode && i.PaymentItemCode == activedb.PaymentItemCode);
                    if (p != null)
                    {
                        p.Amount = (from i in db.PaymentItems where i.PaymentItemCode == activedb.PaymentItemCode select i.Amount).FirstOrDefault() * active_in.NumberOfApplicants;

                    }
                    db.SaveChanges();
                    return RedirectToAction("PersonalActive");
                }
                else
                {
                    ViewBag.ErrorMessage = "報名人數已達上限，無法再修改。";
                    return RedirectToAction("ActiveApply");
                }

            }
            else
            {
                if (activedb != null)
                {
                    int numberOfApplicants = active_in.NumberOfApplicants;
                    // 檢查是否已達到報名人數上限
                    if (activedb.Applicants + numberOfApplicants <= activedb.MaxApplicants)
                    {
                        activedb.Applicants += numberOfApplicants;

                        var registration = new Registration
                        {
                            HouseholdCode = HttpContext.Session.GetString("UserHouseholdCode"),
                            ApplyCode = activedb.ApplyCode,
                            NumberOfApplicants = numberOfApplicants
                        };

                        string DefMail = $"無聲鄰陸社區活動報名通知 {activedb.ActivityName}已成功報名，請留意相關資訊。 若有相關問題請聯絡主辦人:{activedb.Name}，{activedb.Phone}。 無聲鄰陸社區提醒您~";
                        var SubJ = activedb.ActivityName;
                        var userEmail = (from i in db.Residents where i.HouseholdCode == HttpContext.Session.GetString("UserHouseholdCode") select i.Email).FirstOrDefault();
                        var body = GetHtmlcontentforFree(activedb.ActivityName , activedb.Name , activedb.Phone);
                        //如免費活動不需新增繳費資料
                        if (activedb.PaymentItemCode != null)
                        {
                            Payment p = new Payment()
                            {
                                HouseholdCode = HttpContext.Session.GetString("UserHouseholdCode"),
                                Amount = (from i in db.PaymentItems where i.PaymentItemCode == activedb.PaymentItemCode select i.Amount).FirstOrDefault() * numberOfApplicants,
                                Paid = false,
                                PaymentItemCode = (int)activedb.PaymentItemCode,


                            };

                            db.Payments.Add(p);
                            PaymentItem PI = (from i in db.PaymentItems where i.PaymentItemCode == activedb.PaymentItemCode select i).FirstOrDefault();
                            DefMail = $"無聲鄰陸社區活動繳費通知 {PI.Remark}已成功報名，請盡速前往繳費。 繳費期限到{PI.Date.ToString("yyyy-MM-dd")}止，若逾期可能會影響住戶權益。 無聲鄰陸社區提醒您~";
                            SubJ = (from i in db.PaymentItems where i.PaymentItemCode == activedb.PaymentItemCode select i.Remark).FirstOrDefault();
                            body = GetHtmlcontent( PI.Remark , PI.Date);

                        }

                        Email e = new Email()
                        {
                            HouseholdCode = HttpContext.Session.GetString("UserHouseholdCode"),
                            Subject = SubJ,
                            FromEmail = "",
                            ToEmail = userEmail,
                            Time = DateTime.Now,
                            Body = DefMail,

                        };
                        db.Emails.Add(e);
                       

                        try
                        {
                            Mailrequest mailrequest = new Mailrequest();
                            mailrequest.ToEmail = userEmail;
                            mailrequest.Subject = SubJ;
                            mailrequest.Body = body;

                            await emailService.SendEmailAsync(mailrequest);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        db.Registrations.Add(registration);
                        db.SaveChanges();

                        return RedirectToAction("PersonalActive");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "報名人數已達上限，無法再報名。";
                        return RedirectToAction("ActiveApply");
                    }
                }
                return RedirectToAction("ActiveList");
            }
        }

        
        
        private string GetHtmlcontent(string name , DateTime date )
        {

            string response = "<h1>無聲鄰陸社區活動繳費通知</h1>";

            response += $"<h2>{name} 已經開放繳費，請盡速前往繳費。</h2>";
            response += $"<h2>繳費期限到{date.ToString("yyyy-MM-dd")}止，若逾期可能會影響住戶權益。</h2>";
            response += "<br>";
            response += "<h2>無聲鄰陸社區提醒您~</h2>";

            return response;
        }
        private string GetHtmlcontentforFree(string activename, string name,string phone)
        {
             
            string response = "<h1>無聲鄰陸社區活動報名通知</h1>";

            response += $"<h2>{activename} 已成功報名，請留意相關資訊。</h2>";
            response += $"<h2>若有活動上相關問題請聯絡主辦人: {name}，{phone}。</h2>";
            response += "<br>";
            response += "<h2>無聲鄰陸社區提醒您~</h2>";

            return response;
        }
        public IActionResult PersonalActive(string id = "true")
        {
            string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");

            if (string.IsNullOrEmpty(userHouseholdCode))
            {
                return RedirectToAction("UserLogin", "Resident"); //查無此戶，回到登入
            }
            DbHouseContext db = new DbHouseContext();
            IEnumerable<Registration> datas = null;
            if (id == "true")
            {
                datas = (from r in db.Registrations
                         join a in db.Applications on r.ApplyCode equals a.ApplyCode
                         where r.HouseholdCode == userHouseholdCode && a.DateEnd >= DateTime.Now
                         orderby a.DateStart
                         select r).ToList();
            }
            else
            {
                datas = (from r in db.Registrations
                         join a in db.Applications on r.ApplyCode equals a.ApplyCode
                         where r.HouseholdCode == userHouseholdCode && a.DateEnd < DateTime.Now
                         orderby a.DateStart descending
                         select r).ToList();
            }
            List<ActiveApplyViewModel> ActiveApplyView = new List<ActiveApplyViewModel>();

            foreach (var registration in datas)
            {
                Application application = db.Applications.FirstOrDefault(a => a.ApplyCode == registration.ApplyCode);
                Payment pay = db.Payments.FirstOrDefault(p => p.PaymentItemCode == application.PaymentItemCode && p.HouseholdCode == userHouseholdCode);

                string ispaid = "免費";
                if (pay != null)
                {
                    if (pay.Paid)
                    {
                        ispaid = "已完成繳費";
                    }
                    else
                    {
                        ispaid = "未繳" + pay.Amount + "元";
                    }
                }
                //場地資料代入
                var activePlaceName = (from rp in db.ReservationPlaces
                                       join pd in db.PublicSpaceDetails on rp.PlaceCode equals pd.PlaceCode
                                       where rp.ReserveCode == application.ReserveCode
                                       select pd.PlaceName).FirstOrDefault();
                var activeTime = (from rp in db.ReservationPlaces
                                  join pt in db.Periodoftimes on rp.PeriodoftimeCode equals pt.PeriodoftimeCode
                                  where rp.ReserveCode == application.ReserveCode
                                  select pt.Periodoftime1).FirstOrDefault();
                if (application != null)
                {
                    var activeapply = new ActiveApplyViewModel
                    {
                        Registration = registration,
                        Application = application,
                        ApplyCode = registration.ApplyCode,
                        NumberOfApplicants = registration.NumberOfApplicants,
                        PlaceName = activePlaceName,
                        Periodoftime1 = activeTime,
                        isPaid = ispaid
                    };
                    ActiveApplyView.Add(activeapply);
                }
            }
            return View(ActiveApplyView);
        }

        public async Task<IActionResult> DeleteApply(int? id)
        {
            if (id == null)
                return RedirectToAction("PersonalActive");
            DbHouseContext db = new DbHouseContext();
            Registration apply = db.Registrations.FirstOrDefault(i => i.ApplyCode == id);
            Application app = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
            Payment pay = db.Payments.FirstOrDefault(i => i.PaymentItemCode == app.PaymentItemCode && i.HouseholdCode == apply.HouseholdCode);
            if (apply != null && app != null)
            {
                app.Applicants -= apply.NumberOfApplicants;
                db.Registrations.Remove(apply);
                if (pay != null)
                {
                    db.Payments.Remove(pay);
                }
                string userEmail = (from i in db.Residents where i.HouseholdCode == HttpContext.Session.GetString("UserHouseholdCode") select i.Email).FirstOrDefault();
                Email e = new Email()
                {
                    HouseholdCode = HttpContext.Session.GetString("UserHouseholdCode"),
                    Subject = app.ActivityName,
                    FromEmail = "",
                    ToEmail = userEmail,
                    Time = DateTime.Now,
                    Body = $"無聲鄰陸社區活動取消通知 您所報名的 {app.ActivityName} 已成功取消。"
                };
                db.Emails.Add(e);
                try
                {
                    Mailrequest mailrequest = new Mailrequest();
                    mailrequest.ToEmail = userEmail;
                    mailrequest.Subject = app.ActivityName;
                    mailrequest.Body = $"無聲鄰陸社區活動取消通知 您所報名的 {app.ActivityName} 已成功取消。";
                    await emailService.SendEmailAsync(mailrequest);
                }
                catch (Exception ex)
                {
                    throw;
                }

                db.SaveChanges();
            }
            return RedirectToAction("PersonalActive");
        }
        /*用戶個人申請活動的查看*/
        public IActionResult CheckActive()
        {
            string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");
            if (string.IsNullOrEmpty(userHouseholdCode))
            {
                return RedirectToAction("UserLogin", "Resident"); //查無此戶，回到登入
            }
            DbHouseContext db = new DbHouseContext();

            IEnumerable<ActiveApplyViewModel> applicationVM = db.Applications.Where(i => i.HouseholdCode == userHouseholdCode)
            .OrderByDescending(i => i.State).ThenBy(i => i.DateStart)
            .Select(a => new ActiveApplyViewModel
            {
                ApplyCode = a.ApplyCode,
                Application = a,
                PlaceName = ( from rp in db.ReservationPlaces join pd in db.PublicSpaceDetails
                              on rp.PlaceCode equals pd.PlaceCode
                              where rp.ReserveCode == a.ReserveCode select pd.PlaceName).FirstOrDefault(),
                Periodoftime1 = ( from rp in db.ReservationPlaces join p in db.Periodoftimes 
                                  on rp.PeriodoftimeCode equals p.PeriodoftimeCode 
                                  where rp.ReserveCode == a.ReserveCode select p.Periodoftime1).FirstOrDefault()
            }).ToList();
            return View(applicationVM);

        }
        public IActionResult DetailActive(int? id)
        {
            string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");

            DbHouseContext db = new DbHouseContext();
            Application app = db.Applications.FirstOrDefault(i => i.ApplyCode == id);

            IEnumerable<ActiveFromResident> ActfromRes = db.Registrations.Where(i => i.ApplyCode == id).Select(
                r => new ActiveFromResident
                {
                    Resident = (from rs in db.Residents where rs.HouseholdCode == r.HouseholdCode select rs).FirstOrDefault(),
                    Registration = r,
                    isPaid = (from pay in db.Payments where pay.HouseholdCode == userHouseholdCode && pay.PaymentItemCode == app.PaymentItemCode select pay.Paid).FirstOrDefault()
                }).ToList();

            ViewBag.ActiveName = app.ActivityName;
            return View(ActfromRes);

        }
        /*活動的新增、修改、刪除 */
        public IActionResult AddActive()
        {
            string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");
            string userName = HttpContext.Session.GetString("UserName");
            string userPhone = HttpContext.Session.GetString("UserPhone");
            string userEmail = HttpContext.Session.GetString("UserEmail");
            
            DbHouseContext db = new DbHouseContext();

            var app = new Application()
            {
                HouseholdCode = userHouseholdCode,
                Name = userName,
                Phone = userPhone,
                Email = userEmail
            };

            var model = new CActive()
            {
                Applications = app,
                Reservations = new Reservation(),
                ReservationPlace = new ReservationPlace(),
                PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
                Periodoftime = db.Periodoftimes.ToList()
            };



            return View(model);
        }
        [HttpPost]
        public IActionResult AddActive(CActive i)
        {
                using (DbHouseContext db = new DbHouseContext())
                {
                // 檢查是否已存在相同的場地預約
                //var existingReservation = db.ReservationPlaces.FirstOrDefault(rp =>
                //    (rp.FDate >= i.Applications.DateStart && rp.FDate <= i.Applications.DateEnd) ||
                //    (i.Applications.DateStart <= rp.FDateEnd && i.Applications.DateEnd >= rp.FDateStart) &&
                //    rp.PlaceCode == i.PublicSpaceDetail[0].PlaceCode &&
                //    rp.PeriodoftimeCode == i.Periodoftime[0].PeriodoftimeCode);

                ////////////////////////////////////////////////////
                var existingReservations = (from rp in db.ReservationPlaces
                                            join pt in db.Periodoftimes on rp.PeriodoftimeCode equals pt.PeriodoftimeCode
                                            where (rp.FDate >= i.Applications.DateStart && rp.FDate <= i.Applications.DateEnd) ||
                                                  (i.Applications.DateStart <= rp.FDateEnd && i.Applications.DateEnd >= rp.FDateStart) &&
                                                  rp.PlaceCode == i.PublicSpaceDetail[0].PlaceCode 
                                            select pt.Periodoftime1).ToList();

                var periodTime = (from pt in db.Periodoftimes
                                  where pt.PeriodoftimeCode == i.Periodoftime[0].PeriodoftimeCode
                                  select pt.Periodoftime1).FirstOrDefault();

                string timeString = periodTime;

                string[] timeParts = timeString.Split('~'); //0800 - 1200  把 - 當作分隔符號 存入timeParts
                if (timeParts.Length == 2)
                {
                    string startTime = timeParts[0];
                    string endTime = timeParts[1];

                    TimeSpan startTimeSpan = TimeSpan.ParseExact(startTime, "hh':'mm", CultureInfo.InvariantCulture);
                    TimeSpan endTimeSpan = TimeSpan.ParseExact(endTime, "hh':'mm", CultureInfo.InvariantCulture);
                    
                    for (int j = 0; j < existingReservations.Count; j++)
                    {
                        string time = existingReservations[j];
                        string[] timeP = timeString.Split('~');
                        if (timeP.Length == 2)
                        {
                            string startT = timeP[0];
                            string endT = timeP[1];

                            TimeSpan startTSpan = TimeSpan.ParseExact(startT, "hh':'mm", CultureInfo.InvariantCulture);
                            TimeSpan endTSpan = TimeSpan.ParseExact(endT, "hh':'mm", CultureInfo.InvariantCulture);

                            if (startTimeSpan <= endTSpan || endTimeSpan >= startTSpan)
                            {
                                ModelState.AddModelError(string.Empty, "預約已滿");
                                var viewModel = new CActive
                                {
                                    Applications = i.Applications,
                                    Reservations = i.Reservations,
                                    ReservationPlace = i.ReservationPlace,
                                    PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
                                    Periodoftime = db.Periodoftimes.ToList()
                                };
                                return View(viewModel);
                            }
                        }
                    }
                }

                // 使用新的 ReserveCode 創建 Reservation 資料
                var reservation = new Reservation
                {
                    HouseholdCode = i.Applications.HouseholdCode,
                    HName = i.Applications.Name,
                    HPhone = i.Applications.Phone,
                    HEmail = i.Applications.Email,
                    HOrdertime = DateTime.Now,
                    HState = 0
                };
                db.Reservations.Add(reservation);

                //創造活動費用
                var CommunityBuildingId = (from r in db.Residents where r.HouseholdCode == i.Applications.HouseholdCode select r.CommunityBuildingId).FirstOrDefault();
                var CommunityId = (from c in db.CommunityBuildings where c.CommunityBuildingId == CommunityBuildingId select c.CommunityId).FirstOrDefault();
                var forpaymentID = (from c in db.CommunityBuildings where c.CommunityId == CommunityId && c.BuildingName == "管理室" select c.CommunityBuildingId).FirstOrDefault();
                var pay = new PaymentItem
                {
                    CommunityBuildingId = forpaymentID,
                    ItemClassificationCode = 3,
                    PaymentName = "其他",
                    Date = i.Applications.DateStart.AddDays(-1),
                    Amount = i.Amount,
                    Ispushed = false,
                    Remark = i.Applications.ActivityName
                };
                db.PaymentItems.Add(pay);
                db.SaveChanges();

                // 讀取 Reservation 及 PaymentItem 中新生成的 ReserveCode 及 PaymentItemCode
                var newReserveCode = reservation.ReserveCode;
                var newPaymentItemCode = pay.PaymentItemCode;

                // 將 Application 存入資料庫，但將 Applicants = 0，並新增 ReserveCode 及 PaymentItemCode
                i.Applications.Applicants = 0;
                i.Applications.ReserveCode = newReserveCode;
                i.Applications.PaymentItemCode = newPaymentItemCode;
                db.Applications.Add(i.Applications);

                var reservationplace = new ReservationPlace
                {
                    ReserveCode = newReserveCode,
                    PlaceCode = i.PublicSpaceDetail[0].PlaceCode,
                    PeriodoftimeCode = i.Periodoftime[0].PeriodoftimeCode,
                    FState = 0,
                    FDateStart = i.Applications.DateStart,
                    FDateEnd = i.Applications.DateEnd
                };
                db.ReservationPlaces.Add(reservationplace);
                db.SaveChanges();
                return RedirectToAction("CheckActive");
            }
        }
        public IActionResult EditActive(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("CheckActive");
            }
            DbHouseContext db = new DbHouseContext();
            Application active = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
            ReservationPlace place = db.ReservationPlaces.Where(i => i.ReserveCode == active.ReserveCode).FirstOrDefault();
            if (active == null)
            {
                return RedirectToAction("CheckActive");
            }
            CActive active_edit = new CActive
            {
                Applications = active,
                ReservationPlace = place,
                PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
                Periodoftime = db.Periodoftimes.ToList()
            };


            return View(active_edit);
        }
        [HttpPost]
        public IActionResult EditActive(CActive active_edit)
        {
            DbHouseContext db = new DbHouseContext();
            Application activedb = db.Applications.FirstOrDefault(i => i.ApplyCode == active_edit.Applications.ApplyCode);
            ReservationPlace placedb = db.ReservationPlaces.FirstOrDefault(i => i.ReserveCode == activedb.ReserveCode);

            if (activedb != null)
            {
                if (active_edit.photo != null)
                {
                    string originalPhotoName = Path.GetFileNameWithoutExtension(active_edit.photo.FileName); //取得原本檔案名
                    string newPhotoName = $"{originalPhotoName.Split('_')[0]}_{DateTime.Now:yyyyMMddHHmmss}.jpg"; //原檔名+現在時間 之後要改會把時間刪除並做更新
                    string path = _enviro.WebRootPath + "/img/" + newPhotoName;
                    try
                    {
                        // 刪除舊照片（如果存在）
                        if (!string.IsNullOrEmpty(activedb.Image))
                        {
                            string oldFilePath = Path.Combine(_enviro.WebRootPath + "/img/", activedb.Image);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        // 保存新照片，維持原尺寸
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            active_edit.photo.CopyTo(fileStream);
                        }

                        activedb.Image = newPhotoName;
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "更新照片時發生錯誤 \n請嘗試別組照片!";
                        return RedirectToAction("EditActive", new { id = active_edit.Applications.ApplyCode });

                    }
                }
                activedb.ActivityName = active_edit.Applications.ActivityName;
                activedb.Introduce = active_edit.Applications.Introduce;
                activedb.MaxApplicants = active_edit.Applications.MaxApplicants;
                if (active_edit.Applications.Activities == null)
                {
                    activedb.MaxApplicants = active_edit.Applications.MaxApplicants;

                }
                else
                {
                    activedb.Activities = active_edit.Applications.Activities;
                    activedb.DateStart = active_edit.Applications.DateStart;
                    activedb.DateEnd = active_edit.Applications.DateEnd;
                    placedb.PlaceCode = active_edit.ReservationPlace.PlaceCode;
                    placedb.PeriodoftimeCode = active_edit.ReservationPlace.PeriodoftimeCode;
                    placedb.FDateStart = active_edit.Applications.DateStart;
                    placedb.FDateEnd = active_edit.Applications.DateEnd;
                }
                
                db.SaveChanges();
            }
            return RedirectToAction("CheckActive");
        }
        public IActionResult DeleteActive(int? id)
        {
            if (id == null)
                return RedirectToAction("CheckActive");
            DbHouseContext db = new DbHouseContext();
            Application active = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
            Reservation reserve = db.Reservations.FirstOrDefault(i => i.ReserveCode == active.ReserveCode);
            ReservationPlace place = db.ReservationPlaces.FirstOrDefault(i => i.ReserveCode == active.ReserveCode);
            PaymentItem pay = db.PaymentItems.FirstOrDefault(i => i.PaymentItemCode == active.PaymentItemCode);
            if (active != null)
            {
                db.Applications.Remove(active);
                db.ReservationPlaces.Remove(place);
                db.Reservations.Remove(reserve);
                if (pay != null)
                {
                    db.PaymentItems.Remove(pay);
                }

                db.SaveChanges();
            }
            return RedirectToAction("CheckActive");
        }
        /*活動的新增、修改、刪除 */
    }
}
