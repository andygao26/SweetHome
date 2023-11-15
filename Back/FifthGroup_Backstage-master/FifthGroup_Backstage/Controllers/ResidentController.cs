using Microsoft.AspNetCore.Mvc;
using FifthGroup_Backstage.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FifthGroup_Backstage.ViewModel;
using System.Data.Entity;

namespace second.Controllers
{
    public class ResidentController : Controller
    {
        private readonly DbHouseContext dbHouseContext;

        public ResidentController(DbHouseContext dbHouseContext)
        {
            this.dbHouseContext = dbHouseContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {
            DbHouseContext db = new DbHouseContext();
            IEnumerable<Resident> datas = from p in db.Residents
                                          select p;
            return View(datas);
        }
        public IActionResult CreateResident()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateResident(Resident p)
        {
            DbHouseContext db = new DbHouseContext();
            db.Residents.Add(p);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        //-------------------------------Test------------------//
        [HttpGet]
        public IActionResult Test()
        {
            ViewBag.CommunityBuildingList = new SelectList(dbHouseContext.CommunityBuildings, "CommunityBuildingId", "BuildingName");
            return View();

        }
        [HttpPost]
        public IActionResult Test(ResidentViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 創建Resident對象並設定屬性值
                    var resident = new Resident
                    {
                        HouseholdCode = viewModel.HouseholdCode,
                        CommunityBuildingId = viewModel.CommunityBuildingId,
                        FloorNumber = viewModel.FloorNumber,
                        UnitNumber = viewModel.UnitNumber,
                        Name = viewModel.Name,
                        Phone = viewModel.Phone,
                        Email = viewModel.Email,
                        Password = viewModel.Password,
                        Headshot = viewModel.HeadshotFile,
                    };

                    // 儲存回DB
                    dbHouseContext.Residents.Add(resident);
                    dbHouseContext.SaveChanges();

                    // 数据保存成功后，返回一个包含成功消息的JSON响应
                    return Json(new { success = true, message = "提交成功！" });
                }

                // 如果模型验证失败，返回一个包含错误消息的JSON响应
                return Json(new { success = false, message = "失敗。" });
            }
            catch (Exception ex)
            {
                // 如果发生异常，可以在这里记录日志或返回错误消息
                return Json(new { success = false, message = "提交失败：" + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetBuildingDetails(int CommunityBuildingId)
        {
            var building = dbHouseContext.CommunityBuildings
                   .Where(b => b.CommunityBuildingId == CommunityBuildingId)
                   .Select(b => new { b.FloorNumber, b.UnitNumber })
                   .FirstOrDefault();

            return Json(building);
        }

        //-------------------------------Test------------------//

        public IActionResult EditResident(string? id)
        {
            if (id == null)
                return RedirectToAction("List");
            DbHouseContext db = new DbHouseContext();
            Resident cust = db.Residents.FirstOrDefault(t => t.HouseholdCode == id);
            if (cust == null)
                return RedirectToAction("List");
            return View(cust);
        }
        [HttpPost]
        public IActionResult EditResident(Resident custIn)
        {
            DbHouseContext db = new DbHouseContext();
            Resident custDb = db.Residents.FirstOrDefault(t => t.HouseholdCode == custIn.HouseholdCode);

            if (custDb != null)
            {
                //custDb.HouseholdCode= custIn.HouseholdCode;

                custDb.CommunityBuildingId = custIn.CommunityBuildingId;
                custDb.FloorNumber = custIn.FloorNumber;
                custDb.UnitNumber = custIn.UnitNumber;
                custDb.Name = custIn.Name;
                custDb.Phone = custIn.Phone;
                custDb.Email = custIn.Email;
                custDb.Password = custIn.Password;
                custDb.Headshot = custIn.Headshot;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }
        public IActionResult DeleteResident(string? id)
        {
            if (id == null)
                return RedirectToAction("List");
            DbHouseContext db = new DbHouseContext();
            Resident cust = db.Residents.FirstOrDefault(t => t.HouseholdCode == id);
            if (cust != null)
            {
                db.Residents.Remove(cust);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

    }
}
