using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class Periodoftime
{
    public int PeriodoftimeCode { get; set; }

    public string? Periodoftime1 { get; set; }

    public virtual ICollection<ReservationPlace> ReservationPlaces { get; set; } = new List<ReservationPlace>();
}
