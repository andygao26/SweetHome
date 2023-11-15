using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class Community
{
    public int CommunityId { get; set; }

    public string CommunityName { get; set; } = null!;

    public int TotalUnits { get; set; }

    public string VerificationCode { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<CommunityBuilding> CommunityBuildings { get; set; } = new List<CommunityBuilding>();
}
