using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.ViewModel;
using FifthGroup_front.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Security.Policy;

namespace FifthGroup_Backstage.Controllers
{
    public class ActiveController : Controller
    {
        private IWebHostEnvironment _enviro = null;
        public ActiveController(IWebHostEnvironment img)
        {
            _enviro = img;
        }
        //用戶活動
        public IActionResult ActiveList(int page = 1, int pageSize = 10, bool isNow = true ,string searchType="", DateTime? startDate = null, DateTime? endDate = null)
        {
            DbHouseContext db = new DbHouseContext();
            //搜索
            var query = db.Applications.Where(ap => ap.State == 2);
            if (isNow)
            {
                query = query.Where(a => a.DateEnd >= DateTime.Now);
                ViewBag.isNow = true;
            }
            else
            {
                query = query.Where(a => a.DateEnd < DateTime.Now);
                ViewBag.isNow = false;
            }
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

            int skip = (page - 1) * pageSize;
            IEnumerable<Application> activities = query.OrderBy(ap => ap.DateStart).ToList(); 

            ViewBag.TotalCount = query.Count(); // 總數量
            ViewBag.PageSize = pageSize; // 每頁數量
            ViewBag.searchType = searchType;
            ViewBag.searchDateStart = startDate;
            ViewBag.searchDateEnd = endDate;

            activities = activities.Skip(skip).Take(pageSize).ToList();
            return View(activities);
        }
        public IActionResult ActiveCheck()
        {
            DbHouseContext db = new DbHouseContext();
            IEnumerable<CActive> activeVM = db.Applications.Where(i => i.State != 2)
            .OrderByDescending(i => i.State).ThenBy(i => i.DateStart)
            .Select(a => new CActive
            {
                Applications = a,
                Reservations = (from re in db.Reservations where re.ReserveCode == a.ReserveCode select re).FirstOrDefault(),
                ReservationPlace = (from rp in db.ReservationPlaces where rp.ReserveCode == a.ReserveCode select rp).FirstOrDefault(),

                PlaceName = (from rp in db.ReservationPlaces
                             join pd in db.PublicSpaceDetails on rp.PlaceCode equals pd.PlaceCode
                             where rp.ReserveCode == a.ReserveCode select pd.PlaceName).FirstOrDefault(),
                Periodoftime1 = (from rp in db.ReservationPlaces
                                 join p in db.Periodoftimes on rp.PeriodoftimeCode equals p.PeriodoftimeCode
                                 where rp.ReserveCode == a.ReserveCode select p.Periodoftime1).FirstOrDefault()
            }).ToList();
            return View(activeVM);
        }
        [HttpPost]
        public IActionResult ActiveCheck(List<int> ids, int state)
        {
            DbHouseContext db = new DbHouseContext();

            foreach (var id in ids)
            {
                Application activecheck = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
                if (activecheck != null)
                {
                    activecheck.State = state;
                }
                ReservationPlace placecheck = db.ReservationPlaces.FirstOrDefault(i => i.ReserveCode == activecheck.ReserveCode);
                if (placecheck != null)
                {
                    placecheck.FState = state;
                }
                if (state == 2)
                {
                    PaymentItem pay = db.PaymentItems.FirstOrDefault(i => i.PaymentItemCode == activecheck.PaymentItemCode);
                    pay.Ispushed = true;
                    db.SaveChanges();
                }
            }
            db.SaveChanges();

            // 返回一个表示成功的JSON响应（可以根据需要进行修改）
            return Json(new { success = true, message = "狀態已更新" });
        }
        public IActionResult ActiveDetails(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return RedirectToAction("ActiveList");
            }
            DbHouseContext db = new DbHouseContext();
            Application active = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
            ReservationPlace resavationplace = db.ReservationPlaces.Where(i => i.ReserveCode == active.ReserveCode).FirstOrDefault();
            string place = db.PublicSpaceDetails.Where(p => p.PlaceCode == resavationplace.PlaceCode).Select(p => p.PlaceName).FirstOrDefault();
            string time = db.Periodoftimes.Where(p => p.PeriodoftimeCode == resavationplace.PeriodoftimeCode).Select(p => p.Periodoftime1).FirstOrDefault();
            ViewBag.Place = place;
            ViewBag.Time = time;
            if (active == null)
            {
                return RedirectToAction("ActiveList");
            }

            CActive active_edit = new CActive
            {
                Applications = active,
                ReservationPlace = resavationplace,
            };

            return View(active_edit);
        }

