using FifthGroup_front.Models;

namespace FifthGroup_front.ViewModels
{
    public class CReservations_four
    {
        public IEnumerable<Reservation>? Reservations { get; set; }
        public IEnumerable<ReservationPlace>? ReservationPlace { get; set; }
        public IEnumerable<PublicSpaceDetail>? PublicSpaceDetail { get; set; }
        public IEnumerable<Periodoftime>? Periodoftime { get; set; }
        public int Code { get; set; }

        public int ReserveCode { get; set; }

        public int PlaceCode { get; set; }

        public DateTime? FDate { get; set; }

        public int? PeriodoftimeCode { get; set; }
        public int? FState { get; set; }

    }
}
