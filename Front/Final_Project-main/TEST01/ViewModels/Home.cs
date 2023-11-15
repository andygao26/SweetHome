using FifthGroup_front.Models;

namespace FifthGroup_front.ViewModels
{
    public class Home
    {

            public IEnumerable<BlogPost> BlogPosts { get; set; }

            public IEnumerable<Tag> Tags { get; set; }
    }



}
