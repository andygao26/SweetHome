using FifthGroup_front.Models;

namespace FifthGroup_front.ViewModels

{
    public class ActiveApplyViewModel
    {
        public int ApplyCode { get; set; }
        public Application Application { get; set; }
        public Registration Registration { get; set; }
        public int NumberOfApplicants { get; set; }
        public bool isEditMode { get; set; } = false;

        public string? PlaceName { get; set; }
        public string? Periodoftime1 { get; set; }
        public string? isPaid { get; set; }
    }
}
