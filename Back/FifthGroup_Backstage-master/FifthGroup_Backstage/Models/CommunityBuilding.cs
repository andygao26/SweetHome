using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class CommunityBuilding
{
    public int CommunityBuildingId { get; set; }

    public int CommunityId { get; set; }

    public string BuildingName { get; set; } = null!;

    public int FloorNumber { get; set; }

    public int UnitNumber { get; set; }

    public virtual Community Community { get; set; } = null!;

    public virtual ICollection<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();

    public virtual ICollection<Resident> Residents { get; set; } = new List<Resident>();
}
