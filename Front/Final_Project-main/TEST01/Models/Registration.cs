using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class Registration
{
    public int AutoCode { get; set; }

    public int ApplyCode { get; set; }

    public string HouseholdCode { get; set; } = null!;

    public int NumberOfApplicants { get; set; }

    public virtual Application ApplyCodeNavigation { get; set; } = null!;

    public virtual Resident HouseholdCodeNavigation { get; set; } = null!;
}
