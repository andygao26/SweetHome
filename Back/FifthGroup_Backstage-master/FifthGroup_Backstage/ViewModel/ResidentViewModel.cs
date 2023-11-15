using FifthGroup_Backstage.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FifthGroup_Backstage.ViewModel
{
    public class ResidentViewModel
    {
        // Resident屬性

        public int CommunityBuildingId { get; set; }

        public string HouseholdCode { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }







        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string HeadshotFile { get; set; }

        // 用戶選擇的樓層和單元
        [Required(ErrorMessage = "Please select a floor.")]
        public int FloorNumber { get; set; }

        [Required(ErrorMessage = "Please select a unit.")]
        public int UnitNumber { get; set; }
    }
}



