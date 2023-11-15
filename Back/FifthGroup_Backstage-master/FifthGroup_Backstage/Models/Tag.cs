using System;
using System.Collections.Generic;

namespace FifthGroup_Backstage.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
