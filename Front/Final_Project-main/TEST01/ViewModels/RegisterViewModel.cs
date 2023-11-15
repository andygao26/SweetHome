using FifthGroup_front.Models;

namespace FifthGroup_front.ViewModels
{
    public class RegisterViewModel
    {
        public IEnumerable<ResidentRegister> ResidentRegister { get; set; }
        public IEnumerable<Resident> Resident { get; set; }
    }
}