        public IActionResult ActiveEdit(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (id == null)
            {
                return RedirectToAction("ActiveList");
            }
            DbHouseContext db = new DbHouseContext();
            Application active = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
            ReservationPlace place = db.ReservationPlaces.Where(i => i.ReserveCode == active.ReserveCode).FirstOrDefault();
            if (active == null)
            {
                return RedirectToAction("ActiveList");
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
        public IActionResult ActiveEdit(CActive active_edit)
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
                    string path = _enviro.WebRootPath + "/images/Active/" + newPhotoName;
                    try
                    {
                        // 刪除舊照片（如果存在）
                        if (!string.IsNullOrEmpty(activedb.Image))
                        {
                            string oldFilePath = Path.Combine(_enviro.WebRootPath + "/images/Active/", activedb.Image);
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
                        return RedirectToAction("ActiveEdit", new { id = active_edit.Applications.ApplyCode });

                    }

                }
                activedb.Name = active_edit.Applications.Name;
                activedb.Phone = active_edit.Applications.Phone;
                activedb.Email = active_edit.Applications.Email;
                activedb.ActivityName = active_edit.Applications.ActivityName;
                activedb.Activities = active_edit.Applications.Activities;
                activedb.DateStart = active_edit.Applications.DateStart;
                activedb.DateEnd = active_edit.Applications.DateEnd;
                activedb.Introduce = active_edit.Applications.Introduce;
                activedb.MaxApplicants = active_edit.Applications.MaxApplicants;

                placedb.PlaceCode = active_edit.ReservationPlace.PlaceCode;
                placedb.PeriodoftimeCode = active_edit.ReservationPlace.PeriodoftimeCode;
                placedb.FDateStart = active_edit.Applications.DateStart;
                placedb.FDateEnd = active_edit.Applications.DateEnd;
                db.SaveChanges();
            }
            return RedirectToAction("ActiveList");
        }
        //用戶活動
        //公共活動
        public IActionResult ActivePublic(int page = 1, int pageSize = 10)
        {            
            DbHouseContext db = new DbHouseContext();
            string userid = HttpContext.Session.GetString("UserID");
            Admin admin = db.Admins.FirstOrDefault(i => i.UserId == userid);
            string username = admin.UserName;
            IEnumerable<Application> active_db = from ap in db.Applications where ap.Name == username orderby ap.DateStart select ap;
            int skip = (page - 1) * pageSize;
            ViewBag.TotalCount = active_db.Count(); // 總數量
            ViewBag.PageSize = pageSize; // 每頁數量
            active_db = active_db.Skip(skip).Take(pageSize).ToList();
            return View(active_db);
        }
        public IActionResult ActiveCreate()
        {
            string userid = HttpContext.Session.GetString("UserID");

            DbHouseContext db = new DbHouseContext();
            Admin admin = db.Admins.FirstOrDefault(i => i.UserId == userid);


            var app = new Application()
            {
                HouseholdCode = userid, //admin.UserId, 串admin > residrnt
                Name = admin.UserName,
                Phone = admin.UserPhone,   //admin.UserPhone,
                Email = admin.UserEmail  //admin.UserEmail
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
        public IActionResult ActiveCreate(CActive i)
        {
            using (DbHouseContext db = new DbHouseContext())
            {
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
                                string userid = HttpContext.Session.GetString("UserID");
                                Admin admin = db.Admins.FirstOrDefault(i => i.UserId == userid);
                                var app = new Application()
                                {
                                    HouseholdCode = userid, //admin.UserId, 串admin > residrnt
                                    Name = admin.UserName,
                                    Phone = admin.UserPhone,   //admin.UserPhone,
                                    Email = admin.UserEmail  //admin.UserEmail
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
                        }
                    }
                }


                i.Applications.ReserveCode = null;
                i.Applications.Applicants = 0;
                i.Applications.State = 2;
                db.Applications.Add(i.Applications);
                db.SaveChanges();

                // 使用新的 ReserveCode 創建 Reservation 資料
                var reservation = new Reservation
                {
                    HouseholdCode = i.Applications.HouseholdCode,
                    HName = i.Applications.Name,
                    HPhone = i.Applications.Phone,
                    HEmail = i.Applications.Email,
                    HOrdertime = DateTime.Now,
                    HState = 2
                };

                db.Reservations.Add(reservation);
                db.SaveChanges();

                // 讀取 Reservation 中的新生成的 ReserveCode
                var newReserveCode = reservation.ReserveCode;
                // 更新 Application 中的 ReserveCode
                i.Applications.ReserveCode = newReserveCode;

                var reservationplace = new ReservationPlace
                {
                    ReserveCode = newReserveCode,
                    PlaceCode = i.PublicSpaceDetail[0].PlaceCode,
                    PeriodoftimeCode = i.Periodoftime[0].PeriodoftimeCode,
                    FState = 2,  //管理員新增的
                    FDateStart = i.Applications.DateStart,
                    FDateEnd = i.Applications.DateEnd
                };
                db.ReservationPlaces.Add(reservationplace);
                db.SaveChanges();

                return RedirectToAction("ActivePublic");
            }
        }
        public IActionResult ActiveDelete(int? id)
        {
            if (id == null)
                return RedirectToAction("ActivePublic");
            DbHouseContext db = new DbHouseContext();
            Application active = db.Applications.FirstOrDefault(i => i.ApplyCode == id);
            Reservation reserve = db.Reservations.FirstOrDefault(i => i.ReserveCode == active.ReserveCode);
            ReservationPlace place = db.ReservationPlaces.FirstOrDefault(i => i.ReserveCode == active.ReserveCode);

            if (active != null)
            {
                db.Applications.Remove(active);
                db.ReservationPlaces.Remove(place);
                db.Reservations.Remove(reserve);
                db.SaveChanges();
            }
            return RedirectToAction("ActivePublic");
        }

        //公共活動

    }
}
