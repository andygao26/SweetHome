using FifthGroup_Backstage.Repositories;
using FifthGroup_front.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FifthGroup_front.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;

        public BlogsController(IBlogPostRepository blogPostRepository)
        {
            this.blogPostRepository=blogPostRepository;
        }

        public async Task<IActionResult> Index(string urlHandle)
        {
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            return View(blogPost);
        }
    }
}
