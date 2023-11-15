using FifthGroup_front.Models;

namespace FifthGroup_front.ViewModels
{
    public class RepairMerge
    {
        public IEnumerable<Repair> Repair { get; set; }
        public IEnumerable<Resident> Resident { get; set; }
    }
}
