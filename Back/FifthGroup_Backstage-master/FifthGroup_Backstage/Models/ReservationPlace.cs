using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class ReservationPlace
{
    public int Code { get; set; }

    public int ReserveCode { get; set; }

    public int PlaceCode { get; set; }

    public DateTime? FDate { get; set; }

    public int? PeriodoftimeCode { get; set; }

    public int? FState { get; set; }

    public DateTime? FDateStart { get; set; }

    public DateTime? FDateEnd { get; set; }

    public virtual Periodoftime? PeriodoftimeCodeNavigation { get; set; }

    public virtual PublicSpaceDetail PlaceCodeNavigation { get; set; } = null!;

    public virtual Reservation ReserveCodeNavigation { get; set; } = null!;
}
