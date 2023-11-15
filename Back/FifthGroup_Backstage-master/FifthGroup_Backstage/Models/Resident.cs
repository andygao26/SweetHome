using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class Resident
{
    public string HouseholdCode { get; set; } = null!;

    public int CommunityBuildingId { get; set; }

    public int FloorNumber { get; set; }

    public int UnitNumber { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Headshot { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual CommunityBuilding CommunityBuilding { get; set; } = null!;

    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
