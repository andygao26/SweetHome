using System;
using System.Collections.Generic;

namespace FifthGroup_front.Models;

public partial class BlogPostTag
{
    public int BlogPostsId { get; set; }

    public int TagsId { get; set; }
}
