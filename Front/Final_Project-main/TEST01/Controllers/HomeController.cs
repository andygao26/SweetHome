using FifthGroup_Backstage.Repositories;
using FifthGroup_front.Models;
using FifthGroup_front.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TEST01.Models;

namespace TEST01.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbHouseContext dbHouseContext;
        private readonly IBlogPostRepository blogPostRepository;

        public HomeController(ILogger<HomeController> logger,DbHouseContext dbHouseContext,IBlogPostRepository blogPostRepository)
        {
            _logger = logger;
            this.dbHouseContext = dbHouseContext;
            this.blogPostRepository=blogPostRepository;
        }

        public async Task<IActionResult> Index()
        {
            var blogPosts = await blogPostRepository.GetAllAsync();
            return View(blogPosts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}