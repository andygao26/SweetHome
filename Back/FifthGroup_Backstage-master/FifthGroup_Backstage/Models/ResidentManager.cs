using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class ResidentManager
{
    public int AdminCode { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public string? Headshot { get; set; }
}
