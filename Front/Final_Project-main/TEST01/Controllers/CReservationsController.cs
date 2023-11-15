using FifthGroup_front.Models;
using FifthGroup_front.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using XAct.Library.Settings;

namespace FifthGroup_front.Controllers
{
    public class CReservationsController : Controller
    {
        public IActionResult ListR()
        {
            using (var db = new DbHouseContext())
            {
                var currentDate = DateTime.Today;
                string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");

                var reservationPlaces = db.ReservationPlaces
                    .Where(r => r.FDate >= currentDate)
                    .ToList();

                var reservations = db.Reservations
                    .Where(r => r.HouseholdCode == userHouseholdCode)
                    .ToList();

                // 使用 Join 操作合并 Reservations 和 ReservationPlaces
                var mergedReservations = reservations
                    .Join(reservationPlaces,
                        r => r.ReserveCode,
                        rp => rp.ReserveCode,
                        (r, rp) => new
                        {
                            Reservation = r,
                            ReservationPlace = rp
                        })
                    .ToList();

                CReservations_four model = new CReservations_four()
                {
                    Reservations = mergedReservations.Select(r => r.Reservation).ToList(),
                    ReservationPlace = mergedReservations.Select(r => r.ReservationPlace)
                    .OrderBy(r=>r.FDate).ToList(),
                    PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
                    Periodoftime = db.Periodoftimes.ToList(),
                };

                return View(model);
            }
        }
        public IActionResult ListBefore()
        {
            using (var db = new DbHouseContext())
            {
                var currentDate = DateTime.Today; // 获取今天的日期
                string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");
                var reservations = db.Reservations
               .Where(r => r.HouseholdCode == userHouseholdCode)
              .ToList();
                var reservationPlaces = db.ReservationPlaces
                    .Where(r => r.FDate < currentDate) // 只选择日期小于今天的记录
                    .OrderByDescending(r => r.FDate)
                    .ToList();
                // 使用 Join 操作合并 Reservations 和 ReservationPlaces
                var mergedReservations = reservations
                    .Join(reservationPlaces,
                        r => r.ReserveCode,
                        rp => rp.ReserveCode,
                        (r, rp) => new
                        {
                            Reservation = r,
                            ReservationPlace = rp
                        })
                    .ToList();
                CReservations_four model = new CReservations_four()
                {
                    Reservations = mergedReservations.Select(r => r.Reservation).ToList(),
                    ReservationPlace = mergedReservations.Select(r => r.ReservationPlace).ToList(),
                    PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
                    Periodoftime = db.Periodoftimes.ToList(),
                };

                return View(model);
            }
        }
        public IActionResult ListAgain()
        {
            using (var db = new DbHouseContext())
            {
                var currentDate = DateTime.Today;
                string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");

                var reservationPlaces = db.ReservationPlaces
                    .Where(r => r.FDate >= currentDate)
                    .ToList();

                var reservations = db.Reservations
                    .Where(r => r.HouseholdCode == userHouseholdCode)
                    .ToList();

                // 使用 Join 操作合并 Reservations 和 ReservationPlaces
                var mergedReservations = reservations
                    .Join(reservationPlaces,
                        r => r.ReserveCode,
                        rp => rp.ReserveCode,
                        (r, rp) => new
                        {
                            Reservation = r,
                            ReservationPlace = rp
                        })
                    .ToList();

                CReservations_four model = new CReservations_four()
                {
                    Reservations = mergedReservations.Select(r => r.Reservation).ToList(),
                    ReservationPlace = mergedReservations.Select(r => r.ReservationPlace)
                    .OrderBy(r => r.FDate).ToList(),
                    PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
                    Periodoftime = db.Periodoftimes.ToList(),
                };

                return View(model);
            }


            //return View(model);
            //using (var db = new DbHouseContext())
            //{
            //    var currentDate = DateTime.Today;
            //    var reservationPlaces = db.ReservationPlaces
            //   .Where(r => r.FDate >= currentDate) // 只选择大于或等于今天的日期
            //   .OrderBy(r => r.FDate)
            //   .ToList();
            //    CReservations_four model = new CReservations_four()
            //    {
            //        Reservations = db.Reservations.ToList(), // 將結果複製到集合中
            //        ReservationPlace = reservationPlaces,
            //        PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
            //        Periodoftime = db.Periodoftimes.ToList(),
            //    };
            //    return View(model);
            //}
        }
        [HttpPost]
        public IActionResult ListAgain(Reservation r)
        {
            DbHouseContext db = new DbHouseContext();
            TempData["ReserveCode"] = r.ReserveCode;
            return RedirectToAction("ReservationPlacesTEST");
        }
        //---------------------------
        public IActionResult ReservationsInfor()    //Create in Reservation
        {
            string userHouseholdCode = HttpContext.Session.GetString("UserHouseholdCode");
            string userName = HttpContext.Session.GetString("UserName");
            string userPhone = HttpContext.Session.GetString("UserPhone");
            string userEmail = HttpContext.Session.GetString("UserEmail");

            DbHouseContext db = new DbHouseContext();

            var app = new Reservation()
            {
                HouseholdCode = userHouseholdCode,
                HName = userName,
                HPhone = userPhone,
                HEmail = userEmail
            };

            return View(app);
        }
        [HttpPost]
        public IActionResult ReservationsInfor(Reservation r)
        {
            DbHouseContext db = new DbHouseContext();

            db.Reservations.Add(r);
            db.SaveChanges();
            TempData["ReserveCode"] = r.ReserveCode;
            return RedirectToAction("ReservationPlacesTEST");
        }
        //---------------------------
        public IActionResult ReservationPlacesTEST(int? placeCode, string periods)    //Create in ReservationPlaces
        {
            DbHouseContext db = new DbHouseContext();
            var publicSpaceDetail = db.PublicSpaceDetails;
            var reservations = db.Reservations;
            var periodoftimes = db.Periodoftimes;
            // 创建PublicSpaceViewModel并填充数据
            var viewModel = new CReservations_four
            {
                PublicSpaceDetail = publicSpaceDetail,
                Reservations = reservations,
                Periodoftime = periodoftimes
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult ReservationPlacesTEST(ReservationPlace r)
        {
            DbHouseContext db = new DbHouseContext();
            try
            {
                // 檢查是否同一人有相同的預約
                var sameperson = db.ReservationPlaces.FirstOrDefault(rp =>
                    (rp.FDate == r.FDate || (rp.FDateStart <= r.FDate && rp.FDateEnd >= r.FDate)) &&
                    rp.PlaceCode == r.PlaceCode &&
                    rp.ReserveCode == r.ReserveCode &&
                    rp.PeriodoftimeCode == r.PeriodoftimeCode);

                // 檢查是否已存在相同的預約
                var existingReservation = db.ReservationPlaces.FirstOrDefault(rp =>
                    (rp.FDate == r.FDate || (rp.FDateStart <= r.FDate && rp.FDateEnd >= r.FDate)) &&
                    rp.PlaceCode == r.PlaceCode &&
                    rp.PeriodoftimeCode == r.PeriodoftimeCode);

                // 預約衝突最多3筆
                var countOfMatchingReservations = db.ReservationPlaces.Count(rp =>
                    (rp.FDate == r.FDate || (rp.FDateStart <= r.FDate && rp.FDateEnd >= r.FDate)) &&
                    rp.PlaceCode == r.PlaceCode &&
                    rp.PeriodoftimeCode == r.PeriodoftimeCode);


                if (sameperson != null)
                {
                    // 如果已存在相同的預約，設定錯誤訊息
                    ModelState.AddModelError(string.Empty, "不可以重複預約！");
                    var viewModel = new CReservations_four
                    {
                        PublicSpaceDetail = db.PublicSpaceDetails,
                        Reservations = db.Reservations,
                        Periodoftime = db.Periodoftimes
                    };
                    TempData["ReserveCode"] = r.ReserveCode;
                    return View(viewModel); // 返回包含 CReservations_four 模型的視圖
                }
                else if (existingReservation != null && countOfMatchingReservations < 3)
                {
                    // 如果已存在相同的預約，設定候補訊息
                    ModelState.AddModelError(string.Empty, $"該場地已有預約，您為候補第{countOfMatchingReservations}位");
                    var viewModel = new CReservations_four
                    {
                        PublicSpaceDetail = db.PublicSpaceDetails,
                        Reservations = db.Reservations,
                        Periodoftime = db.Periodoftimes
                    };
                    db.ReservationPlaces.Add(r);
                    db.SaveChanges();
                    TempData["ReserveCode"] = r.ReserveCode;
                    TempData["候補"] = $"該時段已有預約，您為候補第{countOfMatchingReservations}位"; // 返回包含 CReservations_four 模型的視圖
                    return RedirectToAction("ListAgain");
                }
                else if (existingReservation != null && countOfMatchingReservations >= 3)
                {
                    // 如果已存在相同的預約，設定錯誤訊息
                    ModelState.AddModelError(string.Empty, "預約已滿");
                    var viewModel = new CReservations_four
                    {
                        PublicSpaceDetail = db.PublicSpaceDetails,
                        Reservations = db.Reservations,
                        Periodoftime = db.Periodoftimes
                    };
                    TempData["ReserveCode"] = r.ReserveCode;
                    return View(viewModel); // 返回包含 CReservations_four 模型的視圖
                }
                db.ReservationPlaces.Add(r);
                db.SaveChanges();
                TempData["ReserveCode"] = r.ReserveCode;
                TempData["候補"] = "預約成功！請耐心等候審核~";
                return RedirectToAction("ListAgain");
            }
            catch (Exception ex)
            {
                // 處理例外狀況
            }
            return View(r);
        }
        //---------------------------
        public IActionResult Edit(int? id) //回歸原始
        {
            if (id == null)
                return RedirectToAction("ListR");
            DbHouseContext db = new DbHouseContext();
            var publicSpaceDetail = db.PublicSpaceDetails;
            var reservations = db.Reservations;
            var periodoftimes = db.Periodoftimes;

            //ReservationPlace reservationPlace = db.ReservationPlaces.FirstOrDefault(t => t.ReserveCode == id);
            var reservationPlace = db.ReservationPlaces.Where(t => t.ReserveCode == id).Select(CRf => new CReservations_four 
            {
                Code=CRf.Code,
                ReserveCode=CRf.ReserveCode,
                PlaceCode= CRf.PlaceCode,
                FDate = CRf.FDate,
                FState=CRf.FState,
                PeriodoftimeCode = CRf.PeriodoftimeCode,
                PublicSpaceDetail = publicSpaceDetail.ToList(),
                Reservations = reservations.ToList(),
                Periodoftime = periodoftimes.ToList()
            }).First();
            if (reservationPlace == null)
                return RedirectToAction("ListR");
            return View(reservationPlace);
        }
        [HttpPost]
        public  IActionResult Edit(CReservations_four reservationPlaceIn)
        {
            DbHouseContext db = new DbHouseContext();
            ReservationPlace reservationPlaceDb = db.ReservationPlaces.FirstOrDefault(t => t.ReserveCode == reservationPlaceIn.ReserveCode);

            if (reservationPlaceDb != null)
            {
                reservationPlaceDb.ReserveCode = reservationPlaceIn.ReserveCode;
                reservationPlaceDb.PlaceCode = reservationPlaceIn.PlaceCode;
                reservationPlaceDb.FDate = reservationPlaceIn.FDate;
                reservationPlaceDb.PeriodoftimeCode = reservationPlaceIn.PeriodoftimeCode;
                reservationPlaceDb.FState = reservationPlaceIn.FState;
                db.SaveChanges();
            }
            return RedirectToAction("ListR");
        }
        //---------------------------
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("ListR");
            DbHouseContext db = new DbHouseContext();
            ReservationPlace rp = db.ReservationPlaces.FirstOrDefault(t => t.ReserveCode == id);
            if (rp != null)
            {
                db.ReservationPlaces.Remove(rp);
                db.SaveChanges();
            }
            return RedirectToAction("ListR");
        }
        private bool ReservationPlaceExists(string id)  //我到現在還是不知道這是什麼...
        {
            DbHouseContext db = new DbHouseContext();

            return (db.ReservationPlaces?.Any(e => e.ReserveCode.ToString() == id)).GetValueOrDefault();
        }
    }
}
