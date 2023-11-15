using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class PublicSpaceDetail
{
    public int PlaceCode { get; set; }

    public string Pid { get; set; } = null!;

    public string AreaCode { get; set; } = null!;

    public string PlaceName { get; set; } = null!;

    public virtual ICollection<ReservationPlace> ReservationPlaces { get; set; } = new List<ReservationPlace>();
}
