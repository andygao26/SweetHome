using Microsoft.AspNetCore.Mvc.Rendering;

namespace FifthGroup_Backstage.ViewModel
{
    public class CommunityBuildingViewModel
    {
        public int SelectedCommunityBuildingId { get; set; }
        public SelectList CommunityBuildings { get; set; }
        public SelectList FloorNumbers { get; set; }
        public SelectList UnitNumbers { get; set; }
    }
}
