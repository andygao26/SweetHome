using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class Application
{
    public int ApplyCode { get; set; }

    public string ActivityName { get; set; } = null!;

    public string? HouseholdCode { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Activities { get; set; } = null!;

    public int? ReserveCode { get; set; }

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public string Email { get; set; } = null!;

    public int State { get; set; }

    public string? Introduce { get; set; }

    public string? Image { get; set; }

    public int? Applicants { get; set; }

    public int? MaxApplicants { get; set; }

    public int? PaymentItemCode { get; set; }

    public virtual Resident? HouseholdCodeNavigation { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual Reservation? ReserveCodeNavigation { get; set; }
}
