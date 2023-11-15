﻿using FifthGroup_Backstage.Models;

namespace FifthGroup_Backstage.ViewModel
{
    public class CActive
    {
        public Application Applications { get; set; }
        public Reservation Reservations { get; set; }
        public ReservationPlace ReservationPlace { get; set; } //ReserveCode(用Reservations) , PlaceCode(連PublicSpaceDetail) , FDate(datetime.now()) , PeriodoftimeCode(讀取Periodoftime)
        public List<PublicSpaceDetail> PublicSpaceDetail { get; set; } //建好 讀取用 PlaceCode(int) -> Pid(string) , AreaCode(string) , PlaceName(string)
        public List<Periodoftime> Periodoftime { get; set; }   //建好 讀取   PeriodoftimeCode(int) -> Periodoftime1(string)
        public IFormFile photo { get; set; }
        public string PlaceName { get; set; }
        public string Periodoftime1 { get; set; }
    }
}
