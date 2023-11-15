using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class BlogPost
{
    public int Id { get; set; }

    public string Heading { get; set; } = null!;

    public string PageTitle { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public string FeacturedImageUrl { get; set; } = null!;

    public string UrlHandle { get; set; } = null!;

    public DateTime PublishedDate { get; set; }

    public string Author { get; set; } = null!;

    public bool Visible { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
