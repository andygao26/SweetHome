using FifthGroup_Backstage.Models;
using FifthGroup_Backstage.Repositories;
using FifthGroup_Backstage.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FifthGroup_Backstage.Controllers
{
    public class BuildingController : Controller
    {
        private readonly DbHouseContext dbHouseContext;

        public BuildingController(DbHouseContext dbHouseContext)
        {
            this.dbHouseContext=dbHouseContext;
        }

        [HttpGet]
        public IActionResult Add() 
        {
            // 從數據庫獲取所有的社區
            var communities = dbHouseContext.Communities.ToList();

            // 將社區ID和名稱作為選項傳遞到視圖
            ViewBag.CommunityId = new SelectList(communities, "CommunityId", "CommunityName");

            return View();

        }

        [HttpPost]
        public IActionResult Add(CommunityBuildingInfo model)
        {
            if (ModelState.IsValid)
            {
                // 使用模型中的數據創建 CommunityBuilding 對象
                var communityBuilding = new CommunityBuilding
                {
                    CommunityId = model.CommunityId,
                    BuildingName = model.BuildingName,
                    FloorNumber = model.FloorNumber,
                    UnitNumber = model.UnitNumber
                };

                // 將 CommunityBuilding 對象添加到數據庫上下文並保存更改
                dbHouseContext.CommunityBuildings.Add(communityBuilding);
                dbHouseContext.SaveChanges();

                // 數據保存成功後，返回一個包含成功消息的JSON響應
                return Json(new { success = true, message = "提交成功！" });
            }

            // 如果模型驗證失敗，返回一個包含錯誤消息的JSON響應
            return Json(new { success = false, message = "請填寫所有內容。" });
        }
     }

}


