using FifthGroup_front.Models;

namespace FifthGroup_front.ViewModels
{
    public class ActiveFromResident
    {
        public Resident Resident { get; set; }
        public Registration Registration { get; set; }
        public bool isPaid { get; set; }
    }
}
