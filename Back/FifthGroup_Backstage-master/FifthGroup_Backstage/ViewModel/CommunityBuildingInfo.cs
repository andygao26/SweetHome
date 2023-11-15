using FifthGroup_Backstage.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifthGroup_Backstage.ViewModel
{
    public class CommunityBuildingInfo
    {
        public int CommunityId { get; set; } //用於儲存所選社區的Id
    
        public required string BuildingName { get; set; }//分棟名稱
        public int FloorNumber { get; set; }//樓層數
        public int UnitNumber { get; set; }//每層戶數
    }
}
