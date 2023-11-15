using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class ResidentRegister
{
    public int RegisterCode { get; set; }

    public string HouseholdCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? PersonId { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string VerifyCode { get; set; } = null!;

    public string Headshot { get; set; } = null!;
}
