using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_Backstage.Controllers
{
    public class FinancialController : Controller
    {
        private readonly DbHouseContext dbHouseContext;

        public FinancialController(DbHouseContext dbHouseContext)
        {
            this.dbHouseContext=dbHouseContext;
        }

        public IActionResult PHome()
        {
            return View();
        }


        public IActionResult Report1()
        {
            var data = (from pi in dbHouseContext.PaymentItems
                        join cb in dbHouseContext.CommunityBuildings
                        on pi.CommunityBuildingId equals cb.CommunityBuildingId
                        where pi.ItemClassificationCode == 1
                        group new { pi, cb } by new
                        {
                            Year = pi.Date.Year,
                            Month = pi.Date.Month,
                            cb.BuildingName
                        } into g
                        select new FinancialReportModel
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            BuildingName = g.Key.BuildingName,
                            TotalIncome = g.Sum(x => x.pi.Amount)
                        })
                     .OrderBy(r => r.Year)
                     .ThenBy(r => r.Month)
                     .ThenBy(r => r.BuildingName)
                     .ToList();

            return View(data);
        }

        public IActionResult Report2()
        {
            var data = (from pi in dbHouseContext.PaymentItems
                        join cb in dbHouseContext.CommunityBuildings
                        on pi.CommunityBuildingId equals cb.CommunityBuildingId
                        where pi.ItemClassificationCode == 2
                        group new { pi, cb } by new
                        {
                            Year = pi.Date.Year,
                            Month = pi.Date.Month,
                            cb.BuildingName
                        } into g
                        select new FinancialReportModel
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            BuildingName = g.Key.BuildingName,
                            TotalIncome = g.Sum(x => x.pi.Amount)
                        })
                   .OrderBy(r => r.Year)
                   .ThenBy(r => r.Month)
                   .ThenBy(r => r.BuildingName)
                   .ToList();

            return View(data);
        }
    }
}
