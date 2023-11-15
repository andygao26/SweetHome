using FifthGroup_Backstage.Models;

namespace FifthGroup_Backstage.ViewModel
{
    public class CReservations_four
    {
        public IEnumerable<Reservation>? Reservations { get; set; }
        public IEnumerable<ReservationPlace>? ReservationPlace { get; set; }
        public IEnumerable<PublicSpaceDetail>? PublicSpaceDetail { get; set; }
        public IEnumerable<Periodoftime>? Periodoftime { get; set; }

    }
}
