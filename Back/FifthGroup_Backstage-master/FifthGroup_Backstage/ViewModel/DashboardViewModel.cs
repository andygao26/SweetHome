using FifthGroup_Backstage.Models;

namespace FifthGroup_Backstage.ViewModel
{
    public class DashboardViewModel
    {
        public string UserID { get; set; }
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string CommunityAddress { get; set; }
        public int CommunityTotalUnits { get; set; }
        public string UserPhotoUrl { get; set; }
        public DateTime CurrentDate { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<CommunityBuilding> CommunityBuildings { get; set; }
        public List<ApplicationData> ApplicationsData { get; set; }
        public int UnprocessedCount { get; set; }
        public int StateZeroCount { get; set; }
    }

    public class ApplicationData
    {
        public string ActivityName { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int Applicants { get; set; }
        public int MaxApplicants { get; set; }
        public double EnrollmentRate { get; set; }
    }
}
