using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using X.PagedList;


namespace FifthGroup_Backstage.Controllers
{

    public class ReservationsController : Controller
    {
        public async Task<IActionResult> List(int? page) 
        {
            using (var db = new DbHouseContext())
            {
                var currentDate = DateTime.Today;
                var reservationPlaces = await db.ReservationPlaces.Where(s => s.FState > 1 && (s.FDate >= currentDate||s.FDateEnd >= currentDate))  
                    .OrderBy(r => r.FDate).ThenBy(r => r.FDateStart)
                    .ToListAsync();

                int pageNumber = page ?? 1;
                int pageSize = 10;

                IPagedList<ReservationPlace> pagedReservationPlaces = reservationPlaces.ToPagedList(pageNumber, pageSize);

                CReservations_four model = new CReservations_four()
                {
                    Reservations = await db.Reservations.ToListAsync(),
                    ReservationPlace = pagedReservationPlaces,
                    PublicSpaceDetail = await db.PublicSpaceDetails.ToListAsync(),
                    Periodoftime = await db.Periodoftimes.ToListAsync(),
                };
                return View(model);
            }
        }
        //第二個
        //public IActionResult Listreview_personal()
        //{
        //    using (var db = new DbHouseContext())
        //    {
        //        var reservationPlaces = db.ReservationPlaces.Where(s=>s.FState<2 && s.FDate != null)
        //            .OrderBy(r => r.FDate).ToList();
        //        CReservations_four model = new CReservations_four()
        //        {
        //            Reservations = db.Reservations.ToList(), // 將結果複製到集合中
        //            ReservationPlace = reservationPlaces,
        //            PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
        //            Periodoftime = db.Periodoftimes.ToList(),
        //        };
        //        return View(model);
        //    }
        //}public async Task<IActionResult> List(int? page) 
        public async Task<IActionResult> Listreview_personal(int? page, int? placeCode, string periods)
        {
            using (var db = new DbHouseContext())
            {
                // 获取所有未通过审核（FState < 2）且日期不为空的预约
                var query = db.ReservationPlaces.Where(s => s.FState < 2 && s.FDate != null);

                // 如果选择了场地，进行进一步的过滤
                if (placeCode.HasValue)
                {
                    query = query.Where(r => r.PlaceCode == placeCode.Value);
                }

                // 如果选择了时段，进行进一步的过滤
                if (!string.IsNullOrEmpty(periods))
                {
                    var selectedPeriods = periods.Split(',').Select(int.Parse).ToList();
                    query = query.Where(r => selectedPeriods.Contains((int)r.PeriodoftimeCode));
                }

                var reservationPlaces = await query.OrderBy(r => r.FDate).ToListAsync();

                int pageNumber = page ?? 1;
                int pageSize = 10;

                IPagedList<ReservationPlace> pagedReservationPlaces = reservationPlaces.ToPagedList(pageNumber, pageSize);

                CReservations_four model = new CReservations_four()
                {
                    Reservations =  db.Reservations.ToList(),
                    ReservationPlace = pagedReservationPlaces,
                    PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
                    Periodoftime = db.Periodoftimes.ToList(),
                };

                return View(model);
            }
        }

        //public IActionResult Listreview_group()
        //{
        //    using (var db = new DbHouseContext())
        //    {
        //        var reservationPlaces = db.ReservationPlaces.Where(s => s.FState == 0 && s.FDateStart != null)
        //            .OrderBy(r => r.FDate).ToList();
        //        CReservations_four model = new CReservations_four()
        //        {
        //            Reservations = db.Reservations.ToList(), // 將結果複製到集合中
        //            ReservationPlace = reservationPlaces,
        //            PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
        //            Periodoftime = db.Periodoftimes.ToList(),
        //        };
        //        return View(model);
        //    }
        //}
        //第三個
        public async Task<IActionResult> Listbefore(int? page)
        {
            using (var db = new DbHouseContext())
            {
                var currentDate = DateTime.Today;
                var reservationPlaces = await db.ReservationPlaces
                    .Where(s => s.FState < 5 && (s.FDate < currentDate || s.FDateStart < currentDate))
                    .OrderByDescending(r => r.FDate)
                    .ThenBy(r => r.FDateStart)
                    .ToListAsync();

                int pageNumber = page ?? 1;
                int pageSize = 10;

                IPagedList<ReservationPlace> pagedReservationPlaces = reservationPlaces.ToPagedList(pageNumber, pageSize);

                CReservations_four model = new CReservations_four()
                {
                    Reservations = await db.Reservations.ToListAsync(),
                    ReservationPlace = pagedReservationPlaces,
                    PublicSpaceDetail = await db.PublicSpaceDetails.ToListAsync(),
                    Periodoftime = await db.Periodoftimes.ToListAsync(),
                };

                return View(model);
            }
        }        
        //public IActionResult Listbefore()
        //{
        //    using (var db = new DbHouseContext())
        //    {
        //        var currentDate = DateTime.Today;
        //        var reservationPlaces = db.ReservationPlaces.Where(s => s.FState < 5 && (s.FDate < currentDate || s.FDateStart < currentDate)).OrderByDescending(r => r.FDate).ThenBy(r => r.FDateStart).ToList();
        //        CReservations_four model = new CReservations_four()
        //        {
        //            Reservations = db.Reservations.ToList(), // 將結果複製到集合中
        //            ReservationPlace = reservationPlaces,
        //            PublicSpaceDetail = db.PublicSpaceDetails.ToList(),
        //            Periodoftime = db.Periodoftimes.ToList(),
        //        };
        //        return View(model);
        //    }
        //}   
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("Listreview");
            DbHouseContext db = new DbHouseContext();

