using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class Announcement
{
    public int AnnouncementCode { get; set; }

    public string Title { get; set; } = null!;

    public string Kind { get; set; } = null!;

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public string? Contents { get; set; }

    public string? Pic { get; set; }

    public string? Files { get; set; }
}
