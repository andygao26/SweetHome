using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class Email
{
    public int EmailCode { get; set; }

    public string HouseholdCode { get; set; } = null!;

    public string? Subject { get; set; }

    public string FromEmail { get; set; } = null!;

    public string ToEmail { get; set; } = null!;

    public DateTime? Time { get; set; }

    public string? Body { get; set; }

    public bool IsRead { get; set; }

    public virtual Resident HouseholdCodeNavigation { get; set; } = null!;
}