            ReservationPlace reservationPlace = db.ReservationPlaces.FirstOrDefault(t => t.ReserveCode == id);
            if (reservationPlace == null)
                return RedirectToAction("Listreview");
            return View(reservationPlace);
        }   //第二個的修改
        [HttpPost]
        public IActionResult Edit(ReservationPlace reservationPlaceIn)
        {
            DbHouseContext db = new DbHouseContext();
            ReservationPlace reservationPlaceDb = db.ReservationPlaces.FirstOrDefault(t => t.ReserveCode == reservationPlaceIn.ReserveCode);

            if (reservationPlaceDb != null)
            {
                reservationPlaceDb.Code = reservationPlaceIn.Code;
                reservationPlaceDb.ReserveCode = reservationPlaceIn.ReserveCode;

                reservationPlaceDb.PlaceCode = reservationPlaceIn.PlaceCode;
                reservationPlaceDb.FDate = reservationPlaceIn.FDate;
                reservationPlaceDb.PeriodoftimeCode = reservationPlaceIn.PeriodoftimeCode;
                reservationPlaceDb.FState = reservationPlaceIn.FState;

                db.SaveChanges();

            }
            return RedirectToAction("Listreview");
        }
        public IActionResult EditAll()
        {
            DbHouseContext db = new DbHouseContext();

            IEnumerable<ReservationPlace> reservationPlace_db = from ap in db.ReservationPlaces where ap.FState != 2 select ap;

            return View(reservationPlace_db);
        }
        [HttpPost]
        public IActionResult EditAll(List<int> ids, int state)
        {
            DbHouseContext db = new DbHouseContext();

            foreach (var id in ids)
            {
                ReservationPlace item = db.ReservationPlaces.FirstOrDefault(i => i.Code == id);
                if (item != null)
                {
                    item.FState = state;
                }
            }
            db.SaveChanges();

            // 返回一个表示成功的JSON响应（可以根据需要进行修改）
            return Json(new { success = true, message = "狀態已更新" });
        }
        public IActionResult Listbefore_Details(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            DbHouseContext db = new DbHouseContext();

            Reservation reservation = db.Reservations.FirstOrDefault(t => t.ReserveCode == id);
            if (reservation == null)
                return RedirectToAction("List");
            return View(reservation);
        }
        //[HttpPost]
        //public IActionResult Listbefore_Details(Reservation reservationIn)
        //{
        //    DbHouseContext db = new DbHouseContext();
        //    Reservation reservationDb = db.Reservations.FirstOrDefault(t => t.ReserveCode == reservationIn.ReserveCode);

        //    if (reservationDb != null)
        //    {
        //        reservationDb.ReserveCode = reservationIn.ReserveCode;
        //        reservationDb.HouseholdCode = reservationIn.HouseholdCode;
        //        reservationDb.HName = reservationIn.HName;
        //        reservationDb.HPhone = reservationIn.HPhone;
        //        reservationDb.HEmail = reservationIn.HEmail;
        //        reservationDb.HState = reservationIn.HState;
        //        reservationDb.HOrdertime = reservationIn.HOrdertime;
        //        db.SaveChanges();

        //    }
        //    return RedirectToAction("List");
        //}
        //public IActionResult Details(int? id) 好像用不到先放著
        //{
        //    if (id == null)
        //        return RedirectToAction("List");
        //    DbHouseContext db = new DbHouseContext();

        //    Reservation reservation = db.Reservations.FirstOrDefault(t => t.ReserveCode == id);
        //    if (reservation == null)
        //        return RedirectToAction("List");
        //    return View(reservation);
        //}
    }
}
