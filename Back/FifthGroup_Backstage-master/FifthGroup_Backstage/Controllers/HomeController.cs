using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace FifthGroup_Backstage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbHouseContext dbHouseContext;

        public HomeController(ILogger<HomeController> logger,DbHouseContext dbHouseContext)
        {
            _logger = logger;
            this.dbHouseContext=dbHouseContext;
        }

        public IActionResult Index(int page = 1)
        {
            string userID = HttpContext.Session.GetString("UserID");
            var dashboardViewModel = new DashboardViewModel();

            if (userID != null)
            {
                // 透過userID查找Admin所屬社區的CommunityId
                dashboardViewModel.UserID = userID;
                dashboardViewModel.CommunityId = (int)dbHouseContext.Admins.Find(userID).CommunityId;

                // 透過communityId查找Community的地址相關資訊
                var community = dbHouseContext.Communities.Find(dashboardViewModel.CommunityId);
                dashboardViewModel.CommunityName = community.CommunityName;
                dashboardViewModel.CommunityAddress = community.Address;
                dashboardViewModel.CommunityTotalUnits = community.TotalUnits;

                // 分棟資料
                var communityBuildings = dbHouseContext.CommunityBuildings
                    .Where(cb => cb.CommunityId == dashboardViewModel.CommunityId)
                    .ToList();
                dashboardViewModel.CommunityBuildings = communityBuildings;


                // 找大頭貼
                dashboardViewModel.UserPhotoUrl = dbHouseContext.Admins.Find(userID).UserPhoto;

                // 獲取目前時間
                dashboardViewModel.CurrentDate = DateTime.Now;

                // 分頁相關數據
                dashboardViewModel.ItemsPerPage = 5; // 每頁五個
                dashboardViewModel.TotalItems = dbHouseContext.Applications.Count(a => a.State == 2 && a.DateStart >= dashboardViewModel.CurrentDate);
                dashboardViewModel.TotalPages = (int)Math.Ceiling((double)dashboardViewModel.TotalItems / dashboardViewModel.ItemsPerPage);

                // 確保頁碼在有效範圍內
                page = Math.Max(1, Math.Min(page, dashboardViewModel.TotalPages));
                dashboardViewModel.CurrentPage = page;

                // 查询活动数据
                var applicationsData = dbHouseContext.Applications
                    .Where(a => a.State == 2 && a.DateStart >= dashboardViewModel.CurrentDate)
                    .OrderBy(a => a.DateStart)
                    .Skip((page - 1) * dashboardViewModel.ItemsPerPage)
                    .Take(dashboardViewModel.ItemsPerPage)
                    .Select(a => new ApplicationData
                    {
                        ActivityName = a.ActivityName,
                        DateStart = a.DateStart,
                        DateEnd = a.DateEnd,
                        Applicants = (int)a.Applicants,
                        MaxApplicants = (int)a.MaxApplicants,
                        EnrollmentRate = (double)((double)a.Applicants / a.MaxApplicants)
                    })
                    .ToList();

                dashboardViewModel.ApplicationsData = applicationsData;
            }

            dashboardViewModel.UnprocessedCount = dbHouseContext.Repairs.Count(x => x.ProcessingStatus == "未處理");
            dashboardViewModel.StateZeroCount = dbHouseContext.Reservations.Count(x => x.HState == 0);

            return View(dashboardViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}