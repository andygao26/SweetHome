using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class ResidentRe
{
    public int RegisterCode { get; set; }

    public string? PersonId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? VerifyCode { get; set; }
}
