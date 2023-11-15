namespace FifthGroup_front.ViewModels
{
    public class ResidentViewModel
    {
        public string HouseholdCode { get; set; } = null!;

        public int CommunityBuildingId { get; set; }

        public int FloorNumber { get; set; }

        public int UnitNumber { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Headshot { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}
