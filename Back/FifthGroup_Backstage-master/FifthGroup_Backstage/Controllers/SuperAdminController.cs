using FifthGroup_Backstage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FifthGroup_Backstage.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly DbHouseContext dbHouseContext;

        public IActionResult Index()
        {
            return View();
        }

        public SuperAdminController(DbHouseContext dbHouseContext)
        {
            this.dbHouseContext = dbHouseContext;
        }


        private IEnumerable<Admin> GetAdmins()
        {

            // 從資料庫中獲取Admin物件的列表
            var admins = dbHouseContext.Admins.ToList();

            return admins;
        }

        [HttpGet]
        public IActionResult List()
        {

            // 從資料庫或其他來源獲取Admin物件的列表
            IEnumerable<Admin> admins = GetAdmins();

            // 返回包含Admin物件列表的視圖
            return View(admins);

        }
    }
}
