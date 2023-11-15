using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class Reservation
{
    public int ReserveCode { get; set; }

    public string? HouseholdCode { get; set; }

    public string HName { get; set; } = null!;

    public string HPhone { get; set; } = null!;

    public string HEmail { get; set; } = null!;

    public int? HState { get; set; }

    public DateTime? HOrdertime { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Resident? HouseholdCodeNavigation { get; set; }

    public virtual ICollection<ReservationPlace> ReservationPlaces { get; set; } = new List<ReservationPlace>();
}
