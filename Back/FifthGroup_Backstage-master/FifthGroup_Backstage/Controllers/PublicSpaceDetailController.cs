using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_Backstage.Controllers
{
    public class PublicSpaceDetailController : Controller
    {
        public IActionResult List(HKeywordViewModel vm)
        {
            DbHouseContext db = new DbHouseContext();

            IEnumerable<PublicSpaceDetail> PublicSpaceDetaildatas = null;

            if (string.IsNullOrEmpty(vm.txtKeyword))
                PublicSpaceDetaildatas = from p in db.PublicSpaceDetails.OrderByDescending(r=>r.Pid).ThenBy(r=>r.AreaCode)
                                         select p;
            else
                PublicSpaceDetaildatas = db.PublicSpaceDetails.Where(t => t.PlaceName.Contains(vm.txtKeyword));
            return View(PublicSpaceDetaildatas);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(PublicSpaceDetail p)
        {
            DbHouseContext db = new DbHouseContext();
            db.PublicSpaceDetails.Add(p);
            db.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            DbHouseContext db = new DbHouseContext();

            PublicSpaceDetail publicSpaceDetail = db.PublicSpaceDetails.FirstOrDefault(t => t.PlaceCode == id);
            if (publicSpaceDetail == null)
                return RedirectToAction("List");
            return View(publicSpaceDetail);
        }
        [HttpPost]
        public IActionResult Edit(PublicSpaceDetail publicSpaceDetailIn)
        {
            DbHouseContext db = new DbHouseContext();
            PublicSpaceDetail publicSpaceDetailDb = db.PublicSpaceDetails.FirstOrDefault(t => t.PlaceCode == publicSpaceDetailIn.PlaceCode);

            if (publicSpaceDetailDb != null)
            {
                publicSpaceDetailDb.PlaceCode = publicSpaceDetailIn.PlaceCode;
                publicSpaceDetailDb.Pid = publicSpaceDetailIn.Pid;
                publicSpaceDetailDb.AreaCode = publicSpaceDetailIn.AreaCode;
                publicSpaceDetailDb.PlaceName = publicSpaceDetailIn.PlaceName;
                db.SaveChanges();

            }
            return RedirectToAction("List");
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            DbHouseContext db = new DbHouseContext();
            PublicSpaceDetail psd = db.PublicSpaceDetails.FirstOrDefault(t => t.PlaceCode == id);
            if (psd != null)
            {
                db.PublicSpaceDetails.Remove(psd);
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
