using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FifthGroup_Backstage.Controllers
{
    public class CommunityController : Controller
    {
        private readonly DbHouseContext dbHouseContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CommunityController(DbHouseContext dbHouseContext,IHttpContextAccessor httpContextAccessor)
        {
            this.dbHouseContext = dbHouseContext;
            this.httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(NewCommunity newCommunity)
        {
            try
            {
                // 合併縣市、行政區和詳細地址
                string fullAddress = newCommunity.city + newCommunity.town + newCommunity.Address;

                var community = new Community
                {
                    CommunityName = newCommunity.CommunityName,
                    Address = fullAddress, // 將完整地址設定到 Address 屬性
                    TotalUnits = newCommunity.TotalUnits,
                    VerificationCode = newCommunity.VerificationCode
                };

                dbHouseContext.Communities.Add(community);
                dbHouseContext.SaveChanges();

                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return View("Error");
            }
        }


        [HttpGet]
        public IActionResult List()
        {
            string userID = httpContextAccessor.HttpContext.Session.GetString("UserID");

            var query = from u in dbHouseContext.Admins
                        join c in dbHouseContext.Communities on u.CommunityId equals c.CommunityId
                        join cb in dbHouseContext.CommunityBuildings on c.CommunityId equals cb.CommunityId
                        where u.IsVerified == true && u.UserId == userID

                        select new ComminitySummay
                        {
                            Admin = u.UserName,
                            CommunityName = c.CommunityName,
                            TotalUnits = c.TotalUnits,
                            Address = c.Address,
                            BuildingName = cb.BuildingName,
                            FloorNumber = cb.FloorNumber,
                            UnitNumber = cb.UnitNumber
                        };

            var result = query.ToList();

            return View(result);
        }










    }
}

