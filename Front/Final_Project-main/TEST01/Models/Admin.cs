using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class Admin
{
    public string UserId { get; set; } = null!;

    public string UserPwd { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string? UserPhone { get; set; }

    public string? UserAddress { get; set; }

    public string? UserPhoto { get; set; }

    public string? VerificationCode { get; set; }

    public bool? IsVerified { get; set; }

    public int? CommunityId { get; set; }

    public virtual Community? Community { get; set; }
}
